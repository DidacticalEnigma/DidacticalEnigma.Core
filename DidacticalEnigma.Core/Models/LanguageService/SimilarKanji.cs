﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Utility.Utils;

namespace DidacticalEnigma.Core.Models.LanguageService
{
    public class SimilarKanji : IRelated
    {
        private Dictionary<string, List<string>> similar;

        public SimilarKanji(string path, Encoding encoding)
        {
            similar = new Dictionary<string, List<string>>();
            foreach (var line in File.ReadLines(path, encoding))
            {
                var components = line.Split('/');
                similar.Add(
                    components[0],
                    components
                        .Skip(1)
                        .Select(c => c.Trim())
                        .Where(c => c != "")
                        .ToList());
            }
        }

        public IEnumerable<IGrouping<string, CodePoint>> FindRelated(CodePoint codePoint)
        {
            similar.TryGetValue(codePoint.ToString(), out var resultList);
            IEnumerable<string> result = resultList ?? Enumerable.Empty<string>();
            return EnumerableExt.OfSingle(new CategoryGrouping<CodePoint>("Similar Kanji",
                result.Select(r => CodePoint.FromString(r))));
        }
    }
}