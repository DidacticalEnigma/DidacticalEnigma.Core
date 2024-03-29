﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JDict;
using Optional;
using Optional.Collections;
using TinyIndex;
using Utility.Utils;

namespace DidacticalEnigma.Core.Models.LanguageService
{
    public class IdiomDetector : IDisposable
    {
        private static readonly Guid Version = new Guid("2CAE16ED-6325-40A3-91FA-71A7348A2EC1");

        private JMDictLookup jmDict;

        private readonly IMorphologicalAnalyzer<IEntry> analyzer;

        private Database db;

        private IReadOnlyDiskArray<KeyValuePair<string, long>> entries;

        public class Result
        {
            public JMDictEntry DictionaryEntry { get; }

            public IEnumerable<(string fragment, bool highlight)> RenderedHighlights { get; }

            // arbitrary measure by which a specific result is "better" than other
            public double Similarity { get; }

            public Result(JMDictEntry dictionaryEntry, IEnumerable<(string fragment, bool highlight)> renderedHighlights, double similarity)
            {
                DictionaryEntry = dictionaryEntry;
                RenderedHighlights = renderedHighlights;
                Similarity = similarity;
            }
        }

        private List<(string fragment, bool highlight)> Concatenate(List<(string fragment, bool highlight)> input)
        {
            return input
                .GroupConsecutive(x => x.highlight)
                .Select(x => x.Materialize())
                .Select(x => (string.Join("", x.Select(y => y.fragment)), x.First().highlight))
                .ToList();
        }

        private string OriginalNotNormalized(string normalized, JMDictEntry entry)
        {
            return entry.KanjiEntries
                .Select(k => k.Kanji)
                .Concat(entry.ReadingEntries.Select(r => r.Reading))
                .FirstOrNone(s => Normalize(s) == normalized)
                .ValueOr(normalized);
        }

        private double Similarity(IEntry left, IEntry right)
        {
            if (left.SurfaceForm == right.SurfaceForm)
            {
                return 1.0;
            }
            else if (left.DictionaryForm != null && left.DictionaryForm == right.DictionaryForm)
            {
                return 0.5;
            }

            return 0.0;
        }

        private Option<Result> Rate(
            IReadOnlyList<IEntry> queryMorphemes,
            string candidate,
            long candidateId)
        {
            return jmDict.LookupBySequenceNumber(candidateId).Map(candidateEntry =>
            {
                double similarity = 0.0;
                var remainder = candidate.Substring(candidate.IndexOf('\0')+1);
                var highlights = new List<(string fragment, bool highlight)>();
                
                candidate = candidate.Substring(0, candidate.IndexOf('\0'));
                var original = OriginalNotNormalized(remainder + candidate, candidateEntry);

                int j = 0;
                var analyzedCandidate = Analyze(original);
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                while (j < analyzedCandidate.Count && Similarity(queryMorphemes[0], analyzedCandidate[j]) == 0.0)
                {
                    highlights.Add((analyzedCandidate[j].SurfaceForm, false));
                    j++;
                }

                bool mismatch = false;
                for (int i = 0; j < analyzedCandidate.Count;)
                {
                    // use a sentinel
                    var queryMorpheme = i < queryMorphemes.Count
                        ? queryMorphemes[i]
                        : new IpadicEntry("", Option.None<string>());
                    var candidateMorpheme = analyzedCandidate[j];

                    
                    if (mismatch)
                    {
                        i++;
                        j++;
                        highlights.Add((candidateMorpheme.SurfaceForm, false));
                        continue;
                    }

                    var morphemeSimilarity = Similarity(queryMorpheme, candidateMorpheme);
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (morphemeSimilarity != 0.0)
                    {
                        similarity += morphemeSimilarity;
                        i++;
                        j++;
                        highlights.Add((candidateMorpheme.SurfaceForm, true));
                    }
                    else if(queryMorpheme.PartOfSpeech == PartOfSpeech.Particle && candidateMorpheme.PartOfSpeech == PartOfSpeech.Particle)
                    {
                        i++;
                        j++;
                        highlights.Add((candidateMorpheme.SurfaceForm, false));
                    }
                    else if (queryMorpheme.PartOfSpeech == PartOfSpeech.Particle)
                    {
                        i++;
                    }
                    else if (candidateMorpheme.PartOfSpeech == PartOfSpeech.Particle)
                    {
                        j++;
                        highlights.Add((candidateMorpheme.SurfaceForm, false));
                    }
                    else
                    {
                        mismatch = true;
                        i++;
                        j++;
                        highlights.Add((candidateMorpheme.SurfaceForm, false));
                    }
                }

                return new Result(candidateEntry, Concatenate(highlights), similarity / queryMorphemes.Count);
            });
        }

