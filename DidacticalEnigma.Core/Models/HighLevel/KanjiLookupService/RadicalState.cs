using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace DidacticalEnigma.Core.Models.HighLevel.KanjiLookupService
{
    [Serializable]
    public class RadicalState
    {
        [Required] [NotNull] public string Radical { get; }
        
        [Required] public bool IsAvailable { get; }
        
        [Required] public bool IsSelected { get; }

        public RadicalState([NotNull] string radical, bool isAvailable, bool isSelected)
        {
            Radical = radical ?? throw new ArgumentNullException(nameof(radical));
            IsAvailable = isAvailable;
            IsSelected = isSelected;
        }
    }
}