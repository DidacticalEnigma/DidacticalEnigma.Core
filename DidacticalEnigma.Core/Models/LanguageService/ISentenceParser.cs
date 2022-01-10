using System.Collections.Generic;

namespace DidacticalEnigma.Core.Models.LanguageService
{
    public interface ISentenceParser
    {
        IEnumerable<WordInfo> BreakIntoWords(string input);
    }
}