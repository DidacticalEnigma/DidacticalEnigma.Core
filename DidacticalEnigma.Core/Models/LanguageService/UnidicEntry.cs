using System;
using System.Collections.Generic;
using System.Linq;
using JDict;
using Optional;
using Utility.Utils;

namespace DidacticalEnigma.Core.Models.LanguageService
{
    public class UnidicEntry : IEntry
    {
        public UnidicEntry(string surfaceForm, Option<string> feature)
        {
            SurfaceForm = surfaceForm;
            IsRegular = feature.HasValue;
            if (!IsRegular)
                return;
            
            var features = feature
                .Map(f => StringExt.SplitWithQuotes(f, ',', '"').ToArray())
                .ValueOr(Array.Empty<string>());

            PartOfSpeech = MeCabEntryParser.PartOfSpeechFromString(MeCabEntryParser.OrNull(features, 0)); // f[0]:  pos1
            ConjugatedForm = MeCabEntryParser.OrNull(features, 5); // ; f[5]:  cForm
            DictionaryForm = MeCabEntryParser.OrNull(features,　10); // ; f[10]: orthBase
            Pronunciation = MeCabEntryParser.OrNull(features, 9); // ; f[9]:  pron
            Reading = MeCabEntryParser.OrNull(features, 17); // ; f[17]: kana
            Type = MeCabEntryParser.TypeFromString(ConjugatedForm);
            // ; f[1]:  pos2
            // ; f[2]:  pos3
            // ; f[3]:  pos4
            PartOfSpeechSections = features
                .Skip(1)
                .Take(3)
                .Where(f => f != "*")
                .ToList()
                .AsReadOnly();
        }

        public string ConjugatedForm { get; }
        public string Inflection { get; }
        public bool IsRegular { get; }
        public string SurfaceForm { get; }
        public PartOfSpeech PartOfSpeech { get; }

        public IEnumerable<string> PartOfSpeechSections { get; }
        public string Pronunciation { get; }
        public string Reading { get; }
        public string DictionaryForm { get; }
        public Option<EdictPartOfSpeech> Type { get; }
    }
}