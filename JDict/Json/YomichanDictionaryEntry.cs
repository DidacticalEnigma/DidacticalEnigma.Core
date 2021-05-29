using System.Collections.Generic;
using Newtonsoft.Json;

namespace JDict.Json
{
    [JsonConverter(typeof(YomichanDictionaryEntryConverter))]
    class YomichanDictionaryEntry
    {
        public long Id { get; set; }

        public string Expression { get; set; }

        public string Reading { get; set; }

        public string DefinitionTags { get; set; }

        public string Rules { get; set; }

        public int Score { get; set; }

        public IEnumerable<string> Glossary { get; set; }

        public int Sequence { get; set; }

        public string TermTags { get; set; }
    }
}