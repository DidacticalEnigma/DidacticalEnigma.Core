using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DidacticalEnigma.Core.Models;
using DidacticalEnigma.Core.Models.DataSources;
using DidacticalEnigma.Core.Models.Formatting;
using DidacticalEnigma.Core.Models.HighLevel.KanjiLookupService;
using DidacticalEnigma.Core.Models.LanguageService;
using Gu.Inject;
using JDict;
using NMeCab;
using Optional.Collections;
using Utility.Utils;

namespace DidacticalEnigma.IoCModule
{
    public static class ServiceConfiguration
    {
        public static ICollection<Func<IReadOnlyKernel, IDataSource>> BindDidacticalEnigmaCoreServices(
            this Kernel kernel,
            string dataDirectory,
            string cacheDirectory)
        {
            kernel.Bind(() => KanjiDict.Create(Path.Combine(dataDirectory, "character", "kanjidic2.xml.gz")));
            kernel.Bind(() =>
                new Kradfile(Path.Combine(dataDirectory, "character", "kradfile1_plus_2_utf8"), Encoding.UTF8));
            kernel.Bind(() =>
                new Radkfile(Path.Combine(dataDirectory, "character", "radkfile1_plus_2_utf8"), Encoding.UTF8));
            kernel.Bind(() => JMDictLookup.Create(Path.Combine(dataDirectory, "dictionaries", "JMdict_e.gz"),
                Path.Combine(cacheDirectory, "dictionaries", "JMdict_e.cache")));
            kernel.Bind(() => JMNedictLookup.Create(Path.Combine(dataDirectory, "dictionaries", "JMnedict.xml.gz"),
                Path.Combine(cacheDirectory, "dictionaries", "JMnedict.xml.cache")));
            kernel.Bind(() =>
                new FrequencyList(Path.Combine(dataDirectory, "other", "word_form_frequency_list.txt"), Encoding.UTF8));
            kernel.Bind(() => new Tanaka(Path.Combine(dataDirectory, "corpora", "examples.utf.gz"), Encoding.UTF8));
            kernel.Bind(() => new JESC(Path.Combine(dataDirectory, "corpora", "jesc_raw"), Encoding.UTF8));
            kernel.Bind(() =>
                new BasicExpressionsCorpus(Path.Combine(dataDirectory, "corpora", "JEC_basic_sentence_v1-2.csv"),
                    Encoding.UTF8));
            kernel.Bind<IMorphologicalAnalyzer<IpadicEntry>>(() => new MeCabIpadic(new MeCabParam
            {
                DicDir = Path.Combine(dataDirectory, "mecab", "ipadic"),
                UseMemoryMappedFile = true
            }));
            kernel.Bind<IMorphologicalAnalyzer<UnidicEntry>>(() => new MeCabUnidic(new MeCabParam
            {
                DicDir = Path.Combine(dataDirectory, "mecab", "unidic"),
                UseMemoryMappedFile = true
            }));
            kernel.Bind<IMorphologicalAnalyzer<IEntry>>(get =>
            {
                // not sure if objects created this way will be disposed twice
                // probably doesn't matter (IDisposable.Dispose's contract says
                // it's not a problem), but still
                try
                {
                    return get.Get<IMorphologicalAnalyzer<UnidicEntry>>();
                }
                catch (Exception)
                {
                    return get.Get<IMorphologicalAnalyzer<IpadicEntry>>();
                }
            });
            kernel.Bind(get => new RadicalRemapper(get.Get<Kradfile>(), get.Get<Radkfile>()));
            kernel.Bind(get => EasilyConfusedKana.FromFile(Path.Combine(dataDirectory, "character", "confused.txt")));
            kernel.Bind<IKanjiProperties, KanjiProperties>();
            kernel.Bind(get => new KanjiProperties(
                get.Get<KanjiDict>(),
                get.Get<Kradfile>(),
                get.Get<Radkfile>(),
                null));
            kernel.Bind<IRomaji, ModifiedHepburn>();
            kernel.Bind(get => new ModifiedHepburn(
                get.Get<IMorphologicalAnalyzer<IEntry>>(),
                get.Get<IKanaProperties>()));
            kernel.Bind<IAutoGlosser, AutoGlosserNext>();
            kernel.Bind(get =>
                new AutoGlosserNext(get.Get<ISentenceParser>(), get.Get<JMDictLookup>(), get.Get<IKanaProperties>()));
            kernel.Bind(get => new CharacterDataSource(get.Get<IKanjiProperties>(), get.Get<IKanaProperties>()));
            kernel.Bind(get => new CharacterStrokeOrderDataSource());
            kernel.Bind(get => new JMDictDataSource(get.Get<JMDictLookup>(), get.Get<IKanaProperties>()));
            kernel.Bind(get => new JNeDictDataSource(get.Get<JMNedictLookup>()));
            kernel.Bind(get => new VerbConjugationDataSource(get.Get<JMDictLookup>()));
            kernel.Bind(get => new WordFrequencyRatingDataSource(get.Get<FrequencyList>()));
            kernel.Bind(get => new PartialExpressionJMDictDataSource(get.Get<IdiomDetector>()));
            kernel.Bind(get => new JGramDataSource(get.Get<IJGramLookup>()));
            kernel.Bind(get => new AutoGlosserDataSource(get.Get<IAutoGlosser>()));
            kernel.Bind(get => new CustomNotesDataSource(Path.Combine(dataDirectory, "custom", "custom_notes.txt")));
            kernel.Bind(get => new TanakaCorpusFastDataSource(get.Get<Corpus>()));
            kernel.Bind(get => new TanakaCorpusDataSource(get.Get<Tanaka>()));
            kernel.Bind(get => new BasicExpressionCorpusDataSource(get.Get<BasicExpressionsCorpus>()));
            kernel.Bind(get => new PartialWordLookupJMDictDataSource(get.Get<PartialWordLookup>(), get.Get<FrequencyList>()));
            kernel.Bind(get => new JESCDataSource(get.Get<JESC>()));
            kernel.Bind(get => new RomajiDataSource(get.Get<IRomaji>()));

            var epwingDictionaries = CreateEpwing(dataDirectory, cacheDirectory);
            
            ICollection<Func<IReadOnlyKernel, IDataSource>> dataSourceCollection =
                new List<Func<IReadOnlyKernel, IDataSource>>(new Func<IReadOnlyKernel, IDataSource>[]
                {
                    get => get.Get<CharacterDataSource>(),
                    get => get.Get<CharacterStrokeOrderDataSource>(),
                    get => get.Get<JMDictDataSource>(),
                    get => get.Get<JNeDictDataSource>(),
                    get => get.Get<VerbConjugationDataSource>(),
                    get => get.Get<WordFrequencyRatingDataSource>(),
                    get => get.Get<PartialExpressionJMDictDataSource>(),
                    get => get.Get<JGramDataSource>(),
                    get => get.Get<AutoGlosserDataSource>(),
                    get => get.Get<CustomNotesDataSource>(),
                    get => get.Get<TanakaCorpusFastDataSource>(),
                    get => get.Get<TanakaCorpusDataSource>(),
                    get => get.Get<BasicExpressionCorpusDataSource>(),
                    get => get.Get<PartialWordLookupJMDictDataSource>(),
                    get => get.Get<JESCDataSource>(),
                    get => get.Get<RomajiDataSource>(),
                }.Concat(epwingDictionaries.Dictionaries
                    .Select(dict => new Func<IReadOnlyKernel, IDataSource>(
                        get => new EpwingDataSource(dict, get.Get<IKanaProperties>())))));
            
            kernel.Bind<IEnumerable<IDataSource>>(get => dataSourceCollection.Select(factory => factory(get)));
            
            kernel.Bind<IKanaProperties, KanaProperties2>();
            kernel.Bind(get =>
                new KanaProperties2(Path.Combine(dataDirectory, "character", "kana.txt"), Encoding.UTF8));
            kernel.Bind(get =>
                new SimilarKanji(Path.Combine(dataDirectory, "character", "kanji.tgz_similars.ut8"), Encoding.UTF8));
            kernel.Bind(get => new SentenceParser(get.Get<IMorphologicalAnalyzer<IEntry>>(), get.Get<JMDictLookup>()));
            kernel.Bind<ISentenceParser, SentenceParser>();
            kernel.Bind<IRelated>(get =>
                new CompositeRelated(
                    get.Get<KanaProperties2>(),
                    get.Get<EasilyConfusedKana>(),
                    get.Get<SimilarKanji>()));
            kernel.Bind(get => new IdiomDetector(get.Get<JMDictLookup>(),
                get.Get<IMorphologicalAnalyzer<IpadicEntry>>(), Path.Combine(cacheDirectory, "dictionaries", "idioms.cache")));
            kernel.Bind<IKanjiLookupService, KanjiLookupService>();
            kernel.Bind(get => new KanjiLookupService(
                get.Get<KanjiRadicalLookup>(),
                get.Get<IRadicalSearcher>(),
                get.Get<IKanjiProperties>(),
                get.Get<KanjiAliveJapaneseRadicalInformation>(),
                get.Get<RadkfileKanjiAliveCorrelator>(),
                CreateTextRadicalMappings(get.Get<KanjiRadicalLookup>().AllRadicals, get.Get<RadkfileKanjiAliveCorrelator>())));
            kernel.Bind(get => new PartialWordLookup(get.Get<JMDictLookup>(), get.Get<IRadicalSearcher>(),
                get.Get<KanjiRadicalLookup>()));
            kernel.Bind(get =>
            {
                using (var reader = File.OpenText(Path.Combine(dataDirectory, "character", "radkfile1_plus_2_utf8")))
                    return new KanjiRadicalLookup(Radkfile.Parse(reader), get.Get<KanjiDict>());
            });
            kernel.Bind(get => new DisclaimersGetter(Path.Combine(dataDirectory, @"about.txt")));
            kernel.Bind<IJGramLookup, JGramLookup>();
            kernel.Bind(get => new JGramLookup(
                Path.Combine(dataDirectory, "dictionaries", "jgram"),
                Path.Combine(dataDirectory, "dictionaries", "jgram_lookup"),
                Path.Combine(cacheDirectory, "dictionaries", "jgram.cache")));
            kernel.Bind(get =>
                new RadkfileKanjiAliveCorrelator(Path.Combine(dataDirectory, "character",
                    "radkfile_kanjilive_correlation_data.txt")));
            kernel.Bind<IRadicalSearcher, RadicalSearcher>();
            kernel.Bind(get => new RadicalSearcher(
                get.Get<KanjiRadicalLookup>().AllRadicals,
                get.Get<KanjiAliveJapaneseRadicalInformation>(),
                get.Get<RadkfileKanjiAliveCorrelator>()));
            kernel.Bind(get => new Corpus(get.Get<Tanaka>().AllSentences,
                get.Get<IMorphologicalAnalyzer<IpadicEntry>>(), Path.Combine(cacheDirectory, "corpora", "tanaka.cache")));
            kernel.Bind(get => new DataSourceDispatcher(get.Get<IEnumerable<IDataSource>>()));
            kernel.Bind(get => new XmlRichFormattingRenderer());
            kernel.Bind(get => new KanjiAliveJapaneseRadicalInformation(
                KanjiAliveJapaneseRadicalInformation.Parse(
                    Path.Combine(dataDirectory, "character", "japanese-radicals.csv"))));
            return dataSourceCollection;
        }
        
