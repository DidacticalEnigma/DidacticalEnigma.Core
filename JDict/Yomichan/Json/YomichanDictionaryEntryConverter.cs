using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JDict.Json
{
    class YomichanDictionaryEntryConverter : JsonConverter<YomichanDictionaryEntry>
    {
        public override void WriteJson(JsonWriter writer, YomichanDictionaryEntry value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, new object[]
            {
                value.Expression,
                value.Reading,
                value.DefinitionTags,
                value.Rules,
                value.Score,
                value.Glossary,
                value.Sequence,
                value.TermTags
            });
        }

        public override YomichanDictionaryEntry ReadJson(
            JsonReader reader,
            Type objectType,
            YomichanDictionaryEntry existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var arr = serializer.Deserialize<JArray>(reader);
            var newValue = new YomichanDictionaryEntry
            {
                Expression = arr[0].Value<string>(),
                Reading = arr[1].Value<string>(),
                DefinitionTags = arr[2].Value<string>(),
                Rules = arr[3].Value<string>(),
                Score = arr[4].Value<int>(),
                Glossary = arr[5].Values<string>().ToList(),
                Sequence = arr[6].Value<int>(),
                TermTags = arr[7].Value<string>()
            };
            return newValue;
        }
    }
}