using System.Collections.Generic;

namespace JDict
{
    public class JMDictReading
    {
        public string Reading { get; }

        public bool NotATrueReading { get; }
        
        public IEnumerable<string> ValidReadingFor { get; }
        
        public IEnumerable<EdictReadingInformation> ReadingInformation { get; }
        
        public IEnumerable<PriorityTag> PriorityInfo { get; }

        public JMDictReading(
            string reading,
            bool notATrueReading,
            IReadOnlyCollection<string> validReadingFor,
            IReadOnlyCollection<EdictReadingInformation> readingInformation,
            IReadOnlyCollection<PriorityTag> priorityInfo)
        {
            Reading = reading;
            NotATrueReading = notATrueReading;
            ValidReadingFor = validReadingFor;
            ReadingInformation = readingInformation;
            PriorityInfo = priorityInfo;
        }
    }
}