using System.Collections.Generic;
using System.Linq;

namespace JDict
{
    public class JnedictEntry
    {
        public long SequenceNumber { get; }

        public IEnumerable<string> Kanji { get; }

        public IEnumerable<string> Reading { get; }

        public IEnumerable<JnedictTranslation> Translation { get; }

        public JnedictEntry(
            long sequenceNumber,
            IEnumerable<string> kanji,
            IEnumerable<string> reading,
            IEnumerable<JnedictTranslation> translation)
        {
            SequenceNumber = sequenceNumber;
            Kanji = kanji.ToList();
            Reading = reading.ToList();
            Translation = translation.ToList();
        }
    }
}