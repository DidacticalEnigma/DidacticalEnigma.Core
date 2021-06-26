using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Optional;

namespace JDict
{
    public class EdictCrossReference
    {
        public string NameReference { get; }
        
        public Option<string> DisambiguatingNameReference { get; }
        
        public Option<int> SenseIndex { get; }

        public EdictCrossReference(string nameReference, Option<string> disambiguatingNameReference, Option<int> senseIndex)
        {
            NameReference = nameReference;
            DisambiguatingNameReference = disambiguatingNameReference;
            SenseIndex = senseIndex;
        }

        public static EdictCrossReference Parse(string rawXref)
        {
            var componentsArray = rawXref.Split('・');
            var components = componentsArray.AsSpan();
            string nameReference = components[0];
            components = components.Slice(1);
            Option<string> disambiguatingNameReference = Option.None<string>();
            Option<int> senseIndex = Option.None<int>();
            
            if (components.Length >= 1)
            {
                var lastComponent = components[components.Length - 1];
                if (int.TryParse(lastComponent, NumberStyles.Number, CultureInfo.InvariantCulture, out int sense))
                {
                    senseIndex = sense.Some();
                    if (components.Length == 2)
                    {
                        disambiguatingNameReference = components[0].Some();
                    }
                }
                else
                {
                    disambiguatingNameReference = lastComponent.Some();
                }
            }

            return new EdictCrossReference(nameReference, disambiguatingNameReference, senseIndex);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(this.NameReference);
            this.DisambiguatingNameReference.MatchSome(disambiguatingNameReference =>
            {
                sb.Append('・');
                sb.Append(disambiguatingNameReference);
            });
            this.SenseIndex.MatchSome(senseIndex =>
            {
                sb.Append('・');
                sb.Append(senseIndex);
            });
            return sb.ToString();
        }
    }
}