using Optional;

namespace JDict
{
    public class EdictLoanSource
    {
        public Option<string> LoanWord { get; }
        
        public string SourceLanguage { get; }
        
        public bool Wasei { get; }
        
        public EdictLoanSourceType SourceType { get; }

        public EdictLoanSource(string sourceLanguage, bool wasei, EdictLoanSourceType sourceType, Option<string> loanWord)
        {
            SourceLanguage = sourceLanguage;
            Wasei = wasei;
            SourceType = sourceType;
            LoanWord = loanWord;
        }
    }
}