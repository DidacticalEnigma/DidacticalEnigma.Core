using System.Collections.Generic;

namespace JDict
{
    public class TatoebaLink
    {
        public long SentenceId { get; }

        public long TranslationId { get; }

        public TatoebaLink(long sentenceId, long translationId)
        {
            SentenceId = sentenceId;
            TranslationId = translationId;
        }

        public KeyValuePair<long, long> AsKeyValuePair()
        {
            return new KeyValuePair<long, long>(SentenceId, TranslationId);
        }
    }
}