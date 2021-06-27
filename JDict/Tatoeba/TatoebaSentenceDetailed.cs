using System;
using Optional;

namespace JDict
{
    public class TatoebaSentenceDetailed
    {
        public long SentenceId { get; }

        public string Language { get; }

        public string Text { get; }

        public string Username { get; }

        public Option<DateTime> DateAdded { get; }

        public Option<DateTime> DateModified { get; }

        public TatoebaSentenceDetailed(
            long sentenceId,
            string language,
            string text,
            string username,
            Option<DateTime> dateAdded = default,
            Option<DateTime> dateModified = default)
        {
            SentenceId = sentenceId;
            Language = language ?? throw new ArgumentNullException(nameof(language));
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Username = username ?? throw new ArgumentNullException(nameof(username));
            DateAdded = dateAdded;
            DateModified = dateModified;
        }

        public TatoebaSentence AsSentence()
        {
            return new TatoebaSentence(SentenceId, Language, Text);
        }
    }
}