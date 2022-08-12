using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DidacticalEnigma.Core.Models.Formatting;
using DidacticalEnigma.Core.Models.LanguageService;
using JDict;
using JetBrains.Annotations;
using Optional;
using Utility.Utils;

namespace DidacticalEnigma.Core.Models.DataSources;

public class JMDictCompactDataSource : IDataSource
{
    [NotNull] private readonly JMDictLookup jdict;
    [NotNull] private readonly IKanaProperties kana;
    [CanBeNull] private readonly JMDictEntrySorter sorter;

    public static DataSourceDescriptor Descriptor { get; } = new DataSourceDescriptor(
        new Guid("F753F375-9501-408F-837B-92452C0A34E7"),
        "JMDict (Compact)",
        "The data JMdict by Electronic Dictionary Research and Development Group",
        new Uri("http://www.edrdg.org/jmdict/j_jmdict.html"));

    public JMDictCompactDataSource(
        [NotNull] JMDictLookup jdict,
        [NotNull] IKanaProperties kana,
        [CanBeNull] JMDictEntrySorter sorter = null)
    {
        this.jdict = jdict ?? throw new ArgumentNullException(nameof(jdict));
        this.kana = kana ?? throw new ArgumentNullException(nameof(kana));
        this.sorter = sorter;
    }

    public void Dispose()
    {
        
    }

    public Task<Option<RichFormatting>> Answer(Request request, CancellationToken token)
    {
        var rich = new RichFormatting();

        var jmDictEntries = jdict.Lookup(request.NotInflected ?? request.QueryText)
            ?? Enumerable.Empty<JMDictEntry>();

        jmDictEntries = sorter != null ? sorter.Sort(jmDictEntries, request) : jmDictEntries;

        if (request.Word.DictionaryFormReading != null)
        {
            var normalizedReading = kana.ToKatakana(request.Word.DictionaryFormReading);

            jmDictEntries = jmDictEntries
                .OrderByDescending(entry =>
                    entry.ReadingEntries
                        .Select(readingEntry =>
                            kana.ToKatakana(readingEntry.Reading))
                        .Contains(normalizedReading)
                        ? 1
                        : 0);
        }

        foreach (var jmDictEntry in jmDictEntries)
        {
            rich.Paragraphs.Add(new TextParagraph(
                new []
                {
                    jmDictEntry.KanjiEntries.Select(kanjiEntry => new Text(kanjiEntry.Kanji, emphasis: true))
                        .Intersperse(new Text("; ", emphasis: true)),
                    jmDictEntry.ReadingEntries.Select(readingEntry => new Text($"{readingEntry.Reading}", emphasis: true))
                        .Intersperse(new Text("; ", emphasis: true))
                        .Prepend(new Text("【", emphasis: true))
                        .Append(new Text("】", emphasis: true)),
                    jmDictEntry.Senses.SelectMany((sense, position) => 
                        sense.Glosses
                            .Select(gloss => new Text(gloss))
                            .Intersperse(new Text("/"))
                            .Prepend(new Text($" ({position+1}) ", fontSize: FontSize.Small))
                            .Prepend(new Text(" (" + string.Join(",", sense.PartOfSpeechInfo.Select(pos => pos.ToAbbrevation())) + ") ", fontSize: FontSize.Small)))
                }.SelectMany(x => x)));
        }

        if (rich.Paragraphs.Count == 0)
            return Task.FromResult(Option.None<RichFormatting>());

        return Task.FromResult(Option.Some(rich));
    }

    public async Task<UpdateResult> UpdateLocalDataSource(CancellationToken cancellation = default(CancellationToken))
    {
        return UpdateResult.NotSupported;
    }

    public string InstanceIdentifier => null;
}