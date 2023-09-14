using System;
using System.Collections;
using JDict.Xml;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Optional;
using Optional.Collections;
using Utility.Utils;

namespace JDict
{
    public class KanjiDict
    {
        private static readonly XmlSerializer serializer = new XmlSerializer(typeof(KanjiDictRoot));

        private Dictionary<string, KanjiEntry> root;

        private Dictionary<int, KanjiEntry> codePointLookup;

        private List<KanjiEntry> allEntries;

        private KanjiDict Init(Stream stream)
        {
            var xmlSettings = new XmlReaderSettings
            {
                CloseInput = false,
                DtdProcessing = DtdProcessing.Parse,
                XmlResolver = null, // we don't want to resolve against external entities
                MaxCharactersFromEntities = 0,
                MaxCharactersInDocument = 32 * 1024 * 1024 / 2
            };
            using (var xmlReader = XmlReader.Create(stream, xmlSettings))
            {
                root = new Dictionary<string, KanjiEntry>();
                codePointLookup = new Dictionary<int, KanjiEntry>();
                allEntries = new List<KanjiEntry>();
                var xmlEntries = ((KanjiDictRoot)serializer.Deserialize(xmlReader)).Characters;
                foreach (var xmlEntry in xmlEntries)
                {
                    var entry = new KanjiEntry(xmlEntry);
                    codePointLookup.Add(entry.CodePoint, entry);
                    root.Add(entry.Literal, entry);
                    allEntries.Add(entry);
                }
            }
            return this;
        }

        public Option<KanjiEntry> LookupCodePoint(int codePoint)
        {
            return codePointLookup.GetValueOrNone(codePoint);
        }

        public Option<KanjiEntry> Lookup(string v)
        {
            return root.GetValueOrNone(v);
        }

        // nanori readings are ignored by this function
        public IEnumerable<KanjiEntry> LookupByReading(IReadOnlySet<string> readings)
        {
            return allEntries.Where(entry =>
            {
                return entry.KunReadings.Concat(entry.OnReadings).Any(reading => readings.Contains(reading));
            });
        }

        private KanjiDict Init(string path)
        {
            using (var file = File.OpenRead(path))
            using (var gzip = new GZipStream(file, CompressionMode.Decompress))
            {
                return Init(gzip);
            }
        }

        private KanjiDict()
        {

        }

        public static KanjiDict Create(string path)
        {
            return new KanjiDict().Init(path);
        }

        public static KanjiDict Create(Stream stream)
        {
            return new KanjiDict().Init(stream);
        }
    }

    public class KanjiEntry
    {
        public string Literal { get; }

        public int CodePoint { get; }

        public int StrokeCount { get; }

        public int FrequencyRating { get; }

        public IEnumerable<string> KunReadings { get; }

        public IEnumerable<string> OnReadings { get; }

        public IEnumerable<string> NanoriReadings { get; }

        public IEnumerable<string> Meanings { get; }

        internal KanjiEntry(KanjiCharacter ch)
        {
            Literal = ch.Literal;
            CodePoint = Convert.ToInt32(ch.CodePoints.CodePoints.First(c => c.Type == "ucs").Value, 16);
            StrokeCount = ch.Misc.StrokeCount[0];
            FrequencyRating = ch.Misc.FrequencyRating != 0
                ? ch.Misc.FrequencyRating
                : 100000;

            KunReadings = GetReadings(ch, "ja_kun");
            OnReadings = GetReadings(ch, "ja_on");
            Meanings = GetMeanings(ch, "en");
            NanoriReadings = GetNanori(ch);

            IReadOnlyCollection<string> GetNanori(KanjiCharacter c)
            {
                return (c.ReadingsAndMeanings?.Nanori ?? Enumerable.Empty<KanjiNanori>())
                    .Select(n => n.Value)
                    .ToList();
            }

            IReadOnlyCollection<string> GetMeanings(KanjiCharacter c, string lang)
            {
                return c.ReadingsAndMeanings?.Groups
                    .FirstOrNone()
                    .FlatMap(x => (x.Meanings?.Where(m => (m.Language ?? "en") == lang)).SomeNotNull())
                    .ValueOr(Enumerable.Empty<KanjiMeaning>())
                    .Select(reading => reading.Value)
                    .Materialize()
                    ?? Array.Empty<string>();
            }

            IReadOnlyCollection<string> GetReadings(KanjiCharacter c, string type)
            {
                return c.ReadingsAndMeanings?.Groups
                    .FirstOrNone()
                    .FlatMap(x => (x.Readings?.Where(r => r.ReadingType == type)).SomeNotNull())
                    .ValueOr(Enumerable.Empty<KanjiReading>())
                    .Select(reading => reading.Value)
                    .Materialize()
                    ?? Array.Empty<string>();
            }
        }
    }
}