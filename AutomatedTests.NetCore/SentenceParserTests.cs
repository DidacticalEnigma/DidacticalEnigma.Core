using System.Linq;
using AutomatedTests;
using DidacticalEnigma.Core.Models;
using DidacticalEnigma.Core.Models.LanguageService;
using Gu.Inject;
using NUnit.Framework;

[TestFixture]
public class SentenceParserTests
{
    private Kernel kernel;
    private SentenceParser parser;

    [OneTimeSetUp]
    public void SetUp()
    {
        this.kernel = PartialWordLookupTests.Configure(TestDataPaths.BaseDir);
        this.parser = this.kernel.Get<SentenceParser>();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        this.kernel.Dispose();
    }
    
    [Test]
    [TestCase("\nお前は\nもう\n死んでいる", new object[]
    {
        new object[0],
        new object[] { "お前", "は" },
        new object[] { "もう" },
        new object[] { "死ん", "で", "いる" },
    })]
    [TestCase("お前は\nもう\n死んでいる\n", new object[]
    {
        new object[] { "お前", "は" },
        new object[] { "もう" },
        new object[] { "死ん", "で", "いる" },
    })]
    public void MultilineTest(string input, object[] expecteds)
    {
        CollectionAssert.AreEqual(expecteds, this.parser.BreakIntoSentences(input).Select(l => l.Select(w => w.RawWord)));
    }

    [Test]
    public void Test()
    {
        var parser = PartialWordLookupTests.Configure(TestDataPaths.BaseDir).Get<SentenceParser>();
        Assert.IsNotNull(parser.BreakIntoWords("試着").First().Reading);
        /*{
            var word = parser.BreakIntoWords("空いて").First();
            Assert.AreEqual("アイ", word.Reading);
            Assert.AreEqual("アク", word.DictionaryFormReading);
        }
        {
            var word = parser.BreakIntoWords("しないで").First();
            Assert.AreEqual("シ", word.Reading);
            Assert.AreEqual("スル", word.DictionaryFormReading);
        }
        {
            var word = parser.BreakIntoWords("来ないで").First();
            Assert.AreEqual("コ", word.Reading);
            Assert.AreEqual("クル", word.DictionaryFormReading);
        }
        {
            var word = parser.BreakIntoWords("こないで").First();
            Assert.AreEqual("コ", word.Reading);
            Assert.AreEqual("クル", word.DictionaryFormReading);
        }
        {
            var word = parser.BreakIntoWords("知った").First();
            Assert.AreEqual("シッタ", word.Reading);
            Assert.AreEqual("シル", word.DictionaryFormReading);
        }*/
    }
}