using System.IO;
using AutomatedTests;
using BenchmarkDotNet.Attributes;
using DidacticalEnigma.Core.Models.LanguageService;
using Gu.Inject;
using JDict;

namespace Benchmarking
{
    public class PartialWordLookupBenchmarks
    {
        private JMDictLookup jmdict;
        private KanjiRadicalLookup kanjiRadicalLookup;
        private IRadicalSearcher radicalSearcher;
        private KanjiDict kanjiDict;
        private RadkfileKanjiAliveCorrelator radkfileKanjiAliveCorrelator;

        [IterationSetup]
        public void Setup()
        {
            this.kanjiDict = KanjiDict.Create(Path.Combine(TestDataPaths.BaseDir, "character", "kanjidic2.xml.gz"));
            using(var reader = File.OpenText(Path.Combine(TestDataPaths.BaseDir, "character", "radkfile1_plus_2_utf8")))
                this.kanjiRadicalLookup = new KanjiRadicalLookup(Radkfile.Parse(reader), kanjiDict);
            this.jmdict = JMDictLookup.Create(TestDataPaths.JMDict, TestDataPaths.JMDictCache);
            this.radkfileKanjiAliveCorrelator =
                new RadkfileKanjiAliveCorrelator(Path.Combine(TestDataPaths.BaseDir, "character",
                    "radkfile_kanjilive_correlation_data.txt"));
            this.radicalSearcher = new RadicalSearcher(
                kanjiRadicalLookup.AllRadicals,
                KanjiAliveJapaneseRadicalInformation.Parse(Path.Combine(TestDataPaths.BaseDir, "character", "japanese-radicals.csv")),
                radkfileKanjiAliveCorrelator);
        }

        [Benchmark]
        public PartialWordLookup Creation()
        {
            var partialWordLookup = new PartialWordLookup(jmdict, radicalSearcher, kanjiRadicalLookup);
            return partialWordLookup;
        }

        [IterationCleanup]
        public void Cleanup()
        {
            this.jmdict.Dispose();
        }
    }
}