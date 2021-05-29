using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JDict.Json
{
    class YomichanDictionaryTagConverter : JsonConverter<YomichanDictionaryTag>
    {
        public override void WriteJson(JsonWriter writer, YomichanDictionaryTag value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, new object[]
            {
                value.Name,
                value.Category,
                value.Order,
                value.Notes,
                value.Score,
            });
        }

        public override YomichanDictionaryTag ReadJson(
            JsonReader reader,
            Type objectType,
            YomichanDictionaryTag existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var arr = serializer.Deserialize<JArray>(reader);
            var newValue = new YomichanDictionaryTag
            {
                Name = arr[0].Value<string>(),
                Category = arr[1].Value<string>(),
                Order = arr[2].Value<int>(),
                Notes = arr[3].Value<string>(),
                Score = arr[4].Value<int>(),
            };
            return newValue;
        }
    }
}