using System;
using System.Collections.Generic;
using System.Linq;
using DidacticalEnigma.Core.Models.LanguageService;
using JDict;
using Optional;
using Utility.Utils;

namespace DidacticalEnigma.Core.Models
{
    public class SentenceParser : ISentenceParser
    {
        private readonly IMorphologicalAnalyzer<IEntry> analyzer;
        private readonly JMDictLookup lookup;
        private readonly IKanaProperties kanaProperties;

        public SentenceParser(
            IMorphologicalAnalyzer<IEntry> analyzer,
            JMDictLookup lookup,
            IKanaProperties kanaProperties)
        {
            this.analyzer = analyzer;
            this.lookup = lookup;
            this.kanaProperties = kanaProperties;
        }

        public IEnumerable<IEnumerable<WordInfo>> BreakIntoSentences(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                yield break;
            }

            var entries = analyzer.ParseToEntries(input)
                .Where(a => a.IsRegular);

            var list = new List<WordInfo>();
            int previousIndex = 0;
            int currentIndex = 0;
            foreach (var entry in entries)
            {
                previousIndex = currentIndex;
                currentIndex = input.IndexOf(entry.SurfaceForm, currentIndex, StringComparison.Ordinal);
                var newlines = input.SubstringFromTo(previousIndex, currentIndex).ReplaceLineEndings("\n")
                    .Count(c => c == '\n');
                for (int i = 0; i < newlines; i++)
                {
                    yield return list;
                    list = new List<WordInfo>();
                }
                list.Add(Map(entry));
            }

            if (list.Count != 0)
            {
                yield return list;
            }

            WordInfo Map(IEntry word)
            {
                var reading = word.Reading ??
                              lookup.Lookup(word.DictionaryForm ?? word.SurfaceForm)?.FirstOrDefault()
                                  ?.ReadingEntries.First().Reading;
                return new WordInfo(
                    word.SurfaceForm,
                    word.PartOfSpeech,
                    word.DictionaryForm,
                    word.GetPartOfSpeechInfo().Contains(PartOfSpeechInfo.Pronoun)
                        ? Option.Some(EdictPartOfSpeech.pn)
                        : word.Type,
                    reading,
                    word is UnidicEntry unidicEntry
                        ? unidicEntry.DictionaryFormReading
                        : null);
            };
        }
    }
}