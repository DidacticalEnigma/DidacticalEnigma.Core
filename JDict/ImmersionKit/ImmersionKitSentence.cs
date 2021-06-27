using Newtonsoft.Json;

namespace JDict.ImmersionKit
{
    public class ImmersionKitSentence
    {
        [JsonProperty("sentence")]
        public string Sentence { get; }
        
        [JsonProperty("translation")]
        public string Translation { get; }

        public ImmersionKitSentence(string sentence, string translation)
        {
            Sentence = sentence;
            Translation = translation;
        }
    }
}