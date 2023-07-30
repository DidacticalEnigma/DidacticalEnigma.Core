using System.ComponentModel;

namespace JDict
{
    public enum EdictMisc
    {
        // reserving 0 for an "unknown"
        [Description("architecture term")]
        archit = 1,
        [Description("abbreviation")]
        abbr,
        [Description("archaism")]
        arch,
        [Description("children's language")]
        chn,
        [Description("colloquialism")]
        col,
        [Description("derogatory")]
        derog,
        [Description("familiar language")]
        fam,
        [Description("female term or language")]
        fem,
        [Description("honorific or respectful (sonkeigo) language")]
        hon,
        [Description("humble (kenjougo) language")]
        hum,
        [Description("idiomatic expression")]
        id,
        [Description("manga slang")]
        m_sl,
        [Description("male term or language")]
        male,
        [Description("male slang")]
        male_sl,
        [Description("obsolete term")]
        obs,
        [Description("obscure term")]
        obsc,
        [Description("onomatopoeic or mimetic word")]
        on_mim,
        [Description("poetical term")]
        poet,
        [Description("polite (teineigo) language")]
        pol,
        [Description("proverb")]
        proverb,
        [Description("quotation")]
        quote,
        [Description("rare")]
        rare,
        [Description("sensitive")]
        sens,
        [Description("slang")]
        sl,
        [Description("word usually written using kana alone")]
        uk,
        [Description("yojijukugo")]
        yoji,
        [Description("vulgar expression or word")]
        vulg,
        [Description("jocular, humorous term")]
        joc,
        [Description("character")]
        @char,
        [Description("creature")]
        creat,
        [Description("deity")]
        dei,
        [Description("event")]
        ev,
        [Description("fiction")]
        fict,
        [Description("legend")]
        leg,
        [Description("mythology")]
        myth,
        [Description("object")]
        obj,
        [Description("other")]
        oth,
        [Description("religion")]
        relig,
        [Description("service")]
        serv,
        [Description("rude or X-rated term (not displayed in educational software)")]
        X,
        [Description("document")]
        doc,
        [Description("formal or literary term")]
        form,
        [Description("group")]
        group,
        [Description("Internet slang")]
        net_sl,
        [Description("dated term")]
        dated,
        [Description("historical term")]
        hist,
        [Description("literary or formal term")]
        litf,
        [Description("family or surname")]
        surname,
        [Description("place name")]
        place,
        [Description("unclassified name")]
        unclass,
        [Description("company name")]
        company,
        [Description("product name")]
        product,
        [Description("work of art, literature, music, etc. name")]
        work,
        [Description("full name of a particular person")]
        person,
        [Description("given name or forename, gender not specified")]
        given,
        [Description("railway station")]
        station,
        [Description("organization name")]
        organization,
        [Description("euphemistic")]
        euph,
    }
}