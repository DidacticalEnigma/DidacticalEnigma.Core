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
        public async Task Sandbox()
        {
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