        private static IReadOnlyDictionary<CodePoint, string> CreateTextRadicalMappings(IEnumerable<CodePoint> radicals, IReadOnlyDictionary<int, int> remapper)
        {
            var dict = radicals.ToDictionary(
                r => r,
                r => Char.ConvertFromUtf32(remapper.GetValueOrNone(r.Utf32).ValueOr(r.Utf32)));
            /*var d = new Dictionary<int, int>
            {
                {'化', '⺅'},
                {'刈', '⺉'},
                {'込', '⻌'},
                {'汁', '氵'},
                {'初', '衤'},
                {'尚', '⺌'},
                {'買', '罒'},
                {'犯', '犭'},
                {'忙', '忄'},
                {'礼', '礻'},
                {'个', 131490},
                {'老', '⺹'},
                {'扎', '扌'},
                {'杰', '灬'},
                {'疔', '疒'},
                {'禹', '禸'},
                {'艾', '⺾'},
                //{'邦', '⻏'},
                //{'阡', '⻖'},
                // 并 none available - upside-down ハ
            };*/
            dict[CodePoint.FromInt('邦')] = "邦";
            dict[CodePoint.FromInt('阡')] = "阡";
            dict[CodePoint.FromInt('老')] = "⺹";
            dict[CodePoint.FromInt('并')] = "丷";
            dict[CodePoint.FromInt('乞')] = "𠂉";
            return dict;
        }

        private static EpwingDictionaries CreateEpwing(string dataDirectory, string cacheDirectory)
        {
            var dictionaries = new EpwingDictionaries();
            try
            {
                var dataPath = Path.Combine(dataDirectory, "epwing");
                var zipFiles = new List<ZipFile2>();
                try
                {
                    foreach (var zipFullPath in Directory.EnumerateFiles(dataPath, "*.zip"))
                    {
                        var zipFile = new ZipFile2(zipFullPath);
                        zipFiles.Add(zipFile);
                        var zipFileName = Path.GetFileName(zipFullPath);
                        var dictionary = new YomichanTermDictionary(zipFile, Path.Combine(cacheDirectory, "epwing", zipFileName + ".cache"));
                        dictionaries.Add(dictionary);
                    }
                }
                finally
                {
                    foreach (var zipFile in zipFiles)
                    {
                        zipFile.Dispose();
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
            }
            catch (InvalidOperationException)
            {
            }

            return dictionaries;
        }
    }
}