using System.IO;
using AutomatedTests;
using BenchmarkDotNet.Attributes;
using JDict;

namespace Benchmarking
{
    // these operations take time in seconds so lower precision doesn't bother me
    [SimpleJob(launchCount: 1, warmupCount: 1, targetCount: 2)]
    public class JMNedictParsing
    {
        [IterationSetup]
        public void Setup()
        {
            File.Delete(TestDataPaths.JMnedictCache);
        }

        [Benchmark]
        public JMNedictLookup Creation()
        {
            var jmdict = JMNedictLookup.Create(TestDataPaths.JMnedict, TestDataPaths.JMnedictCache);
            jmdict.Dispose();
            return jmdict;
        }
    }
}