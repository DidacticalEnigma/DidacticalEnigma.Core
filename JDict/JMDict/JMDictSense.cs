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
        
        public IEnumerable<string> RestrictedToKanji { get; }
        
        public IEnumerable<string> RestrictedToReading { get; }
        
        public IEnumerable<EdictLoanSource> LoanSources { get; }
        
        public IEnumerable<EdictCrossReference> CrossReferences { get; }
        
        public IEnumerable<EdictCrossReference> Antonyms { get; }

        public JMDictSense(
            Option<EdictPartOfSpeech> type,
            IReadOnlyCollection<EdictPartOfSpeech> pos,
            IReadOnlyCollection<EdictDialect> dialect,
            IReadOnlyCollection<string> text,
            IReadOnlyCollection<string> informational,
            IReadOnlyCollection<EdictField> field,
            IReadOnlyCollection<EdictMisc> misc,
            IReadOnlyCollection<string> stagkList,
            IReadOnlyCollection<string> stagrList,
            IReadOnlyCollection<EdictLoanSource> lsourceList,
            IReadOnlyCollection<EdictCrossReference> xrefList,
            IReadOnlyCollection<EdictCrossReference> antList)
        {
            Type = type;
            PartOfSpeechInfo = pos;
            DialectalInfo = dialect;
            Glosses = text;
            Informational = informational;
            FieldData = field;
            Misc = misc;
            RestrictedToKanji = stagkList;
            RestrictedToReading = stagrList;
            LoanSources = lsourceList;
            CrossReferences = xrefList;
            Antonyms = antList;
        }

        public override string ToString()
        {
            return PartOfSpeechString + "\n" + Description;
        }
    }
}