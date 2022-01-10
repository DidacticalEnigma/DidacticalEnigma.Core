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