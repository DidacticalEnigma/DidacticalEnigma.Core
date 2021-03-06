﻿using System.Text;
using DidacticalEnigma.Core.Models.LanguageService;
using NUnit.Framework;

namespace AutomatedTests
{
    [TestFixture]
    class KanaPropertiesTests
    {
        public static readonly TestCaseData[] HiraganaConversion =
        {
            new TestCaseData("ドキドキ", "どきどき"),
            new TestCaseData("キョウ", "きょう"),
        };

        public static IKanaProperties kana;

        [OneTimeSetUp]
        public void SetUp()
        {
            kana = new KanaProperties2(TestDataPaths.Kana, Encoding.UTF8);
        }

        [TestCaseSource(nameof(HiraganaConversion))]
        public void ToHiragana(string input, string expected)
        {
            Assert.AreEqual(expected, kana.ToHiragana(input));
        }
    }
}
