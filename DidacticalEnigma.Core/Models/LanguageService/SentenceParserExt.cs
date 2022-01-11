using System.Collections.Generic;
using System.Linq;

namespace DidacticalEnigma.Core.Models.LanguageService;

public static class SentenceParserExt
{
    public static IEnumerable<WordInfo> BreakIntoWords(this ISentenceParser parser, string input)
    {
        return parser.BreakIntoSentences(input).SelectMany(x => x);
    }
}