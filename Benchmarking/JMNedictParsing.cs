using System.IO;
using AutomatedTests;
using BenchmarkDotNet.Attributes;
using JDict;

namespace Benchmarking
{
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