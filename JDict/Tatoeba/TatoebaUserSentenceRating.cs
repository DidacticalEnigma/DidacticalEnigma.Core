using System;

namespace JDict
{
    public class TatoebaUserSentenceRating
    {
        public string Username { get; }

        public string Language { get; }

        public long SentenceId { get; }

        public Rating Rating { get; }

        public DateTime DateAdded { get; }

        public DateTime DateModified { get; }

        public TatoebaUserSentenceRating(
            string username,
            string language,
            long sentenceId,
            Rating rating,
            DateTime dateAdded,
            DateTime dateModified)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Language = language ?? throw new ArgumentNullException(nameof(language));
            SentenceId = sentenceId;
            Rating = rating;
            DateAdded = dateAdded;
            DateModified = dateModified;
        }
    }
}