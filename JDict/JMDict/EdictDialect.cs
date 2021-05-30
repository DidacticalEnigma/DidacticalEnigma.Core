using System.ComponentModel;

namespace JDict
{
    public enum EdictDialect
    {
        // reserving 0 for an "unknown"
        [Description("Kyoto-ben")]
        kyb = 1,
        [Description("Osaka-ben")]
        osb,
        [Description("Kansai-ben")]
        ksb,
        [Description("Kantou-ben")]
        ktb,
        [Description("Tosa-ben")]
        tsb,
        [Description("Touhoku-ben")]
        thb,
        [Description("Tsugaru-ben")]
        tsug,
        [Description("Kyuushuu-ben")]
        kyu,
        [Description("Ryuukyuu-ben")]
        rkb,
        [Description("Hokkaido-ben")]
        hob,
        [Description("Nagano-ben")]
        nab,
    }
}