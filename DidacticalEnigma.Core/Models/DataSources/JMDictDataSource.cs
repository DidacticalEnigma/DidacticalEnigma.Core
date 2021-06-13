using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DidacticalEnigma.Core.Models.Formatting;
using DidacticalEnigma.Core.Models.LanguageService;
using JDict;
using Optional;
using Utility.Utils;

namespace DidacticalEnigma.Core.Models.DataSources
{
    public class JMDictDataSource : IDataSource
    {
        private readonly JMDictLookup jdict;
        private readonly IKanaProperties kana;

        public static DataSourceDescriptor Descriptor { get; } = new DataSourceDescriptor(
            new Guid("ED1B840C-B2A8-4018-87B0-D5FC64A1ABC8"),
            "JMDict",
            "The data JMdict by Electronic Dictionary Research and Development Group",
            new Uri("http://www.edrdg.org/jmdict/j_jmdict.html"));

        public Task<Option<RichFormatting>> Answer(Request request, CancellationToken token)
        {
            return DictUtils.Lookup(
                request,
                t => jdict.Lookup(t),
                r => GreedyLookup(r),
                kana,
                Render);
        }

        private (IEnumerable<JMDictEntry> entry, string word) GreedyLookup(Request request, int backOffCountStart = 5)
        {
            if (request.SubsequentWords == null)
                return (null, null);

            return DictUtils.GreedyLookup(s => jdict.Lookup(s), request.SubsequentWords, backOffCountStart);
        }

        private IEnumerable<Paragraph> Render(IEnumerable<JMDictEntry> entries)
        {
            foreach (var entry in entries)
            {
                var l = new List<Text>();
                {
                    bool first = true;
                    foreach (var kanji in entry.KanjiEntries)
                    {
                        if (!first)
                            l.Add(new Text(";  "));
                        first = false;
                        l.Add(new Text(kanji.Kanji));
                        var infoList = kanji.Informational.Materialize();
                        if (infoList.Count != 0)
                        {
                            l.Add(new Text("(", fontSize: FontSize.Small));
                            foreach (var information in infoList)
                            {
                                l.Add(new Text(jdict.FriendlyDescriptionOf(information), fontSize: FontSize.Small));
                            }
                            l.Add(new Text(")", fontSize: FontSize.Small));
                        }
                    }

                    l.Add(new Text("\n"));
                }
                {
                    bool first = true;
                    foreach (var reading in entry.ReadingEntries)
                    {
                        if (!first)
                            l.Add(new Text("\n"));
                        first = false;
                        l.Add(new Text(reading.Reading));
                        var infoList = reading.ReadingInformation.Materialize();
                        var validForList = reading.ValidReadingFor.Materialize();
                        var notATrueReading = reading.NotATrueReading;
                        if (notATrueReading || infoList.Count != 0 || validForList.Count != 0)
                        {
                            l.Add(new Text(" (", fontSize: FontSize.Small));
                        }

                        if (notATrueReading)
                        {
                            l.Add(new Text("not a true reading", fontSize: FontSize.Small));
                        }

                        if (infoList.Count != 0)
                        {
                            if (notATrueReading)
                            {
                                l.Add(new Text(", ", fontSize: FontSize.Small));                                
                            }
                            bool f = true;
                            foreach (var information in infoList)
                            {
                                if (!f)
                                {
                                    l.Add(new Text(", "));
                                }

                                l.Add(new Text(jdict.FriendlyDescriptionOf(information), fontSize: FontSize.Small));
                                f = false;
                            }
                        }
                        
                        if(validForList.Count != 0)
                        {
                            if (notATrueReading || infoList.Count != 0)
                            {
                                l.Add(new Text(", ", fontSize: FontSize.Small));
                            }
                            
                            l.Add(new Text("only applicable to:", fontSize: FontSize.Small));
                            bool f = true;
                            foreach (var validReading in validForList)
                            {
                                if (!f)
                                {
                                    l.Add(new Text(", "));
                                }

                                l.Add(new Text(validReading, fontSize: FontSize.Small));
                                f = false;
                            }
                        }

                        if (notATrueReading || infoList.Count != 0 || validForList.Count != 0)
                        {
                            l.Add(new Text(")", fontSize: FontSize.Small));
                        }
                    }

                    l.Add(new Text("\n"));
                }
                {
                    foreach (var sense in entry.Senses)
                    {
                        l.Add(new Text(string.Join("/", sense.PartOfSpeechInfo.Select(pos => jdict.FriendlyDescriptionOf(pos))), fontSize: FontSize.ExtraSmall));
                        l.Add(new Text("\n"));
                        {
                            bool first = true;
                            foreach (var dialect in sense.DialectalInfo)
                            {
                                if(!first)
                                    l.Add(new Text(", ", fontSize: FontSize.Medium));
                                first = false;
                                l.Add(new Text(jdict.FriendlyDescriptionOf(dialect), fontSize: FontSize.Medium));
                            }
                            if(!first)
                                l.Add(new Text("\n"));
                        }
                        {
                            bool first = true;
                            foreach (var field in sense.FieldData)
                            {
                                if(!first)
                                    l.Add(new Text(", ", fontSize: FontSize.Medium));
                                first = false;
                                l.Add(new Text(jdict.FriendlyDescriptionOf(field), fontSize: FontSize.Medium));
                            }
                            if(!first)
                                l.Add(new Text("\n"));
                        }
                        {
                            bool first = true;
                            foreach (var misc in sense.Misc)
                            {
                                if(!first)
                                    l.Add(new Text(", ", fontSize: FontSize.Medium));
                                first = false;
                                l.Add(new Text(jdict.FriendlyDescriptionOf(misc), fontSize: FontSize.Medium));
                            }
                            if(!first)
                                l.Add(new Text("\n"));
                        }
                        foreach (var inf in sense.Informational)
                        {
                            l.Add(new Text(inf, fontSize: FontSize.Medium));
                            l.Add(new Text("\n"));
                        }
                        l.Add(new Text(string.Join("/", sense.Glosses)));
                        l.Add(new Text("\n"));
                    }
                }
                yield return new TextParagraph(l);
            }
        }

        public void Dispose()
        {

        }

        public Task<UpdateResult> UpdateLocalDataSource(CancellationToken cancellation = default)
        {
            return Task.FromResult(UpdateResult.NotSupported);
        }

        public JMDictDataSource(JMDictLookup jdict, IKanaProperties kana)
        {
            this.jdict = jdict;
            this.kana = kana;
        }
    }
}