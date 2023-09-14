using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using NUnit.Framework;
using JDict;
using Optional;

namespace AutomatedTests
{
    [TestFixture]
    class JMDictTests
    {
        private static JMDictLookup jmdict;

        [OneTimeSetUp]
        public void SetUp()
        {
            jmdict = JDict.JMDictLookup.Create(TestDataPaths.JMDict, TestDataPaths.JMDictCache);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            jmdict.Dispose();
        }

        [Test]
        public void LookupKana()
        {
            {
                var entries = jmdict.Lookup("みなみ");
                Assert.True(entries.Any(e => e.Senses.Any(s => s.Glosses.Contains("south"))));
                Assert.True(entries.Any(e => e.Senses.Any(s => s.PartOfSpeechInfo.Contains(EdictPartOfSpeech.n))));
            }
        }

        [Test]
        public void LookupKanji()
        {
            {
                var entries = jmdict.Lookup("南");
                Assert.True(entries.Any(e => e.Senses.Any(s => s.Glosses.Contains("south"))));
            }
        }

        [Test]
        public void LookupOther()
        {
            {
                var entries = jmdict.Lookup("私");
                Assert.False(entries.Any(e => e.Senses.Any(s => s.Glosses.Contains("south"))));
            }
            {
                var entries = jmdict.Lookup("洞穴");
                Assert.True(entries.Any(e => e.Senses.Any(s => s.PartOfSpeechInfo.Contains(EdictPartOfSpeech.n))));
            }
        }
        
        [Test]
        public void FriendlyNameMapping()
        {
            using (var file = File.OpenRead(TestDataPaths.JMDict))
            using (var gzip = new GZipStream(file, CompressionMode.Decompress))
            {
                var jparser = JMDictParser.Create(gzip);
                CollectionAssert.IsNotEmpty(jparser.FriendlyNames);
            }
            
            Assert.AreEqual("noun (common) (futsuumeishi)",jmdict.FriendlyDescriptionOf(EdictPartOfSpeech.n));
        }

        [Test]
        public void IsIndependentFromLocale()
        {
            var previousCulture = CultureInfo.CurrentCulture;
            try
            {
                foreach (var culture in CultureInfo.GetCultures(CultureTypes.AllCultures).Concat(new []{previousCulture}))
                {
                    CultureInfo.CurrentCulture = culture;
                    var entries = jmdict.Lookup("セーラー服");
                    Assert.True(entries.Any(e => e.Senses.Any(s => s.Glosses.Contains("sailor suit"))));
                }
            }
            finally
            {
                CultureInfo.CurrentCulture = previousCulture;
            }
        }

        [Test]
        public void LookupKangaeru()
        {
            {
                var entries = jmdict.Lookup("考える");
                Assert.True(entries.Any(e => e.Senses.Any(s => s.Type == EdictPartOfSpeech.v1.Some())));
            }
        }
    }
}
