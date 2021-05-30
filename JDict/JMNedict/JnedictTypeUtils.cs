using System;
using Optional;
using Utility.Utils;

namespace JDict
{
    public static class JnedictTypeUtils
    {
        public static Option<JMNedictType> FromDescription(string description)
        {
            return mapping.FromDescription(description);
        }

        public static string ToLongString(this JMNedictType value)
        {
            return mapping.ToLongString(value);
        }
        
        public static string ToAbbrevation(this JMNedictType d)
        {
            return d.ToString().Replace("_", "-");
        }
        
        public static Option<JMNedictType> FromAbbrevation(string d)
        {
            if (Enum.TryParse(d.Replace("-", "_"), out JMNedictType e))
            {
                return e.Some();
            }
            else
            {
                return Option.None<JMNedictType>();
            }
        }

        private static EnumMapper<JMNedictType> mapping = new EnumMapper<JMNedictType>();
    }
}