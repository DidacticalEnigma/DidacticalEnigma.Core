using System.ComponentModel;

namespace JDict
{
    // Make sure to synchronize these constants with the values at LibJpConjSharp project
    public enum EdictPartOfSpeech
    {
        [Description("This means no type at all, it is used to return the radical as it is")]
        v0 = 0,

        [Description("Ichidan verb")]
        v1 = 1,

        [Description("Nidan verb with 'u' ending (archaic)")]
        v2a_s = 2,

        [Description("Yodan verb with `hu/fu' ending (archaic)")]
        v4h = 3,

        [Description("Yodan verb with `ru' ending (archaic)")]
        v4r = 4,

        [Description("Godan verb (not completely classified)")]
        v5 = 5,

        [Description("Godan verb - -aru special class")]
        v5aru = 6,

        [Description("Godan verb with `bu' ending")]
        v5b = 7,

        [Description("Godan verb with `gu' ending")]
        v5g = 8,

        [Description("Godan verb with `ku' ending")]
        v5k = 9,

        [Description("Godan verb - Iku/Yuku special class")]
        v5k_s = 10,

        [Description("Godan verb with `mu' ending")]
        v5m = 11,

        [Description("Godan verb with `nu' ending")]
        v5n = 12,

        [Description("Godan verb with `ru' ending")]
        v5r = 13,

        [Description("Godan verb with `ru' ending (irregular verb)")]
        v5r_i = 14,

        [Description("Godan verb with `su' ending")]
        v5s = 15,

        [Description("Godan verb with `tsu' ending")]
        v5t = 16,

        [Description("Godan verb with `u' ending")]
        v5u = 17,

        [Description("Godan verb with `u' ending (special class)")]
        v5u_s = 18,

        [Description("Godan verb - uru old class verb (old form of Eru)")]
        v5uru = 19,

        [Description("Godan verb with `zu' ending")]
        v5z = 20,

        [Description("Ichidan verb - zuru verb (alternative form of -jiru verbs)")]
        vz = 21,

        [Description("Kuru verb - special class")]
        vk = 22,

        [Description("irregular nu verb")]
        vn = 23,

        [Description("noun or participle which takes the aux. verb suru")]
        vs = 24,

        [Description("su verb - precursor to the modern suru")]
        vs_c = 25,

        [Description("suru verb - included")]
        vs_i = 26,

        [Description("suru verb - special class")]
        vs_s = 27,
        
        [Description("Ichidan verb - kureru special class")]
        v1_s = 28,

        [Description("Yodan verb with `ku' ending (archaic)")]
        v4k,
        [Description("Yodan verb with `gu' ending (archaic)")]
        v4g,
        [Description("Yodan verb with `su' ending (archaic)")]
        v4s,
        [Description("Yodan verb with `tsu' ending (archaic)")]
        v4t,
        [Description("Yodan verb with `nu' ending (archaic)")]
        v4n,
        [Description("Yodan verb with `bu' ending (archaic)")]
        v4b,
        [Description("Yodan verb with `mu' ending (archaic)")]
        v4m,
        [Description("Nidan verb (upper class) with `ku' ending (archaic)")]
        v2k_k,
        [Description("Nidan verb (upper class) with `gu' ending (archaic)")]
        v2g_k,
        [Description("Nidan verb (upper class) with `tsu' ending (archaic)")]
        v2t_k,
        [Description("Nidan verb (upper class) with `dzu' ending (archaic)")]
        v2d_k,
        [Description("Nidan verb (upper class) with `hu/fu' ending (archaic)")]
        v2h_k,
        [Description("Nidan verb (upper class) with `bu' ending (archaic)")]
        v2b_k,
        [Description("Nidan verb (upper class) with `mu' ending (archaic)")]
        v2m_k,
        [Description("Nidan verb (upper class) with `yu' ending (archaic)")]
        v2y_k,
        [Description("Nidan verb (upper class) with `ru' ending (archaic)")]
        v2r_k,
        [Description("Nidan verb (lower class) with `ku' ending (archaic)")]
        v2k_s,
        [Description("Nidan verb (lower class) with `gu' ending (archaic)")]
        v2g_s,
        [Description("Nidan verb (lower class) with `su' ending (archaic)")]
        v2s_s,
        [Description("Nidan verb (lower class) with `zu' ending (archaic)")]
        v2z_s,
        [Description("Nidan verb (lower class) with `tsu' ending (archaic)")]
        v2t_s,
        [Description("Nidan verb (lower class) with `dzu' ending (archaic)")]
        v2d_s,
        [Description("Nidan verb (lower class) with `nu' ending (archaic)")]
        v2n_s,
        [Description("Nidan verb (lower class) with `hu/fu' ending (archaic)")]
        v2h_s,
        [Description("Nidan verb (lower class) with `bu' ending (archaic)")]
        v2b_s,
        [Description("Nidan verb (lower class) with `mu' ending (archaic)")]
        v2m_s,
        [Description("Nidan verb (lower class) with `yu' ending (archaic)")]
        v2y_s,
        [Description("Nidan verb (lower class) with `ru' ending (archaic)")]
        v2r_s,
        [Description("Nidan verb (lower class) with `u' ending and `we' conjugation (archaic)")]
        v2w_s,
        [Description("verb unspecified")]
        v_unspec,
        [Description("irregular verb")]
        iv,
        [Description("intransitive verb")]
        vi,
        [Description("irregular ru verb, plain form ends with -ri")]
        vr,
        [Description("transitive verb")]
        vt,

        [Description("noun (common) (futsuumeishi)")]
        n = 128,

        [Description("adverbial noun (fukushitekimeishi)")]
        n_adv = 129,

        [Description("noun, used as a suffix")]
        n_suf = 130,

        [Description("noun, used as a prefix")]
        n_pref = 131,

        [Description("noun (temporal) (jisoumeishi)")]
        n_t = 132,

        [Description("particle")]
        prt = 256,

        [Description("pronoun")]
        pn,

        [Description("pre-noun adjectival (rentaishi)")]
        adj_pn,

        [Description("auxiliary verb")]
        aux_v,

        [Description("copula")]
        cop,

        [Description("adjective (keiyoushi)")]
        adj_i,
        [Description("adjective (keiyoushi) - yoi/ii class")]
        adj_ix,
        [Description("adjectival nouns or quasi-adjectives (keiyodoshi)")]
        adj_na,
        [Description("nouns which may take the genitive case particle `no'")]
        adj_no,
        [Description("`taru' adjective")]
        adj_t,
        [Description("noun or verb acting prenominally")]
        adj_f,
        [Description("adverb (fukushi)")]
        adv,
        [Description("adverb taking the `to' particle")]
        adv_to,
        [Description("auxiliary")]
        aux,
        [Description("auxiliary adjective")]
        aux_adj,
        [Description("conjunction")]
        conj,
        [Description("counter")]
        ctr,
        [Description("expressions (phrases, clauses, etc.)")]
        exp,
        [Description("interjection (kandoushi)")]
        @int,
        [Description("numeric")]
        num,
        [Description("prefix")]
        pref,
        [Description("suffix")]
        suf,
        [Description("unclassified")]
        unc,
        [Description("`kari' adjective (archaic)")]
        adj_kari,
        [Description("`ku' adjective (archaic)")]
        adj_ku,
        [Description("`shiku' adjective (archaic)")]
        adj_shiku,
        [Description("archaic/formal form of na-adjective")]
        adj_nari,
        [Description("proper noun")]
        n_pr,
    }
}