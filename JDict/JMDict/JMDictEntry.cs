using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JDict
{
    public class JMDictEntry
    {
        public long SequenceNumber { get; }

        public IEnumerable<JMDictReading> ReadingEntries { get; }

        public IEnumerable<JMDictKanji> KanjiEntries { get; }
        
        [Obsolete]
        public IEnumerable<string> Readings => ReadingEntries.Select(r => r.Reading);

        [Obsolete]
        public IEnumerable<string> Kanji => KanjiEntries.Select(k => k.Kanji);

        public IEnumerable<JMDictSense> Senses { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            bool first;
            {
                first = true;
                foreach (var kanji in Kanji)
                {
                    if (!first)
                        sb.Append(";  ");
                    first = false;
                    sb.Append(kanji);
                }
                sb.AppendLine();
            }
            {
                first = true;
                foreach (var reading in Readings)
                {
                    if (!first)
                        sb.AppendLine();
                    first = false;
                    sb.Append(reading);
                }
                sb.AppendLine();
            }
            {
                foreach (var sense in Senses)
                {
                    sb.Append(sense);
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }

        public JMDictEntry(
            long sequenceNumber,
            IReadOnlyCollection<JMDictReading> readingEntries,
            IReadOnlyCollection<JMDictKanji> kanjiEntries,
            IReadOnlyCollection<JMDictSense> senses)
        {
            SequenceNumber = sequenceNumber;
            ReadingEntries = readingEntries;
            KanjiEntries = kanjiEntries;
            Senses = senses;
        }
    }
}