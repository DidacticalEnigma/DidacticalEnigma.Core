﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JDict;

namespace DidacticalEnigma.Core.Models.LanguageService
{
    public class PartialWordLookup
    {
        private readonly IRadicalSearcher searcher;

        private readonly KanjiRadicalLookup lookup;

        private readonly string allWords;

        private static readonly char start = '\x0002';

        private static readonly char[] startArr = { start };

        private static readonly char end = '\x0001';

        private static readonly char[] endArr = { end };

        private static Regex groupMatcher = new Regex(@"\\\[(.*?)\]");

        public PartialWordLookup(JMDictLookup jmDictLookup, IRadicalSearcher searcher, KanjiRadicalLookup lookup)
        {
            this.searcher = searcher;
            this.lookup = lookup;
            allWords = string.Join("", jmDictLookup.AllEntries()
                .SelectMany(entry => entry.KanjiEntries.Select(k => k.Kanji).Concat(entry.ReadingEntries.Select(r => r.Reading)))
                .Distinct()
                .Select(word => start + word + end));
        }

        public IEnumerable<string> LookupWords(string input)
        {
            var escaped = Regex.Escape(start + input + end);
            if (PatternMatchesEveryWord(input))
            {
                return Enumerable.Empty<string>();
            }
            
            var groupsReplaced = groupMatcher.Replace(escaped, match =>
            {
                var text = match.Groups[1].Value;
                var kanjiCandidates = lookup.SelectRadical(searcher.Search(text).Select(r => r.Radical)).Kanji;
                return "(" + string.Join("|", kanjiCandidates.Select(k => k.ToString())) + ")";
            });
            var regex = new Regex(
                groupsReplaced
                    .Replace(@"\?", ".")
                    .Replace(@"？", ".")
                    .Replace(@"\*", $"[^{end}]*?")
                    .Replace(@"＊", $"[^{end}]*?"));
            return regex.Matches(allWords)
                .Cast<Match>()
                .Select(m => m.Value.TrimStart(startArr).TrimEnd(endArr))
                .ToList();
        }

        private bool PatternMatchesEveryWord(string input)
        {
            return input
                .Replace(@"?", "")
                .Replace(@"？", "")
                .Replace(@"*", "")
                .Replace(@"＊", "")
                .Length == 0;
        }
    }
}
