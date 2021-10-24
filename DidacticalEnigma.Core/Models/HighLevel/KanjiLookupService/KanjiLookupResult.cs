using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace DidacticalEnigma.Core.Models.HighLevel.KanjiLookupService
{
    public class KanjiLookupResult
    {
        [Required] [NotNull] public string NewQuery { get; }
        
        [Required] [NotNull] [ItemNotNull] public IReadOnlyCollection<string> Kanji { get; }

        [Required] [NotNull] [ItemNotNull] public IReadOnlyCollection<RadicalState> Radicals { get; }

        public KanjiLookupResult(
            [NotNull] string newQuery,
            [NotNull] [ItemNotNull] IReadOnlyCollection<string> kanji,
            [NotNull] [ItemNotNull] IReadOnlyCollection<RadicalState> radicals)
        {
            NewQuery = newQuery ?? throw new ArgumentNullException(nameof(newQuery));
            Kanji = kanji ?? throw new ArgumentNullException(nameof(kanji));
            Radicals = radicals?? throw new ArgumentNullException(nameof(radicals));
        }
    }
}