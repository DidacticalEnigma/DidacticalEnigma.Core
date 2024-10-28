using System.ComponentModel;

namespace JDict
{
    public enum EdictReadingInformation
    {
        // reserving 0 for an "unknown"
        [Description("gikun (meaning as reading) or jukujikun (special kanji reading)")]
        gikun = 1,
        [Description("word containing irregular kana usage")]
        ik,
        [Description("out-dated or obsolete kana usage")]
        ok,
        [Description("old or irregular kana form")]
        oik,
        [Description("word usually written using kanji alone")]
        uK,
        [Description("search-only kana form")]
        sk,
        [Description("rarely used kana form")]
        rk,
    }
}