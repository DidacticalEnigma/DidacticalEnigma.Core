
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Optional;
using Utility.Utils;

namespace JDict
{
    public static class Tatoeba
    {
        public static IEnumerable<TatoebaSentence> ParseSentences(string path)
        {
            using (var reader = File.OpenText(path))
            {
                foreach (var tatoebaSentence in ParseSentences(reader))
                {
                    yield return tatoebaSentence;
                }
            }
        }

        public static IEnumerable<TatoebaSentence> ParseSentences(TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return ParseSentence(line);
            }
        }

        private static TatoebaSentence ParseSentence(string sentenceLine)
        {
            var components = sentenceLine.Split('\t');
            return new TatoebaSentence(
                long.Parse(components[0]),
                components[1],
                components[2]);
        }

        public static IEnumerable<TatoebaSentenceDetailed> ParseSentencesDetailed(string path)
        {
            using (var reader = File.OpenText(path))
            {
                foreach (var tatoebaSentence in ParseSentencesDetailed(reader))
                {
                    yield return tatoebaSentence;
                }
            }
        }

        public static IEnumerable<TatoebaSentenceDetailed> ParseSentencesDetailed(TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return ParseSentenceDetailed(line);
            }
        }

        private static Option<DateTime> ParseTimeOpt(string time)
        {
            return time == "\\N" || time == "0000-00-00 00:00:00"
                ? ParseTime(time).Some()
                : Option.None<DateTime>();
        }

        private static DateTime ParseTime(string time)
        {
            return DateTime.ParseExact(
                time,
                "yyyy-MM-dd hh:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal);
        }

        private static Editability ParseEditability(string input)
        {
            switch (input)
            {
                case "anyone": return Editability.Anyone;
                case "creator": return Editability.Creator;
                case "no_one": return Editability.NoOne;
            }

            throw new ArgumentException($"'{input}' is not valid input value", nameof(input));
        }

        private static TatoebaSentenceDetailed ParseSentenceDetailed(string sentenceLine)
        {
            var components = sentenceLine.Split('\t');
            return new TatoebaSentenceDetailed(
                long.Parse(components[0]),
                components[1],
                components[2],
                components[3],
                ParseTimeOpt(components[4]),
                ParseTimeOpt(components[5]));
        }

        public static IEnumerable<TatoebaLink> ParseLinks(string path)
        {
            using (var reader = File.OpenText(path))
            {
                foreach (var tatoebaLink in ParseLinks(reader))
                {
                    yield return tatoebaLink;
                }
            }
        }

        public static IEnumerable<TatoebaLink> ParseLinks(TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return ParseLink(line);
            }
        }

        private static TatoebaLink ParseLink(string line)
        {
            var components = line.Split('\t');
            return new TatoebaLink(
                long.Parse(components[0]),
                long.Parse(components[1]));
        }

        public static IEnumerable<TatoebaTag> ParseTags(string path)
        {
            using (var reader = File.OpenText(path))
            {
                foreach (var tatoebaTag in ParseTags(reader))
                {
                    yield return tatoebaTag;
                }
            }
        }

        public static IEnumerable<TatoebaTag> ParseTags(TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return ParseTag(line);
            }
        }

        private static TatoebaTag ParseTag(string line)
        {
            var components = line.Split('\t');
            return new TatoebaTag(
                long.Parse(components[0]),
                components[1]);
        }

        public static IEnumerable<TatoebaList> ParseLists(string path)
        {
            using (var reader = File.OpenText(path))
            {
                foreach (var tatoebaList in ParseLists(reader))
                {
                    yield return tatoebaList;
                }
            }
        }

        public static IEnumerable<TatoebaList> ParseLists(TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return ParseList(line);
            }
        }

        private static TatoebaList ParseList(string line)
        {
            var components = line.Split('\t');
            return new TatoebaList(
                long.Parse(components[0]),
                components[1],
                ParseTime(components[2]),
                ParseTime(components[3]),
                components[4],
                ParseEditability(components[5]));
        }

        public static IEnumerable<TatoebaListSentenceLink> ParseListSentenceLinks(string path)
        {
            using (var reader = File.OpenText(path))
            {
                foreach (var tatoebaListSentenceLink in ParseListSentenceLinks(reader))
                {
                    yield return tatoebaListSentenceLink;
                }
            }
        }

        public static IEnumerable<TatoebaListSentenceLink> ParseListSentenceLinks(TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return ParseListSentenceLink(line);
            }
        }

        private static TatoebaListSentenceLink ParseListSentenceLink(string line)
        {
            var components = line.Split('\t');
            return new TatoebaListSentenceLink(
                long.Parse(components[0]),
                long.Parse(components[1]));
        }

        public static IEnumerable<TatoebaJapaneseIndex> ParseJapaneseIndices(string path)
        {
            using (var reader = File.OpenText(path))
            {
                foreach (var japaneseIndex in ParseJapaneseIndices(reader))
                {
                    yield return japaneseIndex;
                }
            }
        }

        public static IEnumerable<TatoebaJapaneseIndex> ParseJapaneseIndices(TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return ParseJapaneseIndex(line);
            }
        }

        private static TatoebaJapaneseIndex ParseJapaneseIndex(string line)
        {
            var components = line.Split('\t');
            return new TatoebaJapaneseIndex(
                long.Parse(components[0]),
                long.Parse(components[1]),
                components[2]);
        }

        public static IEnumerable<TatoebaSentenceAudio> ParseSentenceAudio(string path)
        {
            using (var reader = File.OpenText(path))
            {
                foreach (var sentenceAudio in ParseSentenceAudio(reader))
                {
                    yield return sentenceAudio;
                }
            }
        }

        public static IEnumerable<TatoebaSentenceAudio> ParseSentenceAudio(TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return ParseSentenceAudioLine(line);
            }
        }

        private static TatoebaSentenceAudio ParseSentenceAudioLine(string line)
        {
            var components = line.Split('\t');
            return new TatoebaSentenceAudio(
                long.Parse(components[0]),
                components[1],
                components[2] == "" || components[2] == "\\N" ? License.Of("tatoeba.org use only") : License.Of(components[2]),
                components[3] == "" || components[3] == "\\N" ? Option.None<Uri>() : new Uri(components[3]).Some());
        }
        
        public static IEnumerable<TatoebaUserSkill> ParseUserSkills(string path)
        {
            using (var reader = File.OpenText(path))
            {
                foreach (var userSkill in ParseUserSkills(reader))
                {
                    yield return userSkill;
                }
            }
        }

        public static IEnumerable<TatoebaUserSkill> ParseUserSkills(TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return ParseUserSkill(line);
            }
        }

        private static TatoebaUserSkill ParseUserSkill(string line)
        {
            var components = line.Split('\t');
            return new TatoebaUserSkill(
                components[0],
                components[1] == "\\N" ? Option.None<int>() : int.Parse(components[1]).Some(),
                components[2],
                components[3]);
        }
        
        public static IEnumerable<TatoebaUserSentenceRating> ParseUserSentenceRatings(string path)
        {
            using (var reader = File.OpenText(path))
            {
                foreach (var rating in ParseUserSentenceRatings(reader))
                {
                    yield return rating;
                }
            }
        }

        public static IEnumerable<TatoebaUserSentenceRating> ParseUserSentenceRatings(TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return ParseUserSentenceRating(line);
            }
        }

        private static TatoebaUserSentenceRating ParseUserSentenceRating(string line)
        {
            var components = line.Split('\t');
            return new TatoebaUserSentenceRating(
                components[0],
                components[1],
                long.Parse(components[2]),
                EnumExt.ParseNumericExact<Rating>(components[3]),
                ParseTime(components[4]),
                ParseTime(components[5]));
        }
    }
}
