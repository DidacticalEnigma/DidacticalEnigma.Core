﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DidacticalEnigma.Core.Models.Formatting;
using DidacticalEnigma.Core.Models.LanguageService;
using JDict;
using Optional;

namespace DidacticalEnigma.Core.Models.DataSources
{
    public class JGramDataSource : IDataSource
    {
        private IJGramLookup lookup;

        public static DataSourceDescriptor Descriptor { get; } = new DataSourceDescriptor(
            new Guid("AF65046C-35CB-4856-9774-943203E26979"),
            "JGram",
            "JGram: The Japanese Grammar Database",
            new Uri("http://www.jgram.org/"));

        public Task<Option<RichFormatting>> Answer(Request request, CancellationToken token)
        {
            var rich = new RichFormatting();

            var key = string.Join("", request.SubsequentWords.Take(10));
            var results = lookup.Lookup(key);

            foreach (var paragraph in results.SelectMany(Render))
            {
                rich.Paragraphs.Add(paragraph);
            }

            return Task.FromResult(Option.Some(rich));
        }

        private IEnumerable<Paragraph> Render(JGram.Entry entry)
        {
            var text = new List<Text>()
            {
                new Text($"{entry.Key} [{entry.Reading}]\n"),
                new Text(entry.Translation + "\n")
            };
            if(entry.Example != null)
                text.Add(new Text(entry.Example, fontSize: FontSize.Small));
            yield return new TextParagraph(text);
            yield return new LinkParagraph(new Uri("https://takoboto.jp/bunpo/" + entry.Id), "more info");
        }

        public void Dispose()
        {
            // purposefully not disposing language service
        }

        public Task<UpdateResult> UpdateLocalDataSource(CancellationToken cancellation = default)
        {
            return Task.FromResult(UpdateResult.NotSupported);
        }

        public JGramDataSource(IJGramLookup lookup)
        {
            this.lookup = lookup;
        }
        
        public string InstanceIdentifier => null;
    }
}
