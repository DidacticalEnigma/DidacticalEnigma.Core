﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DidacticalEnigma.Core.Models.Formatting;
using DidacticalEnigma.Core.Models.LanguageService;
using JDict;
using Optional;

namespace DidacticalEnigma.Core.Models.DataSources
{
    public class PartialWordLookupJMDictDataSource : IDataSource
    {
        private readonly PartialWordLookup lookup;
        private readonly FrequencyList list;

        public static DataSourceDescriptor Descriptor { get; } = new DataSourceDescriptor(
            new Guid("1C91B1EE-FD02-413F-B007-58FEF2B998FB"),
            "Partial word search (JMDict)",
            "The data JMdict by Electronic Dictionary Research and Development Group",
            new Uri("http://www.edrdg.org/jmdict/j_jmdict.html"));

        public PartialWordLookupJMDictDataSource(PartialWordLookup lookup, FrequencyList list)
        {
            this.lookup = lookup;
            this.list = list;
        }

        public void Dispose()
        {
            
        }

        public Task<Option<RichFormatting>> Answer(Request request, CancellationToken token)
        {
            var entry = lookup.LookupWords(request.Word.RawWord.Trim());
            var rich = new RichFormatting();
            var p = new TextParagraph();
            p.Content.Add(new Text(string.Join("\n", entry.OrderByDescending(m => list.RateFrequency(m)).Distinct()), fontSize: FontSize.Large));
            rich.Paragraphs.Add(p);
            return Task.FromResult(Option.Some(rich));
        }

        public Task<UpdateResult> UpdateLocalDataSource(CancellationToken cancellation = default(CancellationToken))
        {
            return Task.FromResult(UpdateResult.NotSupported);
        }
        
        public string InstanceIdentifier => null;
    }
}