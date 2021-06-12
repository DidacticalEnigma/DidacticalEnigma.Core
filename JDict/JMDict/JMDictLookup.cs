using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using JDict.Xml;
using Optional;
using Optional.Collections;
using TinyIndex;

namespace JDict
{
    public class JMDictLookup : IDisposable
    {
        private static readonly Guid Version = new Guid("E42B739F-86DA-4303-87E2-9C1877769D0B");

        private TinyIndex.Database db;

        private IReadOnlyDiskArray<KeyValuePair<string, IReadOnlyList<long>>> kvps;

        private IReadOnlyDiskArray<JMDictEntry> entries;

        private IReadOnlyDiskArray<KeyValuePair<string, string>> friendlyNames;

        private JMDictLookup Init(Stream stream, string cache)
        {
            var entrySerializer = TinyIndex.Serializer.ForComposite()
                .With(Serializer.ForLong())
                .With(Serializer.ForReadOnlyCollection(Serializer.ForStringAsUTF8()))
                .With(Serializer.ForReadOnlyCollection(Serializer.ForStringAsUTF8()))
                .With(Serializer.ForReadOnlyCollection(Serializer.ForComposite()
                    .With(SerializerExt.ForOption(Serializer.ForEnum<EdictPartOfSpeech>()))
                    .With(Serializer.ForReadOnlyCollection(Serializer.ForEnum<EdictPartOfSpeech>()))
                    .With(Serializer.ForReadOnlyCollection(Serializer.ForEnum<EdictDialect>()))
                    .With(Serializer.ForReadOnlyCollection(Serializer.ForStringAsUTF8()))
                    .With(Serializer.ForReadOnlyCollection(Serializer.ForStringAsUTF8()))
                    .With(Serializer.ForReadOnlyCollection(Serializer.ForEnum<EdictField>()))
                    .With(Serializer.ForReadOnlyCollection(Serializer.ForEnum<EdictMisc>()))
                    .Create()
                    .Mapping(
                        raw => new JMDictSense(
                            (Option<EdictPartOfSpeech>)raw[0],
                            (IReadOnlyCollection<EdictPartOfSpeech>)raw[1],
                            (IReadOnlyCollection<EdictDialect>)raw[2],
                            (IReadOnlyCollection<string>)raw[3],
                            (IReadOnlyCollection<string>)raw[4],
                            (IReadOnlyCollection<EdictField>)raw[5],
                            (IReadOnlyCollection<EdictMisc>)raw[6]),
                        obj => new object[]
                        {
                            obj.Type,
                            obj.PartOfSpeechInfo,
                            obj.DialectalInfo,
                            obj.Glosses,
                            obj.Informational,
                            obj.FieldData,
                            obj.Misc
                        })))
                .Create()
                .Mapping(
                    raw => new JMDictEntry(
                        (long)raw[0],
                        (IReadOnlyCollection<string>)raw[1],
                        (IReadOnlyCollection<string>)raw[2],
                        (IReadOnlyCollection<JMDictSense>)raw[3]),
                    obj => new object[]
                    {
                        obj.SequenceNumber,
                        obj.Readings,
                        obj.Kanji,
                        obj.Senses
                    });

            using (var jmdictParser = JMDictParser.Create(stream))
            {
                db = TinyIndex.Database.CreateOrOpen(cache, Version)
                    .AddIndirectArray(entrySerializer, db => jmdictParser.ReadRemainingToEnd(),
                        x => x.SequenceNumber)
                    .AddIndirectArray(
                        TinyIndex.Serializer.ForKeyValuePair(TinyIndex.Serializer.ForStringAsUTF8(),
                            TinyIndex.Serializer.ForReadOnlyList(TinyIndex.Serializer.ForLong())), db =>
                        {
                            IEnumerable<KeyValuePair<long, string>> It(IEnumerable<JMDictEntry> entries)
                            {
                                foreach (var e in entries)
                                {
                                    foreach (var r in e.Kanji)
                                    {
                                        yield return new KeyValuePair<long, string>(e.SequenceNumber, r);
                                    }

                                    foreach (var r in e.Readings)
                                    {
                                        yield return new KeyValuePair<long, string>(e.SequenceNumber, r);
                                    }
                                }
                            }

                            return It(db.Get<JMDictEntry>(0)
                                    .LinearScan())
                                .GroupBy(kvp => kvp.Value, kvp => kvp.Key)
                                .Select(x => new KeyValuePair<string, IReadOnlyList<long>>(x.Key, x.ToList()));
                        },
                        x => x.Key, StringComparer.Ordinal)
                    .AddIndirectArray(
                        Serializer.ForKeyValuePair(Serializer.ForStringAsUTF8(), Serializer.ForStringAsUTF8()),
                        db => jmdictParser.FriendlyNames,
                        x => x.Key, StringComparer.Ordinal)
                    .Build();
                entries = db.Get<JMDictEntry>(0, new LruCache<long, JMDictEntry>(128));
                kvps = db.Get<KeyValuePair<string, IReadOnlyList<long>>>(1,
                    new LruCache<long, KeyValuePair<string, IReadOnlyList<long>>>(128));
                friendlyNames = db.Get(2, new LruCache<long, KeyValuePair<string, string>>(256));
            }

            return this;
        }

