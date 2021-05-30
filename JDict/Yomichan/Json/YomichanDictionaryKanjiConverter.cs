using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JDict.Json
{
    class YomichanDictionaryKanjiConverter : JsonConverter<YomichanDictionaryKanji>
    {
        public override void WriteJson(JsonWriter writer, YomichanDictionaryKanji value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, new object[]
            {
                value.Character,
                value.Onyomi,
                value.Kunyomi,
                value.Tags,
                value.Meanings,
                value.Stats,
            });
        }

        public override YomichanDictionaryKanji ReadJson(
            JsonReader reader,
            Type objectType,
            YomichanDictionaryKanji existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var arr = serializer.Deserialize<JArray>(reader);
            var newValue = new YomichanDictionaryKanji()
            {
                Character = arr[0].Value<string>(),
                Onyomi = arr[1].Values<string>().ToList(),
                Kunyomi = arr[2].Values<string>().ToList(),
                Tags = arr[3].Values<string>().ToList(),
                Meanings = arr[4].Values<string>().ToList(),
                Stats = arr[5].ToObject<Dictionary<string, string>>(),
            };
            return newValue;
        }
    }
}