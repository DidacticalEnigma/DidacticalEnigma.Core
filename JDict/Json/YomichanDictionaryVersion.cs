using Newtonsoft.Json;

namespace JDict.Json
{
    // {"title":"研究社　新和英大辞典　第５版","format":3,"revision":"wadai1","sequenced":true}
    
    class YomichanDictionaryVersion
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("format")]
        public int Format { get; set; }

        [JsonProperty("revision")]
        public string Revision { get; set; }

        [JsonProperty("sequenced")]
        public bool Sequenced { get; set; }
    }
}