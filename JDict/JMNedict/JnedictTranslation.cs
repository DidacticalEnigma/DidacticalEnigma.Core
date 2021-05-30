using System.Collections.Generic;
using System.Linq;

namespace JDict
{
    public class JnedictTranslation
    {
        public IEnumerable<JMNedictType> Type { get; }

        public IEnumerable<string> Translation { get; }

        public JnedictTranslation(
            IEnumerable<JMNedictType> type,
            IEnumerable<string> translation)
        {
            Type = type.ToList();
            Translation = translation.ToList();
        }
    }
}