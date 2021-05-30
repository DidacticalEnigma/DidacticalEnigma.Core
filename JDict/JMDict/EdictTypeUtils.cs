using System;
using Optional;
using Utility.Utils;

namespace JDict
{
    public static class EdictTypeUtils
    {
        public static Option<EdictPartOfSpeech> FromDescription(string description)
        {
            return mapping.FromDescription(description);
        }

        public static string ToDescription(this EdictPartOfSpeech pos)
        {
            return mapping.ToLongString(pos);
        }

        public static string ToAbbrevation(this EdictPartOfSpeech pos)
        {
            return pos.ToString().Replace("_", "-");
        }
        
        public static Option<EdictPartOfSpeech> FromAbbrevation(string d)
        {
            if (Enum.TryParse(d.Replace("-", "_"), out EdictPartOfSpeech e))
            {
                return e.Some();
            }
            else
            {
                return Option.None<EdictPartOfSpeech>();
            }
        }

        private static EnumMapper<EdictPartOfSpeech> mapping = new EnumMapper<EdictPartOfSpeech>();
    }
}