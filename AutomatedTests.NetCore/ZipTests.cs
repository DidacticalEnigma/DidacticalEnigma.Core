using System.IO;
using System.Linq;
using NUnit.Framework;
using Utility.Utils;

namespace AutomatedTests
{
    [TestFixture]
    class ZipTests
    {
        [DependentOnKenkyuusha]
        [Test]
        public void TestKenkyuushaZipFile()
        {
            using (var zip = new ZipFile(TestDataPaths.Kenkyusha5))
            {
                var l = zip.Files.ToList();
                using (var file = zip.OpenFile(l.First(e => e == "index.json")))
                using (var reader = new StreamReader(file))
                {
                    _ = reader.ReadToEnd();
                }
            }
        }
        
        [DependentOnKenkyuusha]
        [Test]
        public void TestKenkyuushaZipFile2()
        {
            using (var zip = new ZipFile2(TestDataPaths.Kenkyusha5))
            {
                var l = zip.Files.ToList();
                using (var file = zip.OpenFile(l.First(e => e == "index.json")))
                using (var reader = new StreamReader(file))
                {
                    _ = reader.ReadToEnd();
                }
            }
        }
        
        [Test]
        public void TestZipFile2()
        {
            using (var zip = new ZipFile2(Path.Combine(TestDataPaths.TestDir, "ziptest.zip")))
            {
                CollectionAssert.AreEquivalent(new[]
                {
                    "A",
                    "C/A"
                }, zip.Files);
            }
        }
    }
}
