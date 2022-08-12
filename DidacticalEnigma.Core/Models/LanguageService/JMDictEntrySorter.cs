using System.Collections.Generic;
using System.Linq;
using DidacticalEnigma.Core.Models.DataSources;
using JDict;
using Optional;

namespace DidacticalEnigma.Core.Models.LanguageService;

public class JMDictEntrySorter
{
    public IEnumerable<JMDictEntry> Sort(IEnumerable<JMDictEntry> entries, Request request)
    {
        return entries.OrderByDescending(entry =>
        {
            if (request.PartOfSpeech == PartOfSpeech.Particle)
            {
                return entry.Senses.Sum(sense => sense.Type == EdictPartOfSpeech.prt.Some() ? 1 : 0);
            }
            return 0;
        });
    }
}