        public IEnumerable<Result> Detect(string p)
        {
            int limit = 50;
            var analyzed = Analyze(p);
            var normalized = Normalize(analyzed);
            var (start, end) = entries.EqualRange(normalized, kvp => kvp.Key);
            var outOfBoundLimit = (limit - (end - start))/2;
            start = Math.Max(0, start - outOfBoundLimit);
            end = Math.Min(entries.Count, end + outOfBoundLimit);
            var mid = (start + end)/2;
            var resultEntries =
                EnumerableExt.Zip(
                        EnumerableExt.Range(start, end - start),
                        entries.GetIdRange(start, end))
                    .OrderBy(kvp => Math.Abs(kvp.Item1 - mid))
                    .Select(kvp => Rate(analyzed, kvp.Item2.Key, kvp.Item2.Value))
                    .Values()
                    .Where(r => r.Similarity > 0)
                    .OrderByDescending(r => r.Similarity);
            return 
                EnumerableExt.DistinctBy(resultEntries, r => r.DictionaryEntry.SequenceNumber);
        }

        public IdiomDetector(
            JMDictLookup jmDict,
            IMorphologicalAnalyzer<IpadicEntry> analyzer,
            string cachePath)
        {
            this.jmDict = jmDict;
            this.analyzer = analyzer;
            db = Database.CreateOrOpen(cachePath, Version)
                .AddIndirectArray(Serializer.ForKeyValuePair(Serializer.ForStringAsUtf8(), Serializer.ForLong()),
                    CreateEntries, kvp => kvp.Key)
                .Build();

            entries = db.Get<KeyValuePair<string, long>>(0);
        }

        private IEnumerable<KeyValuePair<string, long>> CreateEntries(Database db)
        {
            var exprs = jmDict.AllEntries()
                .Where(entry =>
                    entry.Senses.Any(s =>
                        s.PartOfSpeechInfo.Any(pos => pos == EdictPartOfSpeech.exp)))
                .SelectMany(entry => entry.KanjiEntries.Select(k => k.Kanji).Concat(entry.ReadingEntries.Select(r => r.Reading)).Select(e => new KeyValuePair<string, long>(Normalize(e), entry.SequenceNumber)));
            var rotations = exprs
                .SelectMany(e =>
                    StringExt.AllRotationsOf(e.Key + "\0")
                    .Select(r => new KeyValuePair<string, long>(r, e.Value)))
                    .Where(kvp => !kvp.Key.StartsWith("\0", StringComparison.Ordinal));
            return rotations;
        }

        private string Normalize(string s)
        {
            var morphemes = Analyze(s);
            return Normalize(morphemes);
        }

        private string Normalize(IEnumerable<IEntry> morphemes)
        {
            return string.Join("", morphemes
                .Where(e => e.PartOfSpeech != PartOfSpeech.Particle)
                .Select(m => m.DictionaryForm));
        }

        private IReadOnlyList<IEntry> Analyze(string s)
        {
            var morphemes = analyzer.ParseToEntries(s)
                .Where(e => e.IsRegular)
                .ToList();
            return morphemes;
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
