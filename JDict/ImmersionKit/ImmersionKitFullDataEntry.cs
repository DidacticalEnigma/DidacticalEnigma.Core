using System.Collections.Generic;
using Newtonsoft.Json;

namespace JDict.ImmersionKit
{
    public class ImmersionKitFullDataEntry : ImmersionKitSentence
    {
        [JsonProperty("id")]
        public string Id { get; }
        
        [JsonProperty("deck_name")]
        public string DeckName { get; }
        
        [JsonProperty("sentence_with_furigana")]
        public string SentenceWithFurigana { get; }
        
        [JsonProperty("word_base_list")]
        public IReadOnlyCollection<string> WordBaseList { get; }
        
        [JsonProperty("word_list")]
        public IReadOnlyCollection<string> WordList { get; }
        
        [JsonProperty("translation_word_list")]
        public IReadOnlyCollection<string> TranslationWordList { get; }
        
        [JsonProperty("translation_word_base_list")]
        public IReadOnlyCollection<string> TranslationWordBaseList { get; }
        
        [JsonProperty("image")]
        public string ImagePath { get; }
        
        [JsonProperty("sound")]
        public string SoundPath { get; }

        public ImmersionKitFullDataEntry(
            string sentence,
            string translation,
            string id,
            string deckName,
            string sentenceWithFurigana,
            IReadOnlyCollection<string> wordBaseList,
            IReadOnlyCollection<string> wordList,
            IReadOnlyCollection<string> translationWordList,
            IReadOnlyCollection<string> translationWordBaseList,
            string imagePath,
            string soundPath)
            : base(sentence, translation)
        {
            Id = id;
            DeckName = deckName;
            SentenceWithFurigana = sentenceWithFurigana;
            WordBaseList = wordBaseList;
            WordList = wordList;
            TranslationWordList = translationWordList;
            TranslationWordBaseList = translationWordBaseList;
            ImagePath = imagePath;
            SoundPath = soundPath;
        }
    }
}