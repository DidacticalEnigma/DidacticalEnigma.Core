using System;
using System.Collections.Generic;
using System.Linq;
using DidacticalEnigma.Core.Models.LanguageService;
using Optional;
using Optional.Unsafe;
using Utility.Utils;

namespace DidacticalEnigma.Core.Models.HighLevel.KanjiLookupService
{
    public class KanjiLookupService : IKanjiLookupService
    {
        private readonly IKanjiRadicalLookup radicalLookup;
        private readonly IRadicalSearcher radicalSearcher;
        private readonly IKanjiProperties kanjiProperties;
        private readonly IReadOnlyDictionary<CodePoint, string> textForms;

        public KanjiLookupService(
            IKanjiRadicalLookup radicalLookup,
            IRadicalSearcher radicalSearcher,
            IKanjiProperties kanjiProperties,
            IReadOnlyDictionary<CodePoint, string> textForms = null)
        {
            this.radicalLookup = radicalLookup;
            this.radicalSearcher = radicalSearcher;
            this.kanjiProperties = kanjiProperties;
            this.textForms = textForms;
        }

        public Option<ListRadicalsResult, Error> ListRadicals()
        {
            return Option.Some<ListRadicalsResult, Error>(new ListRadicalsResult
            (
                possibleRadicals: radicalLookup.AllRadicals
                    .Select(r => r.ToString())
                    .ToList(),
                radicalInformation: this.radicalLookup.AllRadicals.Join(
                    kanjiProperties.Radicals,
                    c => c.Utf32,
                    r => r.CodePoint,
                    (c, r) => new ExtendedRadicalInformation(
                        r.ToString(),
                        r.StrokeCount,
                        textForms != null ? textForms[c] : r.ToString()))
                    .ToList(),
                sortingCriteria: radicalLookup.SortingCriteria
                    .Select(r => r.Description)
                    .ToList()
            ));
        }

        public Option<KanjiLookupResult, Error> SelectRadicals(
            string query,
            string sort,
            string select = null,
            string deselect = null)
        {
            // validation and normalization
            query = query ?? "";
            var sortingCriteriaIndexOpt = sort == null
                ? Option.Some<int>(0)
                : radicalLookup.SortingCriteria
                    .FindIndexOrNone(criterion => criterion.Description == sort);

            if (!sortingCriteriaIndexOpt.HasValue)
            {
                return Option.None<KanjiLookupResult, Error>(
                    new Error(ErrorCodes.InvalidInput, "Invalid sorting criterion", new[] { nameof(sort), sort }));
            }

            var sortingCriteriaIndex = sortingCriteriaIndexOpt.ValueOrFailure();

            // get corresponding radicals
            var radicalSearchResults = radicalSearcher.Search(query);
            var usedRadicals = radicalSearchResults
                .DistinctBy(r => r.Text)
                .Select(r => new KeyValuePair<string, string>(r.Text, r.Radical.ToString()));

            // select
            if (select != null)
            {
                query += " " + select;
            }

            // unselect
            if(deselect != null)
            {
                foreach (var kvp in usedRadicals.Where(x => x.Value == deselect))
                {
                    var name = kvp.Key;
                    query = query.Replace(name, "");
                }
            }

            query = query.Trim();
            
            // search again, with the new radicals
            radicalSearchResults = radicalSearcher.Search(query);
            var radicals = radicalSearchResults
                .Select(result => result.Radical)
                .ToList();
            
            if (radicals.Count == 0)
            {
                return Option.Some<KanjiLookupResult, Error>(new KanjiLookupResult
                (
                    newQuery: query,
                    kanji: Array.Empty<string>(),
                    radicals: radicalLookup.AllRadicals
                        .Select(r => new RadicalState(r.ToString(), isAvailable: true, isSelected: false))
                        .ToList()
                ));
            }

            var selectionResult = radicalLookup.SelectRadical(radicals, sortingCriteriaIndex);
            var usedRadicalsSet = new HashSet<CodePoint>(radicalSearchResults.Select(r => r.Radical));
            return Option.Some<KanjiLookupResult, Error>(new KanjiLookupResult
            (
                newQuery: query,
                kanji: selectionResult.Kanji
                    .Select(k => k.ToString())
                    .ToList(),
                radicals: selectionResult.PossibleRadicals
                    .Select(k => new RadicalState(k.Key.ToString(), k.Value || usedRadicalsSet.Contains(k.Key), usedRadicalsSet.Contains(k.Key)))
                    .ToList()
            ));
        }
    }
}