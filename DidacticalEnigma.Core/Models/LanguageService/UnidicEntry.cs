using System;
using System.Collections.Generic;
using System.Linq;
using JDict;
using Optional;
using Utility.Utils;

namespace DidacticalEnigma.Core.Models.LanguageService
{
    /*
    ; List of features
    ; f[0]:  pos1
    ; f[1]:  pos2
    ; f[2]:  pos3
    ; f[3]:  pos4
    ; f[4]:  cType
    ; f[5]:  cForm
    ; f[6]:  lForm
    ; f[7]:  lemma
    ; f[8]:  orth
    ; f[9]:  pron
    ; f[10]: orthBase
    ; f[11]: pronBase
    ; f[12]: goshu
    ; f[13]: iType
    ; f[14]: iForm
    ; f[15]: fType
    ; f[16]: fForm
    ; f[17]: iConType
    ; f[18]: fConType
    ; f[19]: type
    ; f[20]: kana
    ; f[21]: kanaBase
    ; f[22]: form
    ; f[23]: formBase
    ; f[24]: aType
    ; f[25]: aConType
    ; f[26]: aModType
    ; f[27]: lid
    ; f[28]: lemma_id
     */
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
            Reading = MeCabEntryParser.OrNull(features, 20); // ; f[20]: kana
            DictionaryFormReading = MeCabEntryParser.OrNull(features, 21); // ; f[21]: kanaBase
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

        public string DictionaryFormReading { get; }
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