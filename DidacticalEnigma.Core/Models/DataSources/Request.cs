﻿using System;
using System.Collections.Generic;
using DidacticalEnigma.Core.Models.LanguageService;

namespace DidacticalEnigma.Core.Models.DataSources
{
    public class Request
    {
        public string Character { get; }

        public WordInfo Word { get; }

        public string QueryText { get; }

        public PartOfSpeech PartOfSpeech { get; }

        public string NotInflected { get; }

        public Func<string> AllText { get; }

        public IEnumerable<string> SubsequentWords { get; }

        public Request(string character, WordInfo word, string queryText, Func<string> allText, IEnumerable<string> subsequentWords = null)
        {
            Character = character;
            Word = word;
            QueryText = queryText;
            PartOfSpeech = word.EstimatedPartOfSpeech;
            NotInflected = word.DictionaryForm;
            SubsequentWords = subsequentWords;
            AllText = allText;
        }
    }
}