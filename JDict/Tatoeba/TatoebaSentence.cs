using System;

namespace JDict
{
    public class TatoebaSentence
    {
        public long SentenceId { get; }

        public string Language { get; }

        public string Text { get; }

        public TatoebaSentence(long sentenceId, string language, string text)
        {
            SentenceId = sentenceId;
            Language = language ?? throw new ArgumentNullException(nameof(language));
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }
    }
}