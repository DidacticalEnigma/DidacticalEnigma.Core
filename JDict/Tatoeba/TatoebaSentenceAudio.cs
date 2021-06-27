using System;
using Optional;

namespace JDict
{
    public class TatoebaSentenceAudio
    {
        public long SentenceId { get; }

        public string Username { get; }

        public License License { get; }

        public Option<Uri> AttributionUrl { get; }

        public TatoebaSentenceAudio(long sentenceId, string username, License license, Option<Uri> attributionUrl)
        {
            SentenceId = sentenceId;
            Username = username ?? throw new ArgumentNullException(nameof(username));
            License = license ?? throw new ArgumentNullException(nameof(license));
            AttributionUrl = attributionUrl;
        }
    }
}