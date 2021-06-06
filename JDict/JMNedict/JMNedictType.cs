using System.ComponentModel;

namespace JDict
{
    public enum JMNedictType
    {
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

        [Description("male given name or forename")]
        masc,

        [Description("female given name or forename")]
        fem,

        [Description("full name of a particular person")]
        person,

        [Description("given name or forename, gender not specified")]
        given,

        [Description("railway station")]
        station,

        [Description("organization name")]
        organization,

        [Description("old or irregular kana form")]
        ok,
        
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
        [Description("document")]
        doc,
        [Description("group")]
        group,
    }
}