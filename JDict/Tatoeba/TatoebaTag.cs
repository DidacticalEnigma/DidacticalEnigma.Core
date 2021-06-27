using System;

namespace JDict
{
    public class TatoebaTag
    {
        public long SentenceId { get; }

        public string TagName { get; }

        public TatoebaTag(long sentenceId, string tagName)
        {
            SentenceId = sentenceId;
            TagName = tagName ?? throw new ArgumentNullException(nameof(tagName));
        }
    }
}