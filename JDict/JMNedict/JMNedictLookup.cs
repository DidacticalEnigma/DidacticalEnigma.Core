using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using JDict.Xml;
using Optional.Collections;
using TinyIndex;

// ReSharper disable InconsistentNaming

namespace JDict
{
    public class JMNedictLookup : IDisposable
    {
        private static readonly XmlSerializer serializer = new XmlSerializer(typeof(NeEntry));

        private static readonly Guid Version = new Guid("B3DC1BEF-3342-4CD0-A87F-763D42FCCEBF");

        private Database database;
        private IReadOnlyDiskArray<JnedictEntry> entries;
        private IReadOnlyDiskArray<KeyValuePair<string, IReadOnlyList<long>>> kvps;

        public void Dispose()
        {
            database.Dispose();
        }

        private JMNedictLookup Init(Stream stream, string cache)
        {
            var entrySerializer = Serializer.ForComposite()
                .With(Serializer.ForLong())
                .With(Serializer.ForReadOnlyList(Serializer.ForStringAsUTF8()))
                .With(Serializer.ForReadOnlyList(Serializer.ForStringAsUTF8()))
                .With(Serializer.ForReadOnlyList(Serializer.ForComposite()
                    .With(Serializer.ForReadOnlyList(Serializer.ForEnum<JMNedictType>()))
                    .With(Serializer.ForReadOnlyList(Serializer.ForStringAsUTF8()))
                    .Create()
                    .Mapping(
                        raw => new JnedictTranslation(
                            (IEnumerable<JMNedictType>)raw[0],
                            (IEnumerable<string>)raw[1]),
                        obj => new object[]
                            {
                                obj.Type,
                                obj.Translation
                            })))
                .Create()
                .Mapping(
                    raw => new JnedictEntry(
                        (long)raw[0],
                        (IEnumerable<string>)raw[1],
                        (IEnumerable<string>)raw[2],
                        (IEnumerable<JnedictTranslation>)raw[3]),
                    obj => new object[]
                    {
                        obj.SequenceNumber,
                        obj.Kanji,
                        obj.Reading,
                        obj.Translation
                    });

            using (var parser = JMNedictParser.Create(stream))
            {
                database = Database.CreateOrOpen(cache, Version)
                    .AddIndirectArray(
                        entrySerializer,
                        db => parser.ReadRemainingToEnd())
                    .AddIndirectArray(
                        Serializer.ForKeyValuePair(Serializer.ForStringAsUTF8(),
                            Serializer.ForReadOnlyList(Serializer.ForLong())), db =>
                        {
                            IEnumerable<KeyValuePair<long, string>> It(IEnumerable<JnedictEntry> entries)
                            {
                                foreach (var e in entries)
                                {
                                    foreach (var r in e.Reading)
                                    {
                                        yield return new KeyValuePair<long, string>(e.SequenceNumber, r);
                                    }

                                    foreach (var k in e.Kanji)
                                    {
                                        yield return new KeyValuePair<long, string>(e.SequenceNumber, k);
                                    }
                                }
                            }

                            return It(db.Get<JnedictEntry>(0).LinearScan())
                                .GroupBy(kvp => kvp.Value, kvp => kvp.Key)
                                .Select(x => new KeyValuePair<string, IReadOnlyList<long>>(x.Key, x.ToList()));
                        },
                        x => x.Key, StringComparer.Ordinal)
                    .Build();
            }

            entries = database.Get<JnedictEntry>(0, new LruCache<long, JnedictEntry>(64));
            kvps = database.Get<KeyValuePair<string, IReadOnlyList<long>>>(1, new LruCache<long, KeyValuePair<string, IReadOnlyList<long>>>(64));

            return this;
        }

        public IEnumerable<JnedictEntry> Lookup(string key)
        {
            var res = kvps.BinarySearch(key, kvp => kvp.Key, StringComparer.Ordinal);
            if (res.id == -1)
            {
                return null;
            }

            return It();

            IEnumerable<JnedictEntry> It()
            {
                var sequenceNumbers = res.element.Value;
                foreach (var sequenceNumber in sequenceNumbers)
                {
                    var searchResult = entries.BinarySearch(sequenceNumber, e => e.SequenceNumber);
                    if (searchResult.id != -1)
                    {
                        yield return searchResult.element;
                    }
                }
            }
        }

        private async Task<JMNedictLookup> InitAsync(Stream stream, string cache)
        {
            // TODO: not a lazy way
            await Task.Run(() => Init(stream, cache));
            return this;
        }

        private async Task<JMNedictLookup> InitAsync(string path, string cache)
        {
            using (var file = File.OpenRead(path))
            using (var gzip = new GZipStream(file, CompressionMode.Decompress))
            {
                return await InitAsync(gzip, cache);
            }
        }

        private JMNedictLookup Init(string path, string cache)
        {
            using (var file = File.OpenRead(path))
            using (var gzip = new GZipStream(file, CompressionMode.Decompress))
            {
                return Init(gzip, cache);
            }
        }

        private JMNedictLookup()
        {

        }

        public static JMNedictLookup Create(string path, string cache)
        {
            return new JMNedictLookup().Init(
                path,
                cache);
        }

        public static JMNedictLookup Create(Stream stream, string cache)
        {
            return new JMNedictLookup().Init(
                stream,
                cache);
        }

        public static async Task<JMNedictLookup> CreateAsync(string path, string cache)
        {
            return await new JMNedictLookup().InitAsync(
                path,
                cache);
        }

        public static async Task<JMNedictLookup> CreateAsync(Stream stream, string cache)
        {
            return await new JMNedictLookup().InitAsync(
                stream,
                cache);
        }
    }
}