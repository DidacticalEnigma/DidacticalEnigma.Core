using System;
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

        public ExtendedRadicalInformation(
            [NotNull] string radical,
            int strokeCount,
            [NotNull] string alternativeDisplay)
        {
            Radical = radical ?? throw new ArgumentNullException(nameof(radical));
            StrokeCount = strokeCount;
            AlternativeDisplay = alternativeDisplay ?? throw new ArgumentNullException(nameof(alternativeDisplay));
        }
    }
}