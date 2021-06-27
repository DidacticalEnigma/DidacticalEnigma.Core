namespace JDict
{
    public class TatoebaListSentenceLink
    {
        public long ListId { get; }

        public long SentenceId { get; }

        public TatoebaListSentenceLink(long listId, long sentenceId)
        {
            ListId = listId;
            SentenceId = sentenceId;
        }
    }
}