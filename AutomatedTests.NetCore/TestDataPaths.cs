﻿using System;
using System.IO;

namespace AutomatedTests
{
    public class TestDataPaths
    {
        // FIX THIS PATH SO IT POINTS TO THE ACTUAL DIRECTORY YOUR DATA IS
        // Various test runners put the executables in unrelated places,
        // and also make the current directory unrelated.
        public static readonly string BaseDir = Environment.GetEnvironmentVariable("APPVEYOR_BUILD_FOLDER") is { } envPath
            ? Path.Combine(envPath, "Data")
            : @"/home/milleniumbug/dokumenty/PROJEKTY/InDevelopment/DidacticalEnigma/Data";

        public static readonly string TestDir = Environment.GetEnvironmentVariable("APPVEYOR_BUILD_FOLDER") is { } envPath
            ? Path.Combine(envPath, "TestData")
            : @"/home/milleniumbug/dokumenty/PROJEKTY/InDevelopment/DidacticalEnigma/TestData";

        // POINT THIS TO A yomichan-import CREATED ZIP FILE FROM "研究社　新和英大辞典　第５版" EPWING DICTIONARY
        // Can't attach this in a repo for obvious copyright reasons
        public static readonly string Kenkyusha5 = Path.Combine(BaseDir, "epwing", "kenkyusha5.zip");

        // sanity check for above path, SHA256 hash of the file
        public static readonly string Kenkyusha5Hash =
            "C12FE5AE242F299DB11DBC53FC05FDF44BEFC81303E92B19BCC5B4D758EF234C";

        public static readonly string MagicTranslatorTestDir = Path.Combine(TestDir, "MTProjects");

        public static readonly string Ipadic = Path.Combine(BaseDir, "mecab", "ipadic");

        public static readonly string Unidic = Path.Combine(BaseDir, "mecab", "unidic");

        public static readonly string JMDict = Path.Combine(BaseDir, "dictionaries", "JMdict_e.gz");

        public static readonly string JMDictCache = Path.Combine(BaseDir, "dictionaries", "JMdict_e.cache");

        public static readonly string JMnedict = Path.Combine(BaseDir, "dictionaries", "JMnedict.xml.gz");

        public static readonly string JGram = Path.Combine(BaseDir, "dictionaries", "jgram");

        public static readonly string JGramLookup = Path.Combine(BaseDir, "dictionaries", "jgram_lookup");

        public static readonly string JGramCache = Path.Combine(BaseDir, "dictionaries", "jgram.cache");

        public static readonly string JMnedictCache = Path.Combine(BaseDir, "dictionaries", "JMnedict.xml.cache");

        public static readonly string IdiomsCache = Path.Combine(BaseDir, "dictionaries", "idioms.cache");

        public static readonly string KanjiDic = Path.Combine(BaseDir, "character", "kanjidic2.xml.gz");

        public static readonly string Kradfile = Path.Combine(BaseDir, "character", "kradfile1_plus_2_utf8");

        public static readonly string Radkfile = Path.Combine(BaseDir, "character", "radkfile1_plus_2_utf8");

        public static readonly string Tanaka = Path.Combine(BaseDir, "corpora", "examples.utf.gz");

        public static readonly string JESC = Path.Combine(BaseDir, "corpora", "jesc_raw");

        public static readonly string Hiragana = Path.Combine(BaseDir, "character", "hiragana_romaji.txt");

        public static readonly string Katakana = Path.Combine(BaseDir, @"character", "katakana_romaji.txt");

        public static readonly string HiraganaKatakana = Path.Combine(BaseDir, @"character", "hiragana_katakana.txt");

        public static readonly string KanaRelated = Path.Combine(BaseDir, @"character", "kana_related.txt");

        public static readonly string Kana = Path.Combine(BaseDir, @"character", "kana.txt");

        public static readonly string EasilyConfused = Path.Combine(BaseDir, @"character", "confused.txt");

        public static readonly string KanjiAliveRadicals = Path.Combine(BaseDir, "character", "japanese-radicals.csv");

        public static readonly string RadkfileKanjiAliveRadicalInfoCorrelationData = Path.Combine(BaseDir, "character", "radkfile_kanjilive_correlation_data.txt");
    }
}
