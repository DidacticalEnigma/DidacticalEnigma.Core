using System.Collections.Generic;

namespace JDict
{
    public class JMDictKanji
    {
        public string Kanji { get; }

        public IEnumerable<EdictKanjiInformation> Informational { get; }
        
        public IEnumerable<PriorityTag> PriorityInfo { get; }

        public JMDictKanji(
            string kanji,
            IReadOnlyCollection<EdictKanjiInformation> informational,
            IReadOnlyCollection<PriorityTag> priorityInfo)
        {
            Kanji = kanji;
            Informational = informational;
            PriorityInfo = priorityInfo;
        }
    }
}