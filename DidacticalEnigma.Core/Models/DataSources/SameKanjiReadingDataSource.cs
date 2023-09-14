using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DidacticalEnigma.Core.Models.Formatting;
using DidacticalEnigma.Core.Models.LanguageService;
using Optional;

namespace DidacticalEnigma.Core.Models.DataSources;

public class SameKanjiReadingDataSource : IDataSource
{
    private readonly SameReadingKanjiLookup lookup;

    public static DataSourceDescriptor Descriptor { get; } = new DataSourceDescriptor(
        new Guid("263CB8F6-0183-4C95-9DD3-911B4F9331C0"),
        "Lookup of kanji with same reading",
        "...",
        null);

    public void Dispose()
    {
        
    }

    public SameKanjiReadingDataSource(SameReadingKanjiLookup lookup)
    {
        this.lookup = lookup;
    }

    public Task<Option<RichFormatting>> Answer(Request request, CancellationToken token)
    {
        var option = this.lookup.Lookup(request.Character).Map(
            result =>
            {
                var document = new RichFormatting();

                Paragraph p;
                p = new TextParagraph(new[]
                {
                    new Text("Character: "),
                    new Text(result.Character, emphasis: true)
                });
                document.Paragraphs.Add(p);

                foreach (var (reading, entries) in result.KanjiWithSameReading)
                {
                    var texts = new List<Text>();
                    texts.Add(new Text(reading, emphasis: true));
                    texts.Add(new Text(": "));
                    foreach (var entry in entries)
                    {
                        texts.Add(new Text(entry.Literal));
                        texts.Add(new Text("("));
                        texts.Add(new Text(string.Join(", ", entry.Meanings)));
                        texts.Add(new Text(")"));
                    }
                    p = new TextParagraph(texts);
                    document.Paragraphs.Add(p);
                }

                return document;
            });

        return Task.FromResult(option);
    }

    public async Task<UpdateResult> UpdateLocalDataSource(CancellationToken cancellation = default(CancellationToken))
    {
        return UpdateResult.NoUpdateNeeded;
    }

    public string InstanceIdentifier => null;
}