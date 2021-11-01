using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using JDict.Xml;
using Utility.Utils;

namespace JDict
{
    public class JMNedictParser : IDisposable
    {
        private XmlTextReader xmlReader;
        
        public DualDictionary<string, string> FriendlyNames { get; }
        
        private Stream stream;

        private JMNedictParser(XmlTextReader xmlReader, DualDictionary<string, string> friendlyNames, Stream stream)
        {
            this.xmlReader = xmlReader;
            this.FriendlyNames = friendlyNames;
            this.stream = stream;
        }

        public static JMNedictParser Create(Stream stream)
        {
            var friendlyNames = new DualDictionary<string, string>();
            
            var xmlReader = new XmlTextReader(stream);
            xmlReader.EntityHandling = EntityHandling.ExpandCharEntities;
            xmlReader.DtdProcessing = DtdProcessing.Parse;
            xmlReader.XmlResolver = null;
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.DocumentType)
                {
                    friendlyNames = new DualDictionary<string, string>(XmlEntities
                        .ParseJMDictEntities(xmlReader.Value)
                        .DistinctBy(kvp => kvp.Key));
                }

                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "JMnedict")
                {
                    break;
                }
            }
            return new JMNedictParser(xmlReader, friendlyNames, stream);
        }
        
        public IEnumerable<JnedictEntry> ReadRemainingToEnd()
        {
            JnedictEntry entry;
            while ((entry = this.Read()) != null)
            {
                yield return entry;
            }
        }

        public JnedictEntry Read()
        {
            long? sequenceNumber = null;
            var kanjiElements = new List<string>();
            var readingElements = new List<string>();
            var translations = new List<JnedictTranslation>();
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "entry")
                {
                    return new JnedictEntry(
                        sequenceNumber ?? throw new InvalidDataException(),
                        kanjiElements,
                        readingElements,
                        translations);
                }

                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "ent_seq")
                {
                    sequenceNumber = ReadEntSeq(xmlReader.Depth, xmlReader.Name);
                }

                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "k_ele")
                {
                    kanjiElements.Add(ReadKanjiElement(xmlReader.Depth, xmlReader.Name));
                }

                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "r_ele")
                {
                    readingElements.Add(ReadReadingElement(xmlReader.Depth, xmlReader.Name));
                }
                
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "trans")
                {
                    translations.Add(ReadTrans(xmlReader.Depth, xmlReader.Name));
                }
            }

            return null;
        }

        private JnedictTranslation ReadTrans(int depth, string tag)
        {
            var types = new List<JMNedictType>();
            var translations = new List<string>();
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == tag && xmlReader.Depth == depth)
                {
                    break;
                }

                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "name_type")
                {
                    types.Add(ReadNameType(xmlReader.Depth, xmlReader.Name));
                }

                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "trans_det")
                {
                    var translation = ReadTransDet(xmlReader.Depth, xmlReader.Name);
                    if (translation != null)
                    {
                        translations.Add(translation);
                    }
                }
            }

            return new JnedictTranslation(
                types,
                translations);
        }

        private string ReadTransDet(int depth, string tag)
        {
            string translation = null;
            var lang = xmlReader.GetAttribute("xml:lang");
            if (lang != null && lang != "eng")
            {
                return null;
            }
            
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == tag && xmlReader.Depth == depth)
                {
                    break;
                }

                if (xmlReader.NodeType == XmlNodeType.Text)
                {
                    translation = xmlReader.Value;
                }
            }

            return translation;
        }

        private JMNedictType ReadNameType(int depth, string tag)
        {
            JMNedictType? type = null;
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == tag && xmlReader.Depth == depth)
                {
                    break;
                }
                
                if (xmlReader.NodeType == XmlNodeType.EntityReference)
                {
                    switch (xmlReader.Name)
                    {
                        case "char":
                            type = JMNedictType.@char; 
                            break;
                        case "company":
                            type = JMNedictType.company; 
                            break;
                        case "creat":
                            type = JMNedictType.creat; 
                            break;
                        case "dei":
                            type = JMNedictType.dei; 
                            break;
                        case "doc":
                            type = JMNedictType.doc; 
                            break;
                        case "ev":
                            type = JMNedictType.ev; 
                            break;
                        case "fem":
                            type = JMNedictType.fem; 
                            break;
                        case "fict":
                            type = JMNedictType.fict; 
                            break;
                        case "given":
                            type = JMNedictType.given; 
                            break;
                        case "group":
                            type = JMNedictType.@group; 
                            break;
                        case "leg":
                            type = JMNedictType.leg; 
                            break;
                        case "masc":
                            type = JMNedictType.masc; 
                            break;
                        case "myth":
                            type = JMNedictType.myth; 
                            break;
                        case "obj":
                            type = JMNedictType.obj; 
                            break;
                        case "organization":
                            type = JMNedictType.organization; 
                            break;
                        case "oth":
                            type = JMNedictType.oth; 
                            break;
                        case "person":
                            type = JMNedictType.person; 
                            break;
                        case "place":
                            type = JMNedictType.place; 
                            break;
                        case "product":
                            type = JMNedictType.product; 
                            break;
                        case "relig":
                            type = JMNedictType.relig; 
                            break;
                        case "serv":
                            type = JMNedictType.serv; 
                            break;
                        case "station":
                            type = JMNedictType.station; 
                            break;
                        case "surname":
                            type = JMNedictType.surname; 
                            break;
                        case "unclass":
                            type = JMNedictType.unclass; 
                            break;
                        case "work":
                            type = JMNedictType.work; 
                            break;
                    }
                }
            }

            return type ?? throw new InvalidDataException(tag);
        }

        private long? ReadEntSeq(int depth, string tag)
        {
            long? sequenceNumber = null;
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == tag && xmlReader.Depth == depth)
                {
                    break;
                }

                if (xmlReader.NodeType == XmlNodeType.Text)
                {
                    if (long.TryParse(xmlReader.Value, NumberStyles.Integer, CultureInfo.InvariantCulture,
                        out long seq))
                    {
                        sequenceNumber = seq;
                    }
                }
            }

            return sequenceNumber;
        }

        private string ReadKanjiElement(int depth, string tag)
        {
            string kanjiElement = null;
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == tag && xmlReader.Depth == depth)
                {
                    break;
                }

                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "keb")
                {
                    kanjiElement = ReadKeb(xmlReader.Depth, xmlReader.Name);
                }
            }

            return kanjiElement;
        }

        private string ReadKeb(int depth, string tag)
        {
            string kanjiElement = null;
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == tag && xmlReader.Depth == depth)
                {
                    break;
                }

                if (xmlReader.NodeType == XmlNodeType.Text)
                {
                    kanjiElement = xmlReader.Value;
                }
            }

            return kanjiElement;
        }
        
        private string ReadReadingElement(int depth, string tag)
        {
            string readingElement = null;
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == tag && xmlReader.Depth == depth)
                {
                    break;
                }

                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "reb")
                {
                    readingElement = ReadReb(xmlReader.Depth, xmlReader.Name);
                }
            }

            return readingElement;
        }
        
        private string ReadReb(int depth, string tag)
        {
            string readingElement = null;
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == tag && xmlReader.Depth == depth)
                {
                    break;
                }

                if (xmlReader.NodeType == XmlNodeType.Text)
                {
                    readingElement = xmlReader.Value;
                }
            }

            return readingElement;
        }

        public void Dispose()
        {
            this.xmlReader.Dispose();
            this.stream.Dispose();
        }
    }
}