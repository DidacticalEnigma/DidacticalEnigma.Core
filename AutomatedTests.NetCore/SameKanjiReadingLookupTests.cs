using System.Linq;
using System.Text;
using DidacticalEnigma.Core.Models.LanguageService;
using JDict;
using NUnit.Framework;
using Optional.Unsafe;

namespace AutomatedTests;

[TestFixture]
public class SameKanjiReadingLookupTests
{
    private KanjiDict kanjiDict;
    private KanaProperties2 kana;
    private SameReadingKanjiLookup lookup;

    [SetUp]
    public void Setup()
    {
        this.kanjiDict = KanjiDict.Create(TestDataPaths.KanjiDic);
        this.kana = new KanaProperties2(TestDataPaths.Kana, Encoding.UTF8);
        this.lookup = new SameReadingKanjiLookup(this.kanjiDict, this.kana);
    }

    [Test]
    public void BasicTest()
    {
        var option = this.lookup.Lookup("船");
        Assert.IsTrue(option.HasValue);
        option.MatchSome(result =>
        {
            var entries = result.KanjiWithSameReading.First(x => x.Key == "セン").Value;
            var entry = entries.FirstOrDefault(e => e.Literal == "戦");
            Assert.NotNull(entry);
        });
    }
}