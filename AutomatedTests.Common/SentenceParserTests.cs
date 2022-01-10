using System.Linq;
using AutomatedTests;
using DidacticalEnigma.Core.Models;
using NUnit.Framework;

[TestFixture]
public class SentenceParserTests
{


    [Test]
    public void Test()
    {
        var parser = PartialWordLookupTests.Configure(TestDataPaths.BaseDir).Get<SentenceParser>();
        Assert.IsNotNull(parser.BreakIntoSentences("試着").SelectMany(x => x).First().Reading);
        /*{
            var word = parser.BreakIntoSentences("空いて").SelectMany(x => x).First();
            Assert.AreEqual("アイ", word.Reading);
            Assert.AreEqual("アク", word.DictionaryFormReading);
        }
        {
            var word = parser.BreakIntoSentences("しないで").SelectMany(x => x).First();
            Assert.AreEqual("シ", word.Reading);
            Assert.AreEqual("スル", word.DictionaryFormReading);
        }
        {
            var word = parser.BreakIntoSentences("来ないで").SelectMany(x => x).First();
            Assert.AreEqual("コ", word.Reading);
            Assert.AreEqual("クル", word.DictionaryFormReading);
        }
        {
            var word = parser.BreakIntoSentences("こないで").SelectMany(x => x).First();
            Assert.AreEqual("コ", word.Reading);
            Assert.AreEqual("クル", word.DictionaryFormReading);
        }
        {
            var word = parser.BreakIntoSentences("知った").SelectMany(x => x).First();
            Assert.AreEqual("シッタ", word.Reading);
            Assert.AreEqual("シル", word.DictionaryFormReading);
        }*/
    }
}