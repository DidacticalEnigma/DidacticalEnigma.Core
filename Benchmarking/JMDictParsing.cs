using System.IO;
using AutomatedTests;
using BenchmarkDotNet.Attributes;
using JDict;

namespace Benchmarking
{
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