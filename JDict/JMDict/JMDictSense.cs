using System.Collections.Generic;
using System.Linq;
using Optional;

namespace JDict
{
    public class JMDictSense
    {
        public Option<EdictPartOfSpeech> Type { get; }

        public IEnumerable<EdictPartOfSpeech> PartOfSpeechInfo { get; }

        public IEnumerable<string> Glosses { get; }

        public IEnumerable<string> Informational { get; }

        private string PartOfSpeechString => string.Join("/", PartOfSpeechInfo.Select(pos => EdictTypeUtils.ToDescription(pos)));

        private string Description => string.Join("/", Glosses);

        public IEnumerable<EdictDialect> DialectalInfo { get; }
        
        public IEnumerable<EdictField> FieldData { get; }
        
        public IEnumerable<EdictMisc> Misc { get; }

        public JMDictSense(
            Option<EdictPartOfSpeech> type,
            IReadOnlyCollection<EdictPartOfSpeech> pos,
            IReadOnlyCollection<EdictDialect> dialect,
            IReadOnlyCollection<string> text,
            IReadOnlyCollection<string> informational,
            IReadOnlyCollection<EdictField> field,
            IReadOnlyCollection<EdictMisc> misc)
        {
            Type = type;
            PartOfSpeechInfo = pos;
            DialectalInfo = dialect;
            Glosses = text;
            Informational = informational;
            FieldData = field;
            Misc = misc;
        }

        public override string ToString()
        {
            return PartOfSpeechString + "\n" + Description;
        }
    }
}