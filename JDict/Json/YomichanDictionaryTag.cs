using Newtonsoft.Json;

namespace JDict.Json
{
    [JsonConverter(typeof(YomichanDictionaryTagConverter))]
    class YomichanDictionaryTag
    {
        public string Name { get; set; }
        
        public string Category { get; set; }
        
        public int Order { get; set; }
        
        public string Notes { get; set; }
        
        public int Score { get; set; }
    }
}