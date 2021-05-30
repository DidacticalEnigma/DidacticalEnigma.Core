using System;
using Optional;
using Utility.Utils;

namespace JDict
{
    public static class EdictDialectUtils
    {
        public static Option<EdictDialect> FromDescription(string description)
        {
            return mapping.FromDescription(description);
        }

        public static string ToDescription(this EdictDialect d)
        {
            return mapping.ToLongString(d);
        }

        public static string ToAbbrevation(this EdictDialect d)
        {
            return d.ToString().Replace("_", "-");
        }
        
        public static Option<EdictDialect> FromAbbrevation(string d)
        {
            if (Enum.TryParse(d.Replace("-", "_"), out EdictDialect e))
            {
                return e.Some();
            }
            else
            {
                return Option.None<EdictDialect>();
            }
        }

        private static EnumMapper<EdictDialect> mapping = new EnumMapper<EdictDialect>();
    }
}