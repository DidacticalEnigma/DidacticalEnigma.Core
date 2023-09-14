using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using DidacticalEnigma.Core.Models.DataSources;
using DidacticalEnigma.Core.Models.Formatting;
using DidacticalEnigma.Core.Models.LanguageService;
using Gu.Inject;
using JDict;
using JDict.Xml;
using NUnit.Framework;
using Optional;

namespace AutomatedTests
{
    [TestFixture]
    class SandBox
    {
        private static JMDictLookup jmdict;
        private static Kernel kernel;
        private static IKanaProperties kanaProperties;

        [OneTimeSetUp]
        public void SetUp()
        {
            kernel = PartialWordLookupTests.Configure(TestDataPaths.BaseDir);
            jmdict = kernel.Get<JMDictLookup>();
            kanaProperties = kernel.Get<IKanaProperties>();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            kernel.Dispose();
        }

        [Explicit]
        [Test]
        public async Task Words()
        {
            var entries = jmdict.AllEntries()
                .Where(e =>
                {
                    bool any = e.Senses.Any(s =>
                    {
                        bool isVerb = s.PartOfSpeechInfo.Any(t => t < EdictPartOfSpeech.v_unspec);
                        
                        bool isIntransitive = s.PartOfSpeechInfo.Any(t => t == EdictPartOfSpeech.vi);
                        bool isTransitive = s.PartOfSpeechInfo.Any(t => t == EdictPartOfSpeech.vt);
                        
                        bool isSuruVerb = s.PartOfSpeechInfo.Any(t => t == EdictPartOfSpeech.vs) ||
                                          s.PartOfSpeechInfo.Any(t => t == EdictPartOfSpeech.vs_s) ||
                                          s.PartOfSpeechInfo.Any(t => t == EdictPartOfSpeech.vs_i);
                        bool isExpression = s.PartOfSpeechInfo.Any(t => t == EdictPartOfSpeech.exp);

                        return isVerb && !isIntransitive && !isTransitive && !isSuruVerb && !isExpression;
                    });

                    return any;
                });

            foreach (var entry in entries)
            {
                Console.WriteLine("K: {0}", string.Join("; ", entry.KanjiEntries.Select(x => x.Kanji)));
                Console.WriteLine("R: {0}", string.Join("; ", entry.ReadingEntries.Select(x => x.Reading)));
            }
        }
        
        [Explicit]
        [Test]
        public async Task Sandbox()
        {
            var unidicEntry = new UnidicEntry("切", "形状詞,一般,*,*,*,*,セツ,切,切,セツ,切,セツ,漢,*,*,*,*,*,*,相,セツ,セツ,セツ,セツ,1,C3,*,19579287057342976,71229".Some());

            var xmlRenderer = new XmlRichFormattingRenderer();
            var jmdictSource = new JMDictCompactDataSource(jmdict, kanaProperties);
            var request = new Request(
                "空",
                new WordInfo(
                    "空い",
                    PartOfSpeech.Verb,
                    "空く",
                    EdictPartOfSpeech.v5k.Some(),
                    "あい",
                    "あく"),
                "空い",
                () => null,
                null);
            var answer = await jmdictSource.Answer(request, CancellationToken.None);
            answer.MatchSome(rich =>
            {
                Console.WriteLine(xmlRenderer.Render(rich).OuterXml);
            });
        }
    }
}
