using System.Linq;
using JDict;
using NUnit.Framework;

namespace AutomatedTests
{
    [TestFixture]
    class JnedictTests
    {
        private static JMNedictLookup jmNedictLookup;

        [OneTimeSetUp]
        public void SetUp()
        {
            jmNedictLookup = JMNedictLookup.Create(TestDataPaths.JMnedict, TestDataPaths.JMnedictCache);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            jmNedictLookup.Dispose();
        }

        [Test]
        public void LookupBasic()
        {
            var entries = jmNedictLookup.Lookup("南") ?? Enumerable.Empty<JnedictEntry>();
            Assert.True(entries.Any(e =>
                e.Reading.Contains("みなみ") &&
                e.Translation.Any(t => t.Type.Contains(JMNedictType.fem) && t.Translation.Contains("Minami"))));
        }
    }
}
