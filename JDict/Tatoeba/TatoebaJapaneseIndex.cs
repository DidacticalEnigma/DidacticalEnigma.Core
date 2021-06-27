using System;

namespace JDict
{
    public class TatoebaJapaneseIndex
    {
        public long JapaneseSentenceId { get; }

        public long EnglishSentenceId { get; }

        public string Index { get; }

        public TatoebaJapaneseIndex(long japaneseSentenceId, long englishSentenceId, string index)
        {
            JapaneseSentenceId = japaneseSentenceId;
            EnglishSentenceId = englishSentenceId;
            Index = index ?? throw new ArgumentNullException(nameof(index));
        }
    }
}