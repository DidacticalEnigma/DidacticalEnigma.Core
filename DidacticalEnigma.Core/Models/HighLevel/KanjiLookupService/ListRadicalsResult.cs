using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace DidacticalEnigma.Core.Models.HighLevel.KanjiLookupService
{
    [Serializable]
    public class ListRadicalsResult
    {
        [Required] [NotNull] [ItemNotNull] public IReadOnlyCollection<string> PossibleRadicals { get; }
        
        [Required] [NotNull] [ItemNotNull] public IReadOnlyCollection<ExtendedRadicalInformation> RadicalInformation { get; }

        [Required] [NotNull] [ItemNotNull] public IReadOnlyCollection<string> SortingCriteria { get; }

        public ListRadicalsResult(
            [NotNull] [ItemNotNull] IReadOnlyCollection<string> possibleRadicals,
            [NotNull] [ItemNotNull] IReadOnlyCollection<string> sortingCriteria,
            [NotNull] [ItemNotNull] IReadOnlyCollection<ExtendedRadicalInformation> radicalInformation)
        {
            PossibleRadicals = possibleRadicals ?? throw new ArgumentNullException(nameof(possibleRadicals));
            SortingCriteria = sortingCriteria ?? throw new ArgumentNullException(nameof(sortingCriteria));
            RadicalInformation = radicalInformation ?? throw new ArgumentNullException(nameof(radicalInformation));
        }
    }
}