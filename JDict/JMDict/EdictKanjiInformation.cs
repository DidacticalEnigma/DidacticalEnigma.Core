using System.ComponentModel;

namespace JDict
{
    public enum EdictKanjiInformation
    {
        // reserving 0 for an "unknown"
        [Description("ateji (phonetic) reading")]
        ateji = 1,
        [Description("word containing irregular kanji usage")]
        iK,
        [Description("word containing irregular kana usage")]
        ik,
        [Description("irregular okurigana usage")]
        io,
        [Description("word containing out-dated kanji")]
        oK,
        [Description("rarely-used kanji form")]
        rK,
    }
}