using System.IO;
using AutomatedTests;
using BenchmarkDotNet.Attributes;
using JDict;

namespace Benchmarking
{
    // these operations take time in seconds so lower precision doesn't bother me
    [SimpleJob(launchCount: 1, warmupCount: 1, targetCount: 2)]
    public class JMDictParsing
    {
        [IterationSetup]
        public void Setup()
        {
            File.Delete(TestDataPaths.JMDictCache);
        }

        [Benchmark]
        public JMDictLookup Creation()
        {
            var jmdict = JMDictLookup.Create(TestDataPaths.JMDict, TestDataPaths.JMDictCache);
            jmdict.Dispose();
            return jmdict;
        }
    }
}