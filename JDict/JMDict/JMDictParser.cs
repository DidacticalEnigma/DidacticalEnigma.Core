using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using JDict.Xml;
using Optional;
using Optional.Collections;
using Utility.Utils;

namespace JDict
{
    public class JMDictParser : IDisposable
    {
        private XmlReader xmlReader;

        public DateTime? Version { get; private set; }
        
        public IReadOnlyDualDictionary<string, string> FriendlyNames { get; private set; }

        public JMDictEntry Read()
        {
            long? sequenceNumber = null;
            var kanjiElements = new List<string>();
            var readingElements = new List<string>();
            var senses = new List<JMDictSense>();
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "entry")
                {
                    return new JMDictEntry(
                        sequenceNumber ?? throw new InvalidDataException(),
                        readingElements,
                        kanjiElements,
                        senses);
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

                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "sense")
                {
                    senses.Add(ReadSense(xmlReader.Depth, xmlReader.Name, senses));
                }
            }

            return null;
        }

        private JMDictSense ReadSense(int depth, string tag, IReadOnlyList<JMDictSense> priorSenses)
        {
            var partOfSpeechList = new List<EdictPartOfSpeech>();
            var dialectList = new List<EdictDialect>();
            var textEntryList = new List<string>();
            var infoList = new List<string>();
            var fieldList = new List<EdictField>();
            var miscList = new List<EdictMisc>();
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == tag && xmlReader.Depth == depth)
                {
                    break;
                }
                
                // stagk*, stagr*, xref*, ant*, lsource*, dial*, gloss*
                
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "pos")
                {
                    partOfSpeechList.Add(ReadPos(xmlReader.Depth, xmlReader.Name));
                }
                
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "field")
                {
                    fieldList.Add(ReadField(xmlReader.Depth, xmlReader.Name));
                }
                
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "misc")
                {
                    miscList.Add(ReadMisc(xmlReader.Depth, xmlReader.Name));
                }
                
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "s_inf")
                {
                    infoList.Add(ReadSInf(xmlReader.Depth, xmlReader.Name));
                }
                
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "dial")
                {
                    dialectList.Add(ReadDial(xmlReader.Depth, xmlReader.Name));
                }
                
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "gloss")
                {
                    textEntryList.Add(ReadGloss(xmlReader.Depth, xmlReader.Name));
                }
            }

            if (partOfSpeechList.Count == 0)
            {
                partOfSpeechList.AddRange(priorSenses.LastOrDefault()?.PartOfSpeechInfo ??
                                          Enumerable.Empty<EdictPartOfSpeech>());
            }

            return new JMDictSense(
                partOfSpeechList.FirstOrNone(),
                partOfSpeechList,
                dialectList,
                textEntryList,
                infoList,
                fieldList,
                miscList);
        }

        private EdictPartOfSpeech ReadPos(int depth, string tag)
        {
            EdictPartOfSpeech? pos = null;
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
                        case "adj-f": pos = EdictPartOfSpeech.adj_f; break;
                        case "adj-i": pos = EdictPartOfSpeech.adj_i; break;
                        case "adj-ix": pos = EdictPartOfSpeech.adj_ix; break;
                        case "adj-kari": pos = EdictPartOfSpeech.adj_kari; break;
                        case "adj-ku": pos = EdictPartOfSpeech.adj_ku; break;
                        case "adj-na": pos = EdictPartOfSpeech.adj_na; break;
                        case "adj-nari": pos = EdictPartOfSpeech.adj_nari; break;
                        case "adj-no": pos = EdictPartOfSpeech.adj_no; break;
                        case "adj-pn": pos = EdictPartOfSpeech.adj_pn; break;
                        case "adj-shiku": pos = EdictPartOfSpeech.adj_shiku; break;
                        case "adj-t": pos = EdictPartOfSpeech.adj_t; break;
                        case "adv": pos = EdictPartOfSpeech.adv; break;
                        case "adv-to": pos = EdictPartOfSpeech.adv_to; break;
                        case "aux": pos = EdictPartOfSpeech.aux; break;
                        case "aux-adj": pos = EdictPartOfSpeech.aux_adj; break;
                        case "aux-v": pos = EdictPartOfSpeech.aux_v; break;
                        case "conj": pos = EdictPartOfSpeech.conj; break;
                        case "cop": pos = EdictPartOfSpeech.cop; break;
                        case "ctr": pos = EdictPartOfSpeech.ctr; break;
                        case "exp": pos = EdictPartOfSpeech.exp; break;
                        case "int": pos = EdictPartOfSpeech.@int; break;
                        case "n": pos = EdictPartOfSpeech.n; break;
                        case "n-adv": pos = EdictPartOfSpeech.n_adv; break;
                        case "n-pr": pos = EdictPartOfSpeech.n_pr; break;
                        case "n-pref": pos = EdictPartOfSpeech.n_pref; break;
                        case "n-suf": pos = EdictPartOfSpeech.n_suf; break;
                        case "n-t": pos = EdictPartOfSpeech.n_t; break;
                        case "num": pos = EdictPartOfSpeech.num; break;
                        case "pn": pos = EdictPartOfSpeech.pn; break;
                        case "pref": pos = EdictPartOfSpeech.pref; break;
                        case "prt": pos = EdictPartOfSpeech.prt; break;
                        case "suf": pos = EdictPartOfSpeech.suf; break;
                        case "unc": pos = EdictPartOfSpeech.unc; break;
                        case "v-unspec": pos = EdictPartOfSpeech.v_unspec; break;
                        case "v1": pos = EdictPartOfSpeech.v1; break;
                        case "v1-s": pos = EdictPartOfSpeech.v1_s; break;
                        case "v2a-s": pos = EdictPartOfSpeech.v2a_s; break;
                        case "v2b-k": pos = EdictPartOfSpeech.v2b_k; break;
                        case "v2b-s": pos = EdictPartOfSpeech.v2b_s; break;
                        case "v2d-k": pos = EdictPartOfSpeech.v2d_k; break;
                        case "v2d-s": pos = EdictPartOfSpeech.v2d_s; break;
                        case "v2g-k": pos = EdictPartOfSpeech.v2g_k; break;
                        case "v2g-s": pos = EdictPartOfSpeech.v2g_s; break;
                        case "v2h-k": pos = EdictPartOfSpeech.v2h_k; break;
                        case "v2h-s": pos = EdictPartOfSpeech.v2h_s; break;
                        case "v2k-k": pos = EdictPartOfSpeech.v2k_k; break;
                        case "v2k-s": pos = EdictPartOfSpeech.v2k_s; break;
                        case "v2m-k": pos = EdictPartOfSpeech.v2m_k; break;
                        case "v2m-s": pos = EdictPartOfSpeech.v2m_s; break;
                        case "v2n-s": pos = EdictPartOfSpeech.v2n_s; break;
                        case "v2r-k": pos = EdictPartOfSpeech.v2r_k; break;
                        case "v2r-s": pos = EdictPartOfSpeech.v2r_s; break;
                        case "v2s-s": pos = EdictPartOfSpeech.v2s_s; break;
                        case "v2t-k": pos = EdictPartOfSpeech.v2t_k; break;
                        case "v2t-s": pos = EdictPartOfSpeech.v2t_s; break;
                        case "v2w-s": pos = EdictPartOfSpeech.v2w_s; break;
                        case "v2y-k": pos = EdictPartOfSpeech.v2y_k; break;
                        case "v2y-s": pos = EdictPartOfSpeech.v2y_s; break;
                        case "v2z-s": pos = EdictPartOfSpeech.v2z_s; break;
                        case "v4b": pos = EdictPartOfSpeech.v4b; break;
                        case "v4g": pos = EdictPartOfSpeech.v4g; break;
                        case "v4h": pos = EdictPartOfSpeech.v4h; break;
                        case "v4k": pos = EdictPartOfSpeech.v4k; break;
                        case "v4m": pos = EdictPartOfSpeech.v4m; break;
                        case "v4n": pos = EdictPartOfSpeech.v4n; break;
                        case "v4r": pos = EdictPartOfSpeech.v4r; break;
                        case "v4s": pos = EdictPartOfSpeech.v4s; break;
                        case "v4t": pos = EdictPartOfSpeech.v4t; break;
                        case "v5aru": pos = EdictPartOfSpeech.v5aru; break;
                        case "v5b": pos = EdictPartOfSpeech.v5b; break;
                        case "v5g": pos = EdictPartOfSpeech.v5g; break;
                        case "v5k": pos = EdictPartOfSpeech.v5k; break;
                        case "v5k-s": pos = EdictPartOfSpeech.v5k_s; break;
                        case "v5m": pos = EdictPartOfSpeech.v5m; break;
                        case "v5n": pos = EdictPartOfSpeech.v5n; break;
                        case "v5r": pos = EdictPartOfSpeech.v5r; break;
                        case "v5r-i": pos = EdictPartOfSpeech.v5r_i; break;
                        case "v5s": pos = EdictPartOfSpeech.v5s; break;
                        case "v5t": pos = EdictPartOfSpeech.v5t; break;
                        case "v5u": pos = EdictPartOfSpeech.v5u; break;
                        case "v5u-s": pos = EdictPartOfSpeech.v5u_s; break;
                        case "v5uru": pos = EdictPartOfSpeech.v5uru; break;
                        case "vi": pos = EdictPartOfSpeech.vi; break;
                        case "vk": pos = EdictPartOfSpeech.vk; break;
                        case "vn": pos = EdictPartOfSpeech.vn; break;
                        case "vr": pos = EdictPartOfSpeech.vr; break;
                        case "vs": pos = EdictPartOfSpeech.vs; break;
                        case "vs-c": pos = EdictPartOfSpeech.vs_c; break;
                        case "vs-i": pos = EdictPartOfSpeech.vs_i; break;
                        case "vs-s": pos = EdictPartOfSpeech.vs_s; break;
                        case "vt": pos = EdictPartOfSpeech.vt; break;
                        case "vz": pos = EdictPartOfSpeech.vz; break;
                    }
                }
            }

            return pos ?? throw new InvalidDataException();
        }

        private string ReadGloss(int depth, string tag)
        {
            string sInf = null;
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == tag && xmlReader.Depth == depth)
                {
                    break;
                }

                if (xmlReader.NodeType == XmlNodeType.Text)
                {
                    sInf = xmlReader.Value;
                }
            }

            return sInf ?? throw new InvalidDataException();
        }

        private EdictDialect ReadDial(int depth, string tag)
        {
            EdictDialect? dial = null;
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
                        case "bra": dial = EdictDialect.bra; break;
                        case "hob": dial = EdictDialect.hob; break;
                        case "ksb": dial = EdictDialect.ksb; break;
                        case "ktb": dial = EdictDialect.ktb; break;
                        case "kyb": dial = EdictDialect.kyb; break;
                        case "kyu": dial = EdictDialect.kyu; break;
                        case "nab": dial = EdictDialect.nab; break;
                        case "osb": dial = EdictDialect.osb; break;
                        case "rkb": dial = EdictDialect.rkb; break;
                        case "thb": dial = EdictDialect.thb; break;
                        case "tsb": dial = EdictDialect.tsb; break;
                        case "tsug": dial = EdictDialect.tsug; break;
                    }
                }
            }

            return dial ?? throw new InvalidDataException();
        }

        private string ReadSInf(int depth, string tag)
        {
            string sInf = null;
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == tag && xmlReader.Depth == depth)
                {
                    break;
                }

                if (xmlReader.NodeType == XmlNodeType.Text)
                {
                    sInf = xmlReader.Value;
                }
            }

            return sInf ?? throw new InvalidDataException();
        }

        int a = 0;
        
        private EdictMisc ReadMisc(int depth, string tag)
        {
            EdictMisc? misc = null;
            
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
                        case "abbr": misc = EdictMisc.abbr; break;
                        case "arch": misc = EdictMisc.arch; break;
                        case "char": misc = EdictMisc.@char; break;
                        case "chn": misc = EdictMisc.chn; break;
                        case "col": misc = EdictMisc.col; break;
                        case "company": misc = EdictMisc.company; break;
                        case "creat": misc = EdictMisc.creat; break;
                        case "dated": misc = EdictMisc.dated; break;
                        case "dei": misc = EdictMisc.dei; break;
                        case "derog": misc = EdictMisc.derog; break;
                        case "doc": misc = EdictMisc.doc; break;
                        case "ev": misc = EdictMisc.ev; break;
                        case "fam": misc = EdictMisc.fam; break;
                        case "fem": misc = EdictMisc.fem; break;
                        case "fict": misc = EdictMisc.fict; break;
                        case "form": misc = EdictMisc.form; break;
                        case "given": misc = EdictMisc.given; break;
                        case "group": misc = EdictMisc.group; break;
                        case "hist": misc = EdictMisc.hist; break;
                        case "hon": misc = EdictMisc.hon; break;
                        case "hum": misc = EdictMisc.hum; break;
                        case "id": misc = EdictMisc.id; break;
                        case "joc": misc = EdictMisc.joc; break;
                        case "leg": misc = EdictMisc.leg; break;
                        case "litf": misc = EdictMisc.litf; break;
                        case "m-sl": misc = EdictMisc.m_sl; break;
                        case "male": misc = EdictMisc.male; break;
                        case "myth": misc = EdictMisc.myth; break;
                        case "net-sl": misc = EdictMisc.net_sl; break;
                        case "obj": misc = EdictMisc.obj; break;
                        case "obs": misc = EdictMisc.obs; break;
                        case "obsc": misc = EdictMisc.obsc; break;
                        case "on-mim": misc = EdictMisc.on_mim; break;
                        case "organization": misc = EdictMisc.organization; break;
                        case "oth": misc = EdictMisc.oth; break;
                        case "person": misc = EdictMisc.person; break;
                        case "place": misc = EdictMisc.place; break;
                        case "poet": misc = EdictMisc.poet; break;
                        case "pol": misc = EdictMisc.pol; break;
                        case "product": misc = EdictMisc.product; break;
                        case "proverb": misc = EdictMisc.proverb; break;
                        case "quote": misc = EdictMisc.quote; break;
                        case "rare": misc = EdictMisc.rare; break;
                        case "relig": misc = EdictMisc.relig; break;
                        case "sens": misc = EdictMisc.sens; break;
                        case "serv": misc = EdictMisc.serv; break;
                        case "sl": misc = EdictMisc.sl; break;
                        case "station": misc = EdictMisc.station; break;
                        case "surname": misc = EdictMisc.surname; break;
                        case "uk": misc = EdictMisc.uk; break;
                        case "unclass": misc = EdictMisc.unclass; break;
                        case "vulg": misc = EdictMisc.vulg; break;
                        case "work": misc = EdictMisc.work; break;
                        case "X": misc = EdictMisc.X; break;
                        case "yoji": misc = EdictMisc.yoji; break;
                    }
                }
            }

            return misc ?? throw new InvalidDataException();
        }

        private EdictField ReadField(int depth, string tag)
        {
            EdictField? field = null;
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
                        case "agric": field = EdictField.agric; break;
                        case "anat": field = EdictField.anat; break;
                        case "archeol": field = EdictField.archeol; break;
                        case "archit": field = EdictField.archit; break;
                        case "art": field = EdictField.art; break;
                        case "astron": field = EdictField.astron; break;
                        case "audvid": field = EdictField.audvid; break;
                        case "aviat": field = EdictField.aviat; break;
                        case "baseb": field = EdictField.baseb; break;
                        case "biochem": field = EdictField.biochem; break;
                        case "biol": field = EdictField.biol; break;
                        case "bot": field = EdictField.bot; break;
                        case "Buddh": field = EdictField.Buddh; break;
                        case "bus": field = EdictField.bus; break;
                        case "chem": field = EdictField.chem; break;
                        case "Christn": field = EdictField.Christn; break;
                        case "cloth": field = EdictField.cloth; break;
                        case "comp": field = EdictField.comp; break;
                        case "cryst": field = EdictField.cryst; break;
                        case "ecol": field = EdictField.ecol; break;
                        case "econ": field = EdictField.econ; break;
                        case "elec": field = EdictField.elec; break;
                        case "electr": field = EdictField.electr; break;
                        case "embryo": field = EdictField.embryo; break;
                        case "engr": field = EdictField.engr; break;
                        case "ent": field = EdictField.ent; break;
                        case "finc": field = EdictField.finc; break;
                        case "fish": field = EdictField.fish; break;
                        case "food": field = EdictField.food; break;
                        case "gardn": field = EdictField.gardn; break;
                        case "genet": field = EdictField.genet; break;
                        case "geogr": field = EdictField.geogr; break;
                        case "geol": field = EdictField.geol; break;
                        case "geom": field = EdictField.geom; break;
                        case "go": field = EdictField.go; break;
                        case "golf": field = EdictField.golf; break;
                        case "gramm": field = EdictField.gramm; break;
                        case "grmyth": field = EdictField.grmyth; break;
                        case "hanaf": field = EdictField.hanaf; break;
                        case "horse": field = EdictField.horse; break;
                        case "law": field = EdictField.law; break;
                        case "ling": field = EdictField.ling; break;
                        case "logic": field = EdictField.logic; break;
                        case "MA": field = EdictField.MA; break;
                        case "mahj": field = EdictField.mahj; break;
                        case "math": field = EdictField.math; break;
                        case "mech": field = EdictField.mech; break;
                        case "med": field = EdictField.med; break;
                        case "met": field = EdictField.met; break;
                        case "mil": field = EdictField.mil; break;
                        case "music": field = EdictField.music; break;
                        case "ornith": field = EdictField.ornith; break;
                        case "paleo": field = EdictField.paleo; break;
                        case "pathol": field = EdictField.pathol; break;
                        case "pharm": field = EdictField.pharm; break;
                        case "phil": field = EdictField.phil; break;
                        case "photo": field = EdictField.photo; break;
                        case "physics": field = EdictField.physics; break;
                        case "physiol": field = EdictField.physiol; break;
                        case "print": field = EdictField.print; break;
                        case "psy": field = EdictField.psy; break;
                        case "psych": field = EdictField.psych; break;
                        case "rail": field = EdictField.rail; break;
                        case "Shinto": field = EdictField.Shinto; break;
                        case "shogi": field = EdictField.shogi; break;
                        case "sports": field = EdictField.sports; break;
                        case "stat": field = EdictField.stat; break;
                        case "sumo": field = EdictField.sumo; break;
                        case "telec": field = EdictField.telec; break;
                        case "tradem": field = EdictField.tradem; break;
                        case "vidg": field = EdictField.vidg; break;
                        case "zool": field = EdictField.zool; break;
                    }
                }
            }

            return field ?? throw new InvalidDataException();
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

        private JMDictEntry CreateEntry(JdicEntry xmlEntry, IReadOnlyDictionary<string, string> expandedNamesToAbbrevationsMapping)
        {
            return new JMDictEntry(
                xmlEntry.Number,
                xmlEntry.ReadingElements.Select(r => r.Reb).ToList(),
                xmlEntry.KanjiElements.Select(k => k.Key).ToList(),
                CreateSenses(xmlEntry, expandedNamesToAbbrevationsMapping));
        }

        private IReadOnlyCollection<JMDictSense> CreateSenses(JdicEntry xmlEntry, IReadOnlyDictionary<string, string> expandedNamesToAbbrevationsMapping)
        {
            var sense = new List<JMDictSense>();
            string[] partOfSpeech = Array.Empty<string>();
            var typedPartOfSpeech = new List<EdictPartOfSpeech>();
            foreach (var s in xmlEntry.Senses)
            {
                if (s.PartOfSpeech.Length > 0)
                {
                    partOfSpeech = s.PartOfSpeech;
                    typedPartOfSpeech = partOfSpeech.Select(posStr =>
                    {
                        var unexpandedName = expandedNamesToAbbrevationsMapping[posStr];
                        return EdictTypeUtils.FromAbbrevation(unexpandedName).ValueOr(() =>
                        {
                            Debug.WriteLine($"{posStr} unknown");
                            return default(EdictPartOfSpeech);
                        });
                    }).ToList();
                }

                sense.Add(new JMDictSense(
                    partOfSpeech.Select(pos => EdictTypeUtils.FromAbbrevation(expandedNamesToAbbrevationsMapping[pos])).FirstOrNone().Flatten(),
                    typedPartOfSpeech,
                    s.Dialect.Select(d => EdictDialectUtils.FromAbbrevation(expandedNamesToAbbrevationsMapping[d])).Values().ToList(),
                    s.Glosses.Select(g => g.Text.Trim()).ToList(),
                    s.Information.ToList(),
                    s.Field.Select(f => EdictFieldUtils.FromAbbrevation(expandedNamesToAbbrevationsMapping[f])).Values().ToList(),
                    s.Misc.Select(m => EdictMiscUtils.FromAbbrevation(expandedNamesToAbbrevationsMapping[m])).Values().ToList()));
            }

            return sense;
        }

        public IEnumerable<JMDictEntry> ReadRemainingToEnd()
        {
            JMDictEntry entry;
            while ((entry = this.Read()) != null)
            {
                yield return entry;
            }
        }

        public static JMDictParser Create(Stream stream)
        {
            DateTime? versionDate = null;
            var undoEntityExpansionDictionary = new DualDictionary<string, string>();

            var xmlReader = new XmlTextReader(stream);
            xmlReader.EntityHandling = EntityHandling.ExpandCharEntities;
            xmlReader.DtdProcessing = DtdProcessing.Ignore;
            xmlReader.XmlResolver = null;
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