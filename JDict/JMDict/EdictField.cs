using System.ComponentModel;

namespace JDict
{
    public enum EdictField
    {
        // reserving 0 for an "unknown"
        [Description("martial arts term")]
        MA = 1,
        [Description("Buddhist term")]
        Buddh,
        [Description("chemistry term")]
        chem,
        [Description("computer terminology")]
        comp,
        [Description("food term")]
        food,
        [Description("geometry term")]
        geom,
        [Description("linguistics terminology")]
        ling,
        [Description("mathematics")]
        math,
        [Description("military")]
        mil,
        [Description("physics terminology")]
        physics,
        [Description("astronomy, etc. term")]
        astron,
        [Description("baseball term")]
        baseb,
        [Description("biology term")]
        biol,
        [Description("botany term")]
        bot,
        [Description("business term")]
        bus,
        [Description("economics term")]
        econ,
        [Description("engineering term")]
        engr,
        [Description("finance term")]
        finc,
        [Description("geology, etc. term")]
        geol,
        [Description("law, etc. term")]
        law,
        [Description("mahjong term")]
        mahj,
        [Description("medicine, etc. term")]
        med,
        [Description("music term")]
        music,
        [Description("Shinto term")]
        Shinto,
        [Description("shogi term")]
        shogi,
        [Description("sports term")]
        sports,
        [Description("sumo term")]
        sumo,
        [Description("zoology term")]
        zool,
        [Description("anatomical term")]
        anat,
        [Description("Christian term")]
        Christn,
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
        [Description("agriculture")]
        agric,
        [Description("archeology")]
        archeol,
        [Description("art, aesthetics")]
        art,
        [Description("audiovisual")]
        audvid,
        [Description("aviation")]
        aviat,
        [Description("biochemistry")]
        biochem,
        [Description("clothing")]
        cloth,
        [Description("crystallography")]
        cryst,
        [Description("ecology")]
        ecol,
        [Description("electricity, elec. eng.")]
        elec,
        [Description("electronics")]
        electr,
        [Description("embryology")]
        embryo,
        [Description("entomology")]
        ent,
        [Description("fishing")]
        fish,
        [Description("gardening, horticulture")]
        gardn,
        [Description("genetics")]
        genet,
        [Description("geography")]
        geogr,
        [Description("go (game)")]
        go,
        [Description("golf")]
        golf,
        [Description("grammar")]
        gramm,
        [Description("Greek mythology")]
        grmyth,
        [Description("hanafuda")]
        hanaf,
        [Description("horse racing")]
        horse,
        [Description("logic")]
        logic,
        [Description("mechanical engineering")]
        mech,
        [Description("meteorology")]
        met,
        [Description("ornithology")]
        ornith,
        [Description("paleontology")]
        paleo,
        [Description("pathology")]
        pathol,
        [Description("pharmacy")]
        pharm,
        [Description("philosophy")]
        phil,
        [Description("photography")]
        photo,
        [Description("physiology")]
        physiol,
        [Description("printing")]
        print,
        [Description("psychiatry")]
        psy,
        [Description("psychology")]
        psych,
        [Description("railway")]
        rail,
        [Description("statistics")]
        stat,
        [Description("telecommunications")]
        telec,
        [Description("trademark")]
        tradem,
        [Description("video games")]
        vidg,
    }
}