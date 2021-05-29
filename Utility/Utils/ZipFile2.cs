using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Utility.Utils
{
    public class ZipFile2 : IZipFile
    {
        private Stream stream;

        private ZipArchive archive;

        private Dictionary<string, ZipArchiveEntry> entries;
        
        public ZipFile2(string path)
        {
            stream = File.OpenRead(path);
            archive = new ZipArchive(stream, ZipArchiveMode.Read);
            entries = archive.Entries
                .Where(entry => !entry.FullName.EndsWith("/"))
                .ToDictionary(entry => entry.FullName, entry => entry);
        }
        
        public void Dispose()
        {
            archive.Dispose();
            stream.Dispose();
        }

        public IEnumerable<string> Files => entries.Keys;
        public Stream OpenFile(string path)
        {
            if (entries.TryGetValue(path, out ZipArchiveEntry entry))
            {
                return entry.Open();
            }
            else
            {
                throw new FileNotFoundException();
            }
        }
    }
}