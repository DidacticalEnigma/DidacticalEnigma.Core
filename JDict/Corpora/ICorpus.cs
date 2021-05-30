using System.Collections.Async;
using System.Collections.Generic;

namespace JDict
{
    public interface ICorpus
    {
        IEnumerable<SentencePair> AllSentences();
        IAsyncEnumerable<SentencePair> AllSentencesAsync();
        IEnumerable<SentencePair> SearchByJapaneseText(string text);
        IAsyncEnumerable<SentencePair> SearchByJapaneseTextAsync(string text);
    }
}