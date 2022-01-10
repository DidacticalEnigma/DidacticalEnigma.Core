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

        public IEnumerable<WordInfo> BreakIntoWords(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Enumerable.Empty<WordInfo>();

            return analyzer.ParseToEntries(input)
                .Where(a => a.IsRegular)
                .Select(word =>
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
                });
        }
    }
}