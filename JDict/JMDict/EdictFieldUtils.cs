using System;
using Optional;
using Utility.Utils;

namespace JDict
{
    public static class EdictFieldUtils
    {
        public static Option<EdictField> FromDescription(string description)
        {
            return mapping.FromDescription(description);
        }

        public static string ToDescription(this EdictField d)
        {
            return mapping.ToLongString(d);
        }

        public static string ToAbbrevation(this EdictField d)
        {
            return d.ToString().Replace("_", "-");
        }
        
        public static Option<EdictField> FromAbbrevation(string d)
        {
            if (Enum.TryParse(d.Replace("-", "_"), out EdictField e))
            {
                return e.Some();
            }
            else
            {
                return Option.None<EdictField>();
            }
        }

        private static EnumMapper<EdictField> mapping = new EnumMapper<EdictField>();
    }
}