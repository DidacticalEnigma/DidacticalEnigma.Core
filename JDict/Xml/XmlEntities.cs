using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace JDict.Xml
{
    internal static class XmlEntities
    {
        // this parses a subset of the XML DTD format,
        // ignores whether the DTD is inside an XML file or outside of it
        // and returns 
        public static IEnumerable<KeyValuePair<string, string>> ParseJMDictEntities(string data)
        {
            var namePattern = "[:A-Za-z_\u00C0-\u00D6\u00D8-\u00F6\u00F8-\u02FF\u0370-\u037D\u037F-\u1FFF\u200C-\u200D\u2070-\u218F\u2C00-\u2FEF\u3001-\uD7FF\uF900-\uFDCF\uFDF0-\uFFFD][:A-Za-z_\u00C0-\u00D6\u00D8-\u00F6\u00F8-\u02FF\u0370-\u037D\u037F-\u1FFF\u200C-\u200D\u2070-\u218F\u2C00-\u2FEF\u3001-\uD7FF\uF900-\uFDCF\uFDF0-\uFFFD.0-9\u00B7\u0300-\u036F\u203F-\u2040-]*"; // TODO: handle [#x10000-#xEFFFF] characters outside of BMP
            var whitespaceCharacterPattern = "[ \t\r\n]";
            var entityRegex = new Regex(
                "<!ENTITY" + 
                $"{whitespaceCharacterPattern}+" +
                $"({namePattern})" +
                $"{whitespaceCharacterPattern}+" +
                "\"([^%&\"]*)\"" +
                $"{whitespaceCharacterPattern}*" +
                ">");
            var matches = entityRegex.Matches(data).Cast<Match>();
            foreach (var match in matches)
            {
                if (match.Success)
                {
                    yield return new KeyValuePair<string, string>(
                        match.Groups[1].Value,
                        match.Groups[2].Value);
                } 
            }
        }
    }
}