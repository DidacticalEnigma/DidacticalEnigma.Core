using System.Collections.Generic;
using Newtonsoft.Json;

namespace JDict.Json
{
    [JsonConverter(typeof(YomichanDictionaryKanjiConverter))]
    class YomichanDictionaryKanji
    {
        public string Character { get; set; }
        
        public IReadOnlyList<string> Onyomi { get; set; }
        
        public IReadOnlyList<string> Kunyomi { get; set; }
        
        public IReadOnlyList<string> Tags { get; set; }
        
        public IReadOnlyList<string> Meanings { get; set; }
        
        public IReadOnlyDictionary<string, string> Stats { get; set; }
    }
}