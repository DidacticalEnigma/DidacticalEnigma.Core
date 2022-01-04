using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace DidacticalEnigma.Core.Models.HighLevel.KanjiLookupService
{
    [Serializable]
    public class ExtendedRadicalInformation
    {
        [Required] [NotNull] public string Radical { get; }
        
        [Required] public int StrokeCount { get; }
        
        [Required] [NotNull] public string AlternativeDisplay { get; }
        
        [Required] [NotNull] public IReadOnlyCollection<string> QueryNames { get; }

        public ExtendedRadicalInformation(
            [NotNull] string radical,
            int strokeCount,
            [NotNull] string alternativeDisplay,
            [NotNull] IReadOnlyCollection<string> queryNames)
        {
            Radical = radical ?? throw new ArgumentNullException(nameof(radical));
            StrokeCount = strokeCount;
            AlternativeDisplay = alternativeDisplay ?? throw new ArgumentNullException(nameof(alternativeDisplay));
            QueryNames = queryNames ?? throw new ArgumentNullException(nameof(queryNames));
        }
    }
}