using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using JDict.Xml;
using Utility.Utils;

namespace JDict
{
    public class JMDictParser : IDisposable
    {
        private static readonly XmlSerializer serializer = new XmlSerializer(typeof(JdicEntry));

        private static readonly XmlReaderSettings xmlSettings = new XmlReaderSettings
        {
            CloseInput = false,
            DtdProcessing = DtdProcessing.Parse, // we have local entities
            XmlResolver = null, // we don't want to resolve against external entities
            MaxCharactersFromEntities = 64 * 1024 * 1024 / 2, // 64 MB
            MaxCharactersInDocument = 256 * 1024 * 1024 / 2, // 256 MB
            IgnoreComments = false
        };

        private XmlReader xmlReader;
        
        public DateTime? Version { get; private set; }
        
        public IReadOnlyDualDictionary<string, string> FriendlyNames { get; private set; }
        
        public JdicEntry Read()
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "entry")
                {
                    using (
                        var elementReader =
                            new StringReader(xmlReader.ReadOuterXml()))
                    {
                        var entry = (JdicEntry)serializer.Deserialize(elementReader);
                        entry.Senses = entry.Senses ?? Array.Empty<Sense>();
                        entry.KanjiElements = entry.KanjiElements ?? Array.Empty<KanjiElement>();
                        entry.ReadingElements = entry.ReadingElements ?? Array.Empty<ReadingElement>();
                        foreach (var s in entry.Senses)
                        {
                            s.Glosses = s.Glosses ?? Array.Empty<Gloss>();
                            s.PartOfSpeech = s.PartOfSpeech ?? Array.Empty<string>();
                            s.Antonym = s.Antonym ?? Array.Empty<string>();
                            s.CrossRef = s.CrossRef ?? Array.Empty<string>();
                            s.Dialect = s.Dialect ?? Array.Empty<string>();
                            s.Field = s.Field ?? Array.Empty<string>();
                            s.Information = s.Information ?? Array.Empty<string>();
                            s.LoanWordSource = s.LoanWordSource ?? Array.Empty<LoanSource>();
                            s.Misc = s.Misc ?? Array.Empty<string>();
                            s.Stagk = s.Stagk ?? Array.Empty<string>();
                            s.Stagkr = s.Stagkr ?? Array.Empty<string>();
                            foreach (var gloss in s.Glosses)
                            {
                                if (gloss.Lang == null)
                                {
                                    gloss.Lang = "eng";
                                }
                            }

                            foreach (var loanSource in s.LoanWordSource)
                            {
                                if (loanSource.Lang == null)
                                {
                                    loanSource.Lang = "eng";
                                }
                            }
                        }

                        return entry;
                    }
                }
            }
            
            return null;
        }

        public IEnumerable<JdicEntry> ReadRemainingToEnd()
        {
            JdicEntry entry;
            while ((entry = this.Read()) != null)
            {
                yield return entry;
            }
        }

        public static JMDictParser Create(Stream stream)
        {
            DateTime? versionDate = null;
            var undoEntityExpansionDictionary = new DualDictionary<string, string>();

            var xmlReader = XmlReader.Create(stream, xmlSettings);
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.DocumentType)
                {
                    undoEntityExpansionDictionary = new DualDictionary<string, string>(XmlEntities
                        .ParseJMDictEntities(xmlReader.Value)
                        .DistinctBy(kvp => kvp.Key));
                }

                if (xmlReader.NodeType == XmlNodeType.Comment)
                {
                    var commentText = xmlReader.Value.Trim();
                    if (commentText.StartsWith("JMdict created:", StringComparison.Ordinal))
                    {
                        var generationDate = commentText.Split(':').ElementAtOrDefault(1)?.Trim();
                        if (DateTime.TryParseExact(generationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                            DateTimeStyles.AssumeUniversal, out var date))
                        {
                            versionDate = date;
                        }
                        else
                        {
                            versionDate = null;
                        }
                    }
                }

                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "JMdict")
                {
                    break;
                }
            }
            
            return new JMDictParser(versionDate, undoEntityExpansionDictionary, xmlReader);
        }

        private JMDictParser(DateTime? version, IReadOnlyDualDictionary<string, string> friendlyNames, XmlReader reader)
        {
            this.Version = version;
            this.FriendlyNames = friendlyNames;
            this.xmlReader = reader;
        }

        public void Dispose()
        {
            this.xmlReader.Dispose();
        }
    }
}