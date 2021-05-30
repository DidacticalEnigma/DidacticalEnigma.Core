using System;
using Optional;
using Utility.Utils;

namespace JDict
{
    public static class EdictMiscUtils
    {
        public static Option<EdictMisc> FromDescription(string description)
        {
            return mapping.FromDescription(description);
        }

        public static string ToDescription(this EdictMisc d)
        {
            return mapping.ToLongString(d);
        }

        public static string ToAbbrevation(this EdictMisc d)
        {
            return d.ToString().Replace("_", "-");
        }
        
        public static Option<EdictMisc> FromAbbrevation(string d)
        {
            if (Enum.TryParse(d.Replace("-", "_"), out EdictMisc e))
            {
                return e.Some();
            }
            else
            {
                return Option.None<EdictMisc>();
            }
        }

        private static EnumMapper<EdictMisc> mapping = new EnumMapper<EdictMisc>();
    }
}