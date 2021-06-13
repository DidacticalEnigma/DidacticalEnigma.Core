using AutomatedTests;
using BenchmarkDotNet.Attributes;
using JDict;

namespace Benchmarking
{
    // these operations take time in seconds so lower precision doesn't bother me
    [SimpleJob(launchCount: 2, warmupCount: 3, targetCount: 2)]
    public class KanjiDicParsing
    {
        [Benchmark]
        public KanjiDict Creation()
        {
            var kanjiDict = KanjiDict.Create(TestDataPaths.KanjiDic);
            return kanjiDict;
        }
    }
}