using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JDict;
using JetBrains.Annotations;
using Optional;

namespace DidacticalEnigma.Core.Models.LanguageService;

public class SameReadingKanjiLookup
{
    private readonly KanjiDict kanjiDict;
    private readonly IKanaProperties kanaProperties;

    public SameReadingKanjiLookup(
        KanjiDict kanjiDict,
        IKanaProperties kanaProperties)
    {
        this.kanjiDict = kanjiDict;
        this.kanaProperties = kanaProperties;
    }
    
    public Option<Result> Lookup(string kanji)
    {
        return kanjiDict.Lookup(kanji)
            .Map(ResultFound);
    }

    private Result ResultFound(KanjiEntry kanjiEntry)
    {
        var readings =
            kanjiEntry.KunReadings
                .Concat(kanjiEntry.OnReadings)
                .ToHashSet();

        var sameKanji = kanjiDict.LookupByReading(readings).OrderBy(k => k.FrequencyRating).ToList();

        return new Result(
            Character: kanjiEntry.Literal,
            KanjiEntry: kanjiEntry,
            KanjiWithSameReading: readings
                .Select(reading =>
                {
                    IEnumerable<KanjiEntry> kanjiEntries = sameKanji
                        .Where(k => 
                            k.KunReadings
                                .Concat(k.OnReadings)
                                .Contains(reading))
                        .ToList();
                    return KeyValuePair.Create(
                        reading,
                        kanjiEntries);
                }));
    }

    public record Result(
        string Character,
        KanjiEntry KanjiEntry,
        IEnumerable<KeyValuePair<string, IEnumerable<KanjiEntry>>> KanjiWithSameReading);
}