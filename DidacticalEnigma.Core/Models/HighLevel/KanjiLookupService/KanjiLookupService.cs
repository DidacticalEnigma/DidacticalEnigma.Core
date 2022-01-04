using System;
using System.Collections.Generic;
using System.Linq;
using DidacticalEnigma.Core.Models.LanguageService;
using JDict;
using Optional;
using Optional.Collections;
using Optional.Unsafe;
using Utility.Utils;
using Radical = JDict.Radical;

namespace DidacticalEnigma.Core.Models.HighLevel.KanjiLookupService
{
    
#nullable enable
    internal static class ProjectingEqualityComparer<TIn>
        where TIn : notnull
    {
        public static IEqualityComparer<TIn> Create<TOut>(
            Func<TIn, TOut> projection,
            IEqualityComparer<TOut>? comparer = null)
            where TOut : notnull
        {
            return new ProjectingEqualityComparer<TIn, TOut>(projection, comparer);
        }
    }
    
    internal class ProjectingEqualityComparer<TIn, TOut> : IEqualityComparer<TIn>
        where TIn : notnull
        where TOut : notnull
    {
        private readonly Func<TIn, TOut> projection;
        private readonly IEqualityComparer<TOut> comparer;

        public ProjectingEqualityComparer(
            Func<TIn, TOut> projection,
            IEqualityComparer<TOut>? comparer = null)
        {
            this.projection = projection;
            this.comparer = comparer ?? EqualityComparer<TOut>.Default;
        }
        
        public bool Equals(TIn? x, TIn? y)
        {
            if (x is null && y is null)
            {
                return true;
            }

            if (x is null && y is not null)
            {
                return comparer.Equals(default(TOut?), projection(y));
            }
            
            if (x is not null && y is null)
            {
                return comparer.Equals(projection(x), default(TOut?));
            }

            if (x is not null && y is not null)
            {
                return comparer.Equals(projection(x), projection(y));
            }

            throw new InvalidOperationException();
        }

        public int GetHashCode(TIn obj)
        {
            return comparer.GetHashCode(projection(obj));
        }
    }
#nullable restore

    public class KanjiLookupService : IKanjiLookupService
    {
        private readonly IKanjiRadicalLookup radicalLookup;
        private readonly IRadicalSearcher radicalSearcher;
        private readonly IKanjiProperties kanjiProperties;
        private readonly IReadOnlyDictionary<CodePoint, KanjiAliveJapaneseRadicalInformation.Entry> kanjiAliveInfo;
        private readonly IReadOnlyDictionary<CodePoint, string> textForms;

        public KanjiLookupService(
            IKanjiRadicalLookup radicalLookup,
            IRadicalSearcher radicalSearcher,
            IKanjiProperties kanjiProperties,
            KanjiAliveJapaneseRadicalInformation kanjiAliveInfo,
            RadkfileKanjiAliveCorrelator correlator,
            IReadOnlyDictionary<CodePoint, string> textForms = null)
        {
            this.radicalLookup = radicalLookup;
            this.radicalSearcher = radicalSearcher;
            this.kanjiProperties = kanjiProperties;
            this.kanjiAliveInfo = CreateKanjiAliveMapping(
                radicalLookup.AllRadicals,
                kanjiAliveInfo,
                correlator);
            this.textForms = textForms;
        }

        private static IReadOnlyDictionary<CodePoint, KanjiAliveJapaneseRadicalInformation.Entry> CreateKanjiAliveMapping(
            IEnumerable<CodePoint> radicals,
            KanjiAliveJapaneseRadicalInformation kanjiAliveInfo,
            RadkfileKanjiAliveCorrelator correlator)
        {
            return radicals
                .Join(
                    kanjiAliveInfo.Where(e => e.StrokeCount.HasValue),
                    radical => correlator.GetValueOrNone(radical.Utf32).ValueOr(0),
                    radicalInfo => char.ConvertToUtf32(radicalInfo.Literal, 0),
                    KeyValuePair.Create)
                .ToDictionary((key, leftValue, rightValue) => leftValue);
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
                        textForms != null ? textForms[c] : r.ToString(),
                        this.kanjiAliveInfo.GetValueOrNone(c).Map(kanjiAliveEntry =>
                        {
                            IReadOnlyCollection<string> queryNames = new[]
                            {
                                kanjiAliveEntry.JapaneseReadings,
                                kanjiAliveEntry.RomajiReadings,
                                kanjiAliveEntry.Meanings,
                            }.SelectMany(x => x).ToList();
                            return queryNames;
                        }).ValueOr(Array.Empty<string>())))
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
            var usedRadicals = EnumerableExt.DistinctBy(radicalSearchResults, r => r.Text)
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