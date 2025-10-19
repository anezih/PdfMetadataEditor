namespace PdfMetadataEditor.Interops;

public record Lang(string Language, string Code);

public class ScribeRecognitionOptions
{
    /// <summary>
    /// "speed" | "quality"
    /// </summary>
    public string mode { get; set; } = "speed";

    /// <summary>
    /// https://tesseract-ocr.github.io/tessdoc/Data-Files-in-different-versions.html
    /// </summary>
    public List<string> langs { get; set; } = ["tur", "eng"];

    /// <summary>
    /// "lstm" | "legacy" | "combined"
    /// </summary>
    public string modeAdv { get; set; } = "lstm";

    /// <summary>
    /// "conf" | "data" | "none"
    /// </summary>
    public string combineMode { get; set; } = "data";

    /// <summary>
    /// boolean Whether to use the vanilla Tesseract.js model. (optional, default false)
    /// </summary>
    public bool vanillaMode { get; set; } = false;

    /// <summary>
    /// Config params to pass to to Tesseract.js
    /// </summary>
    public object? config { get; set; }

    public static List<string> ValidModes = ["speed", "quality"];
    public static List<string> ValidModeAdv = ["lstm", "legacy", "combined"];
    public static List<string> ValidCombineModes = ["conf", "data", "none"];
    public static List<Lang> ValidLangs = [new Lang("Afrikaans", "afr"),
        new Lang("Amharic", "amh"),
        new Lang("Arabic", "ara"),
        new Lang("Assamese", "asm"),
        new Lang("Azerbaijani", "aze"),
        new Lang("Azerbaijani - Cyrillic", "aze_cyrl"),
        new Lang("Belarusian", "bel"),
        new Lang("Bengali", "ben"),
        new Lang("Tibetan", "bod"),
        new Lang("Bosnian", "bos"),
        new Lang("Bulgarian", "bul"),
        new Lang("Catalan; Valencian", "cat"),
        new Lang("Cebuano", "ceb"),
        new Lang("Czech", "ces"),
        new Lang("Chinese - Simplified", "chi_sim"),
        new Lang("Chinese - Traditional", "chi_tra"),
        new Lang("Cherokee", "chr"),
        new Lang("Welsh", "cym"),
        new Lang("Danish", "dan"),
        new Lang("German", "deu"),
        new Lang("Dzongkha", "dzo"),
        new Lang("Greek, Modern (1453-)", "ell"),
        new Lang("English", "eng"),
        new Lang("English, Middle (1100-1500)", "enm"),
        new Lang("Esperanto", "epo"),
        new Lang("Estonian", "est"),
        new Lang("Basque", "eus"),
        new Lang("Persian", "fas"),
        new Lang("Finnish", "fin"),
        new Lang("French", "fra"),
        new Lang("German Fraktur", "frk"),
        new Lang("French, Middle (ca. 1400-1600)", "frm"),
        new Lang("Irish", "gle"),
        new Lang("Galician", "glg"),
        new Lang("Greek, Ancient (-1453)", "grc"),
        new Lang("Gujarati", "guj"),
        new Lang("Haitian; Haitian Creole", "hat"),
        new Lang("Hebrew", "heb"),
        new Lang("Hindi", "hin"),
        new Lang("Croatian", "hrv"),
        new Lang("Hungarian", "hun"),
        new Lang("Inuktitut", "iku"),
        new Lang("Indonesian", "ind"),
        new Lang("Icelandic", "isl"),
        new Lang("Italian", "ita"),
        new Lang("Italian - Old", "ita_old"),
        new Lang("Javanese", "jav"),
        new Lang("Japanese", "jpn"),
        new Lang("Kannada", "kan"),
        new Lang("Georgian", "kat"),
        new Lang("Georgian - Old", "kat_old"),
        new Lang("Kazakh", "kaz"),
        new Lang("Central Khmer", "khm"),
        new Lang("Kirghiz; Kyrgyz", "kir"),
        new Lang("Korean", "kor"),
        new Lang("Kurdish", "kur"),
        new Lang("Lao", "lao"),
        new Lang("Latin", "lat"),
        new Lang("Latvian", "lav"),
        new Lang("Lithuanian", "lit"),
        new Lang("Malayalam", "mal"),
        new Lang("Marathi", "mar"),
        new Lang("Macedonian", "mkd"),
        new Lang("Maltese", "mlt"),
        new Lang("Malay", "msa"),
        new Lang("Burmese", "mya"),
        new Lang("Nepali", "nep"),
        new Lang("Dutch; Flemish", "nld"),
        new Lang("Norwegian", "nor"),
        new Lang("Oriya", "ori"),
        new Lang("Panjabi; Punjabi", "pan"),
        new Lang("Polish", "pol"),
        new Lang("Portuguese", "por"),
        new Lang("Pushto; Pashto", "pus"),
        new Lang("Romanian; Moldavian; Moldovan", "ron"),
        new Lang("Russian", "rus"),
        new Lang("Sanskrit", "san"),
        new Lang("Sinhala; Sinhalese", "sin"),
        new Lang("Slovak", "slk"),
        new Lang("Slovenian", "slv"),
        new Lang("Spanish; Castilian", "spa"),
        new Lang("Spanish; Castilian - Old", "spa_old"),
        new Lang("Albanian", "sqi"),
        new Lang("Serbian", "srp"),
        new Lang("Serbian - Latin", "srp_latn"),
        new Lang("Swahili", "swa"),
        new Lang("Swedish", "swe"),
        new Lang("Syriac", "syr"),
        new Lang("Tamil", "tam"),
        new Lang("Telugu", "tel"),
        new Lang("Tajik", "tgk"),
        new Lang("Tagalog", "tgl"),
        new Lang("Thai", "tha"),
        new Lang("Tigrinya", "tir"),
        new Lang("Turkish", "tur"),
        new Lang("Uighur; Uyghur", "uig"),
        new Lang("Ukrainian", "ukr"),
        new Lang("Urdu", "urd"),
        new Lang("Uzbek", "uzb"),
        new Lang("Uzbek - Cyrillic", "uzb_cyrl"),
        new Lang("Vietnamese", "vie"),
        new Lang("Yiddish", "yid"),
    ];

    public bool IsValid()
    {
        return ValidModes.Contains(mode) &&
            ValidModeAdv.Contains(modeAdv) &&
            ValidCombineModes.Contains(combineMode) &&
            langs.All(x => ValidLangs.Select(x => x.Code).Contains(x));
    }
}