        private async Task<JMDictLookup> InitAsync(Stream stream, string cache)
        {
            // TODO: not a lazy way
            await Task.Run(() => Init(stream, cache));
            return this;
        }

        public Option<JMDictEntry> LookupBySequenceNumber(long sequenceNumber)
        {
            var searchResult = entries.BinarySearch(sequenceNumber, e => e.SequenceNumber);
            if (searchResult.id != -1)
            {
                return searchResult.element.Some();
            }

            return Option.None<JMDictEntry>();
        }

        public IEnumerable<JMDictEntry> AllEntries()
        {
            return entries.LinearScan();
        }

        public IEnumerable<JMDictEntry> Lookup(string key)
        {
            var res = kvps.BinarySearch(key, kvp => kvp.Key, StringComparer.Ordinal);
            if (res.id == -1)
            {
                return null;
            }
            else
            {
                return It();
            }

            IEnumerable<JMDictEntry> It()
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

        private async Task<JMDictLookup> InitAsync(string path, string cache)
        {
            using (var file = File.OpenRead(path))
            using (var gzip = new GZipStream(file, CompressionMode.Decompress))
            {
                return await InitAsync(gzip, cache);
            }
        }

        private JMDictLookup Init(string path, string cache)
        {
            using (var file = File.OpenRead(path))
            using (var gzip = new GZipStream(file, CompressionMode.Decompress))
            {
                return Init(gzip, cache);
            }
        }

        private JMDictLookup()
        {

        }

        public static JMDictLookup Create(string path, string cache)
        {
            return new JMDictLookup().Init(
                path,
                cache);
        }

        public static JMDictLookup Create(Stream stream, string cache)
        {
            return new JMDictLookup().Init(
                stream,
                cache);
        }

        public static async Task<JMDictLookup> CreateAsync(string path, string cache)
        {
            return await new JMDictLookup().InitAsync(
                path,
                cache);
        }

        public static async Task<JMDictLookup> CreateAsync(Stream stream, string cache)
        {
            return await new JMDictLookup().InitAsync(
                stream,
                cache);
        }

        public void Dispose()
        {
            db.Dispose();
        }

        private string FriendlyDescriptionOf(string abbr)
        {
            var res = this.friendlyNames.BinarySearch(
                abbr,
                kvp => kvp.Key,
                StringComparer.Ordinal);
            if (res.id == -1)
            {
                throw new InvalidDataException();
            }
            else
            {
                return res.element.Value;
            }
        }

        public string FriendlyDescriptionOf(EdictPartOfSpeech pos)
        {
            return FriendlyDescriptionOf(pos.ToAbbrevation());
        }
        
        public string FriendlyDescriptionOf(EdictDialect pos)
        {
            return FriendlyDescriptionOf(pos.ToAbbrevation());
        }
        
        public string FriendlyDescriptionOf(EdictField pos)
        {
            return FriendlyDescriptionOf(pos.ToAbbrevation());
        }
        
        public string FriendlyDescriptionOf(EdictMisc pos)
        {
            return FriendlyDescriptionOf(pos.ToAbbrevation());
        }
        
    }
}