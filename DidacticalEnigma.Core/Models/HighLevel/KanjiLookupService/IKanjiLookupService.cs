using Optional;

namespace DidacticalEnigma.Core.Models.HighLevel.KanjiLookupService
{
    public interface IKanjiLookupService
    {
        Option<ListRadicalsResult, Error> ListRadicals();

        Option<KanjiLookupResult, Error> SelectRadicals(
            string query,
            string sort,
            string select = null,
            string deselect = null);
    }
}