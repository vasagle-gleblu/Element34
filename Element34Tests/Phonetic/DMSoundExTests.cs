using Element34.StringMetrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Element34Tests.Phonetic
{
    [TestClass]
    public class DMSoundExTests
    {
        private bool DMSoundExTest(string sInput, string sExpected)
        {
            SoundExDM dms = new SoundExDM();
            return dms.Encode(sInput) == sExpected;
        }

        [TestMethod] public void DMSoundExTest001() { Assert.IsTrue(DMSoundExTest("communication", "566536 466536 566436 466436")); }
        [TestMethod] public void DMSoundExTest002() { Assert.IsTrue(DMSoundExTest("worships", "794740")); }
        [TestMethod] public void DMSoundExTest003() { Assert.IsTrue(DMSoundExTest("televisionhome", "387465")); }
        [TestMethod] public void DMSoundExTest004() { Assert.IsTrue(DMSoundExTest("bumi", "760000")); }
        [TestMethod] public void DMSoundExTest005() { Assert.IsTrue(DMSoundExTest("shortbread", "493793")); }
        [TestMethod] public void DMSoundExTest006() { Assert.IsTrue(DMSoundExTest("slice", "485000 484000")); }
        [TestMethod] public void DMSoundExTest007() { Assert.IsTrue(DMSoundExTest("reint", "963000")); }
        [TestMethod] public void DMSoundExTest008() { Assert.IsTrue(DMSoundExTest("Florence", "789650 789640")); }
        [TestMethod] public void DMSoundExTest009() { Assert.IsTrue(DMSoundExTest("fawzi", "774000")); }
        [TestMethod] public void DMSoundExTest010() { Assert.IsTrue(DMSoundExTest("sname", "466000")); }
        [TestMethod] public void DMSoundExTest011() { Assert.IsTrue(DMSoundExTest("modano", "636000")); }
        [TestMethod] public void DMSoundExTest012() { Assert.IsTrue(DMSoundExTest("stumpage", "267500")); }
        [TestMethod] public void DMSoundExTest013() { Assert.IsTrue(DMSoundExTest("clears", "589400 489400")); }
        [TestMethod] public void DMSoundExTest014() { Assert.IsTrue(DMSoundExTest("ciods", "540000 440000")); }
        [TestMethod] public void DMSoundExTest015() { Assert.IsTrue(DMSoundExTest("deseos", "344000")); }
        [TestMethod] public void DMSoundExTest016() { Assert.IsTrue(DMSoundExTest("dippin", "376000")); }
        [TestMethod] public void DMSoundExTest017() { Assert.IsTrue(DMSoundExTest("aranna", "96000")); }
        [TestMethod] public void DMSoundExTest018() { Assert.IsTrue(DMSoundExTest("borse", "794000")); }
        [TestMethod] public void DMSoundExTest019() { Assert.IsTrue(DMSoundExTest("subic", "475000 474000")); }
        [TestMethod] public void DMSoundExTest020() { Assert.IsTrue(DMSoundExTest("Vidrio", "739000")); }
        [TestMethod] public void DMSoundExTest021() { Assert.IsTrue(DMSoundExTest("inserters", "64939")); }
        [TestMethod] public void DMSoundExTest022() { Assert.IsTrue(DMSoundExTest("perature", "793900")); }
        [TestMethod] public void DMSoundExTest023() { Assert.IsTrue(DMSoundExTest("msacideas", "645340 644340")); }
        [TestMethod] public void DMSoundExTest024() { Assert.IsTrue(DMSoundExTest("cougs", "554000 454000")); }
        [TestMethod] public void DMSoundExTest025() { Assert.IsTrue(DMSoundExTest("kittens", "536400")); }
        [TestMethod] public void DMSoundExTest026() { Assert.IsTrue(DMSoundExTest("imperiously", "67948")); }
        [TestMethod] public void DMSoundExTest027() { Assert.IsTrue(DMSoundExTest("boccanegra", "756590 745659 754659 746590")); }
        [TestMethod] public void DMSoundExTest028() { Assert.IsTrue(DMSoundExTest("sextet", "454330")); }
        [TestMethod] public void DMSoundExTest029() { Assert.IsTrue(DMSoundExTest("setattributes", "433973")); }
        [TestMethod] public void DMSoundExTest030() { Assert.IsTrue(DMSoundExTest("dsnt", "463000")); }
        [TestMethod] public void DMSoundExTest031() { Assert.IsTrue(DMSoundExTest("chubby", "570000 470000")); }
        [TestMethod] public void DMSoundExTest032() { Assert.IsTrue(DMSoundExTest("ooda", "30000")); }
        [TestMethod] public void DMSoundExTest033() { Assert.IsTrue(DMSoundExTest("Bressman", "794660")); }
        [TestMethod] public void DMSoundExTest034() { Assert.IsTrue(DMSoundExTest("calendarnew", "586396 486396")); }
        [TestMethod] public void DMSoundExTest035() { Assert.IsTrue(DMSoundExTest("duhon", "356000")); }
        [TestMethod] public void DMSoundExTest036() { Assert.IsTrue(DMSoundExTest("nchen", "656000 646000")); }
        [TestMethod] public void DMSoundExTest037() { Assert.IsTrue(DMSoundExTest("betjemans", "736640 734664")); }
        [TestMethod] public void DMSoundExTest038() { Assert.IsTrue(DMSoundExTest("Mangum", "665600")); }
        [TestMethod] public void DMSoundExTest039() { Assert.IsTrue(DMSoundExTest("cabac", "575000 475000 574000 474000")); }
        [TestMethod] public void DMSoundExTest040() { Assert.IsTrue(DMSoundExTest("pantys", "763400")); }
        [TestMethod] public void DMSoundExTest041() { Assert.IsTrue(DMSoundExTest("cofactors", "575394 475394 574394 474394")); }
        [TestMethod] public void DMSoundExTest042() { Assert.IsTrue(DMSoundExTest("clickz", "585400 485400 584540 484540")); }
        [TestMethod] public void DMSoundExTest043() { Assert.IsTrue(DMSoundExTest("chiap", "570000 470000")); }
        [TestMethod] public void DMSoundExTest044() { Assert.IsTrue(DMSoundExTest("Dalpiaz", "387400")); }
        [TestMethod] public void DMSoundExTest045() { Assert.IsTrue(DMSoundExTest("philosophically", "784758 784748")); }
        [TestMethod] public void DMSoundExTest046() { Assert.IsTrue(DMSoundExTest("backfill", "757800 745780")); }
        [TestMethod] public void DMSoundExTest047() { Assert.IsTrue(DMSoundExTest("Murders", "693940")); }
        [TestMethod] public void DMSoundExTest048() { Assert.IsTrue(DMSoundExTest("ecartis", "059340 049340")); }
        [TestMethod] public void DMSoundExTest049() { Assert.IsTrue(DMSoundExTest("vede", "730000")); }
        [TestMethod] public void DMSoundExTest050() { Assert.IsTrue(DMSoundExTest("usoe", "40000")); }
        [TestMethod] public void DMSoundExTest051() { Assert.IsTrue(DMSoundExTest("ostara", "43900")); }
        [TestMethod] public void DMSoundExTest052() { Assert.IsTrue(DMSoundExTest("labourer", "879900")); }
        [TestMethod] public void DMSoundExTest053() { Assert.IsTrue(DMSoundExTest("speir", "479000")); }
        [TestMethod] public void DMSoundExTest054() { Assert.IsTrue(DMSoundExTest("durchaus", "395400 394400")); }
        [TestMethod] public void DMSoundExTest055() { Assert.IsTrue(DMSoundExTest("inetformfiller", "63796")); }
        [TestMethod] public void DMSoundExTest056() { Assert.IsTrue(DMSoundExTest("itrs", "40000")); }
        [TestMethod] public void DMSoundExTest057() { Assert.IsTrue(DMSoundExTest("Tennant", "366300")); }
        [TestMethod] public void DMSoundExTest058() { Assert.IsTrue(DMSoundExTest("bashore", "749000")); }
        [TestMethod] public void DMSoundExTest059() { Assert.IsTrue(DMSoundExTest("lemurian", "869600")); }
        [TestMethod] public void DMSoundExTest060() { Assert.IsTrue(DMSoundExTest("dungannon", "365660")); }
        [TestMethod] public void DMSoundExTest061() { Assert.IsTrue(DMSoundExTest("epl", "78000")); }
        [TestMethod] public void DMSoundExTest062() { Assert.IsTrue(DMSoundExTest("grader", "593900")); }
        [TestMethod] public void DMSoundExTest063() { Assert.IsTrue(DMSoundExTest("synchronise", "465964 464964")); }
        [TestMethod] public void DMSoundExTest064() { Assert.IsTrue(DMSoundExTest("gondorian", "563960")); }
        [TestMethod] public void DMSoundExTest065() { Assert.IsTrue(DMSoundExTest("cvnet", "576300 476300")); }
        [TestMethod] public void DMSoundExTest066() { Assert.IsTrue(DMSoundExTest("Centers", "563940 463940")); }
        [TestMethod] public void DMSoundExTest067() { Assert.IsTrue(DMSoundExTest("soaks", "454000")); }
        [TestMethod] public void DMSoundExTest068() { Assert.IsTrue(DMSoundExTest("gogllle", "558000")); }
        [TestMethod] public void DMSoundExTest069() { Assert.IsTrue(DMSoundExTest("peotone", "736000")); }
        [TestMethod] public void DMSoundExTest070() { Assert.IsTrue(DMSoundExTest("Sharp", "497000")); }
        [TestMethod] public void DMSoundExTest071() { Assert.IsTrue(DMSoundExTest("mawer", "679000")); }
        [TestMethod] public void DMSoundExTest072() { Assert.IsTrue(DMSoundExTest("Morasca", "694000")); }
        [TestMethod] public void DMSoundExTest073() { Assert.IsTrue(DMSoundExTest("ticknor", "356900 345690")); }
        [TestMethod] public void DMSoundExTest074() { Assert.IsTrue(DMSoundExTest("beaverdale", "779380")); }
        [TestMethod] public void DMSoundExTest075() { Assert.IsTrue(DMSoundExTest("caledonianew", "583667 483667")); }
        [TestMethod] public void DMSoundExTest076() { Assert.IsTrue(DMSoundExTest("wellnessdicke", "786443")); }
        [TestMethod] public void DMSoundExTest077() { Assert.IsTrue(DMSoundExTest("Rodin", "936000")); }
        [TestMethod] public void DMSoundExTest078() { Assert.IsTrue(DMSoundExTest("bluffer", "787900")); }
        [TestMethod] public void DMSoundExTest079() { Assert.IsTrue(DMSoundExTest("joey", "100000 400000")); }
        [TestMethod] public void DMSoundExTest080() { Assert.IsTrue(DMSoundExTest("unbekannter", "67563")); }
        [TestMethod] public void DMSoundExTest081() { Assert.IsTrue(DMSoundExTest("Baylor", "789000")); }
        [TestMethod] public void DMSoundExTest082() { Assert.IsTrue(DMSoundExTest("striplib", "297870")); }
        [TestMethod] public void DMSoundExTest083() { Assert.IsTrue(DMSoundExTest("boquete", "753000")); }
        [TestMethod] public void DMSoundExTest084() { Assert.IsTrue(DMSoundExTest("geylang", "586500")); }
        [TestMethod] public void DMSoundExTest085() { Assert.IsTrue(DMSoundExTest("Lavi", "870000")); }
        [TestMethod] public void DMSoundExTest086() { Assert.IsTrue(DMSoundExTest("moldovans", "683764")); }
        [TestMethod] public void DMSoundExTest087() { Assert.IsTrue(DMSoundExTest("linq", "865000")); }
        [TestMethod] public void DMSoundExTest088() { Assert.IsTrue(DMSoundExTest("Kenworthy", "567930")); }
        [TestMethod] public void DMSoundExTest089() { Assert.IsTrue(DMSoundExTest("Steckman", "256600 245660")); }
        [TestMethod] public void DMSoundExTest090() { Assert.IsTrue(DMSoundExTest("czechoslovakian", "454875 444875")); }
        [TestMethod] public void DMSoundExTest091() { Assert.IsTrue(DMSoundExTest("marmora", "696900")); }
        [TestMethod] public void DMSoundExTest092() { Assert.IsTrue(DMSoundExTest("Carvill", "597800 497800")); }
        [TestMethod] public void DMSoundExTest093() { Assert.IsTrue(DMSoundExTest("chihiro", "559000 459000")); }
        [TestMethod] public void DMSoundExTest094() { Assert.IsTrue(DMSoundExTest("hctc", "540000")); }
        [TestMethod] public void DMSoundExTest095() { Assert.IsTrue(DMSoundExTest("nolde", "683000")); }
        [TestMethod] public void DMSoundExTest096() { Assert.IsTrue(DMSoundExTest("aerosolized", "94843")); }
        [TestMethod] public void DMSoundExTest097() { Assert.IsTrue(DMSoundExTest("canterbury", "563979 463979")); }
        [TestMethod] public void DMSoundExTest098() { Assert.IsTrue(DMSoundExTest("jss", "140000 400000")); }
        [TestMethod] public void DMSoundExTest099() { Assert.IsTrue(DMSoundExTest("rmn", "966000")); }
        [TestMethod] public void DMSoundExTest100() { Assert.IsTrue(DMSoundExTest("mazlish", "648400")); }
        [TestMethod] public void DMSoundExTest101() { Assert.IsTrue(DMSoundExTest("faites", "734000")); }
        [TestMethod] public void DMSoundExTest102() { Assert.IsTrue(DMSoundExTest("foyer", "719000")); }
        [TestMethod] public void DMSoundExTest103() { Assert.IsTrue(DMSoundExTest("ductions", "353640 343640")); }
        [TestMethod] public void DMSoundExTest104() { Assert.IsTrue(DMSoundExTest("Rambo", "967000")); }
        [TestMethod] public void DMSoundExTest105() { Assert.IsTrue(DMSoundExTest("Curnutte", "596300 496300")); }
        [TestMethod] public void DMSoundExTest106() { Assert.IsTrue(DMSoundExTest("Feltus", "783400")); }
        [TestMethod] public void DMSoundExTest107() { Assert.IsTrue(DMSoundExTest("forbairt", "797930")); }
        [TestMethod] public void DMSoundExTest108() { Assert.IsTrue(DMSoundExTest("southerners", "439694")); }
        [TestMethod] public void DMSoundExTest109() { Assert.IsTrue(DMSoundExTest("hypnotists", "576343")); }
        [TestMethod] public void DMSoundExTest110() { Assert.IsTrue(DMSoundExTest("euphony", "176000")); }
        [TestMethod] public void DMSoundExTest111() { Assert.IsTrue(DMSoundExTest("mhpa", "670000")); }
        [TestMethod] public void DMSoundExTest112() { Assert.IsTrue(DMSoundExTest("matteotti", "633000")); }
        [TestMethod] public void DMSoundExTest113() { Assert.IsTrue(DMSoundExTest("Klande", "586300")); }
        [TestMethod] public void DMSoundExTest114() { Assert.IsTrue(DMSoundExTest("Trudgeon", "393560")); }
        [TestMethod] public void DMSoundExTest115() { Assert.IsTrue(DMSoundExTest("schoolhill", "485800")); }
        [TestMethod] public void DMSoundExTest116() { Assert.IsTrue(DMSoundExTest("equalizer", "58490")); }
        [TestMethod] public void DMSoundExTest117() { Assert.IsTrue(DMSoundExTest("xnet", "563000")); }
        [TestMethod] public void DMSoundExTest118() { Assert.IsTrue(DMSoundExTest("Thell", "380000")); }
        [TestMethod] public void DMSoundExTest119() { Assert.IsTrue(DMSoundExTest("Canel", "568000 468000")); }
        [TestMethod] public void DMSoundExTest120() { Assert.IsTrue(DMSoundExTest("vmetro", "763900")); }
        [TestMethod] public void DMSoundExTest121() { Assert.IsTrue(DMSoundExTest("burgtheater", "795339")); }
        [TestMethod] public void DMSoundExTest122() { Assert.IsTrue(DMSoundExTest("Berlinger", "798659")); }
        [TestMethod] public void DMSoundExTest123() { Assert.IsTrue(DMSoundExTest("crehan", "595600 495600")); }
        [TestMethod] public void DMSoundExTest124() { Assert.IsTrue(DMSoundExTest("abysses", "74400")); }
        [TestMethod] public void DMSoundExTest125() { Assert.IsTrue(DMSoundExTest("Hipolito", "578300")); }
        [TestMethod] public void DMSoundExTest126() { Assert.IsTrue(DMSoundExTest("splintered", "478639")); }
        [TestMethod] public void DMSoundExTest127() { Assert.IsTrue(DMSoundExTest("maintenace", "663650 663640")); }
        [TestMethod] public void DMSoundExTest128() { Assert.IsTrue(DMSoundExTest("decency", "356500 346500 356400 346400")); }
        [TestMethod] public void DMSoundExTest129() { Assert.IsTrue(DMSoundExTest("quantz", "564000")); }
        [TestMethod] public void DMSoundExTest130() { Assert.IsTrue(DMSoundExTest("wette", "730000")); }
        [TestMethod] public void DMSoundExTest131() { Assert.IsTrue(DMSoundExTest("Feary", "790000")); }
        [TestMethod] public void DMSoundExTest132() { Assert.IsTrue(DMSoundExTest("Cuchiara", "559000 459000 549000 449000")); }
        [TestMethod] public void DMSoundExTest133() { Assert.IsTrue(DMSoundExTest("Arbon", "97600")); }
        [TestMethod] public void DMSoundExTest134() { Assert.IsTrue(DMSoundExTest("shyness", "464000")); }
        [TestMethod] public void DMSoundExTest135() { Assert.IsTrue(DMSoundExTest("Salin", "486000")); }
        [TestMethod] public void DMSoundExTest136() { Assert.IsTrue(DMSoundExTest("Zicafoose", "457400 447400")); }
        [TestMethod] public void DMSoundExTest137() { Assert.IsTrue(DMSoundExTest("Menn", "660000")); }
        [TestMethod] public void DMSoundExTest138() { Assert.IsTrue(DMSoundExTest("funker", "765900")); }
        [TestMethod] public void DMSoundExTest139() { Assert.IsTrue(DMSoundExTest("santillan", "463860")); }
        [TestMethod] public void DMSoundExTest140() { Assert.IsTrue(DMSoundExTest("punani", "766000")); }
        [TestMethod] public void DMSoundExTest141() { Assert.IsTrue(DMSoundExTest("basa", "740000")); }
        [TestMethod] public void DMSoundExTest142() { Assert.IsTrue(DMSoundExTest("bluefin", "787600")); }
        [TestMethod] public void DMSoundExTest143() { Assert.IsTrue(DMSoundExTest("cc", "500000 450000 540000 400000")); }
        [TestMethod] public void DMSoundExTest144() { Assert.IsTrue(DMSoundExTest("zipgenius", "475640")); }
        [TestMethod] public void DMSoundExTest145() { Assert.IsTrue(DMSoundExTest("Kirchen", "595600 594600")); }
        [TestMethod] public void DMSoundExTest146() { Assert.IsTrue(DMSoundExTest("commentaar", "566390 466390")); }
        [TestMethod] public void DMSoundExTest147() { Assert.IsTrue(DMSoundExTest("expansive", "54764")); }
        [TestMethod] public void DMSoundExTest148() { Assert.IsTrue(DMSoundExTest("castlebar", "543879 443879")); }
        [TestMethod] public void DMSoundExTest149() { Assert.IsTrue(DMSoundExTest("lft", "873000")); }
        [TestMethod] public void DMSoundExTest150() { Assert.IsTrue(DMSoundExTest("robla", "978000")); }
        [TestMethod] public void DMSoundExTest151() { Assert.IsTrue(DMSoundExTest("zarontin", "496360")); }
        [TestMethod] public void DMSoundExTest152() { Assert.IsTrue(DMSoundExTest("nawc", "675000 674000")); }
        [TestMethod] public void DMSoundExTest153() { Assert.IsTrue(DMSoundExTest("resolvconf", "948756 948746")); }
        [TestMethod] public void DMSoundExTest154() { Assert.IsTrue(DMSoundExTest("networker", "637959")); }
        [TestMethod] public void DMSoundExTest155() { Assert.IsTrue(DMSoundExTest("airlins", "98640")); }
        [TestMethod] public void DMSoundExTest156() { Assert.IsTrue(DMSoundExTest("Piskel", "745800")); }
        [TestMethod] public void DMSoundExTest157() { Assert.IsTrue(DMSoundExTest("hilson", "584600")); }
        [TestMethod] public void DMSoundExTest158() { Assert.IsTrue(DMSoundExTest("deco", "350000 340000")); }
        [TestMethod] public void DMSoundExTest159() { Assert.IsTrue(DMSoundExTest("cents", "564000 464000")); }
        [TestMethod] public void DMSoundExTest160() { Assert.IsTrue(DMSoundExTest("award", "79300")); }
        [TestMethod] public void DMSoundExTest161() { Assert.IsTrue(DMSoundExTest("reykjanes", "956400 954640")); }
        [TestMethod] public void DMSoundExTest162() { Assert.IsTrue(DMSoundExTest("perona", "796000")); }
        [TestMethod] public void DMSoundExTest163() { Assert.IsTrue(DMSoundExTest("epistle", "74380")); }
        [TestMethod] public void DMSoundExTest164() { Assert.IsTrue(DMSoundExTest("insound", "64630")); }
        [TestMethod] public void DMSoundExTest165() { Assert.IsTrue(DMSoundExTest("qantas", "563400")); }
        [TestMethod] public void DMSoundExTest166() { Assert.IsTrue(DMSoundExTest("wholey", "758000")); }
        [TestMethod] public void DMSoundExTest167() { Assert.IsTrue(DMSoundExTest("aragona", "95600")); }
        [TestMethod] public void DMSoundExTest168() { Assert.IsTrue(DMSoundExTest("Vancheri", "765900 764900")); }
        [TestMethod] public void DMSoundExTest169() { Assert.IsTrue(DMSoundExTest("allocution", "085360 084360")); }
        [TestMethod] public void DMSoundExTest170() { Assert.IsTrue(DMSoundExTest("volodya", "783000")); }
        [TestMethod] public void DMSoundExTest171() { Assert.IsTrue(DMSoundExTest("enunciate", "066530 066430")); }
        [TestMethod] public void DMSoundExTest172() { Assert.IsTrue(DMSoundExTest("uuuu", "0")); }
        [TestMethod] public void DMSoundExTest173() { Assert.IsTrue(DMSoundExTest("Louato", "830000")); }
        [TestMethod] public void DMSoundExTest174() { Assert.IsTrue(DMSoundExTest("Marquitta", "695300")); }
        [TestMethod] public void DMSoundExTest175() { Assert.IsTrue(DMSoundExTest("surmountable", "496637")); }
        [TestMethod] public void DMSoundExTest176() { Assert.IsTrue(DMSoundExTest("gaultier", "583900")); }
        [TestMethod] public void DMSoundExTest177() { Assert.IsTrue(DMSoundExTest("dafoe", "370000")); }
        [TestMethod] public void DMSoundExTest178() { Assert.IsTrue(DMSoundExTest("Claudio", "583000 483000")); }
        [TestMethod] public void DMSoundExTest179() { Assert.IsTrue(DMSoundExTest("ritva", "937000")); }
        [TestMethod] public void DMSoundExTest180() { Assert.IsTrue(DMSoundExTest("spunky", "476500")); }
        [TestMethod] public void DMSoundExTest181() { Assert.IsTrue(DMSoundExTest("npov", "677000")); }
        [TestMethod] public void DMSoundExTest182() { Assert.IsTrue(DMSoundExTest("eurythmics", "193640")); }
        [TestMethod] public void DMSoundExTest183() { Assert.IsTrue(DMSoundExTest("skittish", "453400")); }
        [TestMethod] public void DMSoundExTest184() { Assert.IsTrue(DMSoundExTest("scooter", "239000")); }
        [TestMethod] public void DMSoundExTest185() { Assert.IsTrue(DMSoundExTest("unfermented", "67966")); }
        [TestMethod] public void DMSoundExTest186() { Assert.IsTrue(DMSoundExTest("kerridge", "593500")); }
        [TestMethod] public void DMSoundExTest187() { Assert.IsTrue(DMSoundExTest("scenography", "265970")); }
        [TestMethod] public void DMSoundExTest188() { Assert.IsTrue(DMSoundExTest("sahalee", "458000")); }
        [TestMethod] public void DMSoundExTest189() { Assert.IsTrue(DMSoundExTest("spooktacular", "475358 475348")); }
        [TestMethod] public void DMSoundExTest190() { Assert.IsTrue(DMSoundExTest("boissons", "746400")); }
        [TestMethod] public void DMSoundExTest191() { Assert.IsTrue(DMSoundExTest("arbete", "97300")); }
        [TestMethod] public void DMSoundExTest192() { Assert.IsTrue(DMSoundExTest("Norrick", "695000 694500")); }
        [TestMethod] public void DMSoundExTest193() { Assert.IsTrue(DMSoundExTest("Sonnier", "469000")); }
        [TestMethod] public void DMSoundExTest194() { Assert.IsTrue(DMSoundExTest("gnomeui", "566100")); }
        [TestMethod] public void DMSoundExTest195() { Assert.IsTrue(DMSoundExTest("milian", "686000")); }
        [TestMethod] public void DMSoundExTest196() { Assert.IsTrue(DMSoundExTest("blackplanetcom", "785786 784578")); }
        [TestMethod] public void DMSoundExTest197() { Assert.IsTrue(DMSoundExTest("tuned", "363000")); }
        [TestMethod] public void DMSoundExTest198() { Assert.IsTrue(DMSoundExTest("Salone", "486000")); }
        [TestMethod] public void DMSoundExTest199() { Assert.IsTrue(DMSoundExTest("marathon", "693600")); }
        [TestMethod] public void DMSoundExTest200() { Assert.IsTrue(DMSoundExTest("layoffs", "817400")); }
        [TestMethod] public void DMSoundExTest201() { Assert.IsTrue(DMSoundExTest("hotek", "535000")); }
        [TestMethod] public void DMSoundExTest202() { Assert.IsTrue(DMSoundExTest("emme", "60000")); }
        [TestMethod] public void DMSoundExTest203() { Assert.IsTrue(DMSoundExTest("amarasinghe", "69465")); }
        [TestMethod] public void DMSoundExTest204() { Assert.IsTrue(DMSoundExTest("midas", "634000")); }
        [TestMethod] public void DMSoundExTest205() { Assert.IsTrue(DMSoundExTest("inhaltsangabe", "65846")); }
        [TestMethod] public void DMSoundExTest206() { Assert.IsTrue(DMSoundExTest("smow", "467000")); }
        [TestMethod] public void DMSoundExTest207() { Assert.IsTrue(DMSoundExTest("policing", "785650 784650")); }
        [TestMethod] public void DMSoundExTest208() { Assert.IsTrue(DMSoundExTest("usmf", "46700")); }
        [TestMethod] public void DMSoundExTest209() { Assert.IsTrue(DMSoundExTest("inferred", "67930")); }
        [TestMethod] public void DMSoundExTest210() { Assert.IsTrue(DMSoundExTest("Eaglen", "58600")); }
        [TestMethod] public void DMSoundExTest211() { Assert.IsTrue(DMSoundExTest("dralle", "398000")); }
        [TestMethod] public void DMSoundExTest212() { Assert.IsTrue(DMSoundExTest("kalmus", "586400")); }
        [TestMethod] public void DMSoundExTest213() { Assert.IsTrue(DMSoundExTest("nork", "695000")); }
        [TestMethod] public void DMSoundExTest214() { Assert.IsTrue(DMSoundExTest("Waterston", "739436")); }
        [TestMethod] public void DMSoundExTest215() { Assert.IsTrue(DMSoundExTest("penalised", "768430")); }
        [TestMethod] public void DMSoundExTest216() { Assert.IsTrue(DMSoundExTest("Lupacchino", "875600 874560 875460 874600")); }
        [TestMethod] public void DMSoundExTest217() { Assert.IsTrue(DMSoundExTest("getgid", "535300")); }
        [TestMethod] public void DMSoundExTest218() { Assert.IsTrue(DMSoundExTest("kever", "579000")); }
        [TestMethod] public void DMSoundExTest219() { Assert.IsTrue(DMSoundExTest("pallavicini", "787560 787460")); }
        [TestMethod] public void DMSoundExTest220() { Assert.IsTrue(DMSoundExTest("Pais", "740000")); }
        [TestMethod] public void DMSoundExTest221() { Assert.IsTrue(DMSoundExTest("youghiogheny", "155600")); }
        [TestMethod] public void DMSoundExTest222() { Assert.IsTrue(DMSoundExTest("Catherina", "539600 439600")); }
        [TestMethod] public void DMSoundExTest223() { Assert.IsTrue(DMSoundExTest("Givant", "576300")); }
        [TestMethod] public void DMSoundExTest224() { Assert.IsTrue(DMSoundExTest("koffi", "570000")); }
        [TestMethod] public void DMSoundExTest225() { Assert.IsTrue(DMSoundExTest("troutbeck", "393750 393745")); }
        [TestMethod] public void DMSoundExTest226() { Assert.IsTrue(DMSoundExTest("moreelse", "698400")); }
        [TestMethod] public void DMSoundExTest227() { Assert.IsTrue(DMSoundExTest("seacrest", "459430 449430")); }
        [TestMethod] public void DMSoundExTest228() { Assert.IsTrue(DMSoundExTest("ufj", "070000 074000")); }
        [TestMethod] public void DMSoundExTest229() { Assert.IsTrue(DMSoundExTest("dereferences", "397965 397964")); }
        [TestMethod] public void DMSoundExTest230() { Assert.IsTrue(DMSoundExTest("sarena", "496000")); }
        [TestMethod] public void DMSoundExTest231() { Assert.IsTrue(DMSoundExTest("Kurt", "593000")); }
        [TestMethod] public void DMSoundExTest232() { Assert.IsTrue(DMSoundExTest("comanche", "566500 466500 566400 466400")); }
        [TestMethod] public void DMSoundExTest233() { Assert.IsTrue(DMSoundExTest("chersoness", "594640 494640")); }
        [TestMethod] public void DMSoundExTest234() { Assert.IsTrue(DMSoundExTest("cd", "530000 430000")); }
        [TestMethod] public void DMSoundExTest235() { Assert.IsTrue(DMSoundExTest("merzky", "645000 694500")); }
        [TestMethod] public void DMSoundExTest236() { Assert.IsTrue(DMSoundExTest("Nordsiek", "694500")); }
        [TestMethod] public void DMSoundExTest237() { Assert.IsTrue(DMSoundExTest("samus", "464000")); }
        [TestMethod] public void DMSoundExTest238() { Assert.IsTrue(DMSoundExTest("tougher", "359000")); }
        [TestMethod] public void DMSoundExTest239() { Assert.IsTrue(DMSoundExTest("monodoc", "663500 663400")); }
        [TestMethod] public void DMSoundExTest240() { Assert.IsTrue(DMSoundExTest("deepings", "376540")); }
        [TestMethod] public void DMSoundExTest241() { Assert.IsTrue(DMSoundExTest("datel", "338000")); }
        [TestMethod] public void DMSoundExTest242() { Assert.IsTrue(DMSoundExTest("Stockett", "253000 245300")); }
        [TestMethod] public void DMSoundExTest243() { Assert.IsTrue(DMSoundExTest("bryum", "796000")); }
        [TestMethod] public void DMSoundExTest244() { Assert.IsTrue(DMSoundExTest("surveyed", "497130")); }
        [TestMethod] public void DMSoundExTest245() { Assert.IsTrue(DMSoundExTest("microcephaly", "659578 649578 659478 649478")); }
        [TestMethod] public void DMSoundExTest246() { Assert.IsTrue(DMSoundExTest("trimoxazole", "396544")); }
        [TestMethod] public void DMSoundExTest247() { Assert.IsTrue(DMSoundExTest("bastide", "743300")); }
        [TestMethod] public void DMSoundExTest248() { Assert.IsTrue(DMSoundExTest("lipo", "870000")); }
        [TestMethod] public void DMSoundExTest249() { Assert.IsTrue(DMSoundExTest("Bosket", "745300")); }
        [TestMethod] public void DMSoundExTest250() { Assert.IsTrue(DMSoundExTest("oroms", "96400")); }
        [TestMethod] public void DMSoundExTest251() { Assert.IsTrue(DMSoundExTest("vandenburg", "763679")); }
        [TestMethod] public void DMSoundExTest252() { Assert.IsTrue(DMSoundExTest("wirtschaftsforschung", "794747")); }
        [TestMethod] public void DMSoundExTest253() { Assert.IsTrue(DMSoundExTest("lilypond", "887630")); }
        [TestMethod] public void DMSoundExTest254() { Assert.IsTrue(DMSoundExTest("mouthfeel", "637800")); }
        [TestMethod] public void DMSoundExTest255() { Assert.IsTrue(DMSoundExTest("wwwunibancocombr", "767655 767645 767654 767644")); }
        [TestMethod] public void DMSoundExTest256() { Assert.IsTrue(DMSoundExTest("Worster", "794390")); }
        [TestMethod] public void DMSoundExTest257() { Assert.IsTrue(DMSoundExTest("Ahmad", "63000")); }
        [TestMethod] public void DMSoundExTest258() { Assert.IsTrue(DMSoundExTest("sweetser", "474900")); }
        [TestMethod] public void DMSoundExTest259() { Assert.IsTrue(DMSoundExTest("vsepr", "747900")); }
        [TestMethod] public void DMSoundExTest260() { Assert.IsTrue(DMSoundExTest("dput", "373000")); }
        [TestMethod] public void DMSoundExTest261() { Assert.IsTrue(DMSoundExTest("austlit", "43830")); }
        [TestMethod] public void DMSoundExTest262() { Assert.IsTrue(DMSoundExTest("Clemen", "586600 486600")); }
        [TestMethod] public void DMSoundExTest263() { Assert.IsTrue(DMSoundExTest("phrva", "797000")); }
        [TestMethod] public void DMSoundExTest264() { Assert.IsTrue(DMSoundExTest("weipa", "770000")); }
        [TestMethod] public void DMSoundExTest265() { Assert.IsTrue(DMSoundExTest("Schrantz", "496400")); }
        [TestMethod] public void DMSoundExTest266() { Assert.IsTrue(DMSoundExTest("meany", "660000")); }
        [TestMethod] public void DMSoundExTest267() { Assert.IsTrue(DMSoundExTest("hodgepodge", "535735")); }
        [TestMethod] public void DMSoundExTest268() { Assert.IsTrue(DMSoundExTest("knuble", "567800")); }
        [TestMethod] public void DMSoundExTest269() { Assert.IsTrue(DMSoundExTest("caniglia", "565800 465800")); }
        [TestMethod] public void DMSoundExTest270() { Assert.IsTrue(DMSoundExTest("blowjobsasics", "787744 787474")); }
        [TestMethod] public void DMSoundExTest271() { Assert.IsTrue(DMSoundExTest("flyjacket", "785300 784530 784453")); }
        [TestMethod] public void DMSoundExTest272() { Assert.IsTrue(DMSoundExTest("galatian", "583600")); }
        [TestMethod] public void DMSoundExTest273() { Assert.IsTrue(DMSoundExTest("hotsel", "548000")); }
        [TestMethod] public void DMSoundExTest274() { Assert.IsTrue(DMSoundExTest("autorun", "39600")); }
        [TestMethod] public void DMSoundExTest275() { Assert.IsTrue(DMSoundExTest("cocoon", "556000 456000 546000 446000")); }
        [TestMethod] public void DMSoundExTest276() { Assert.IsTrue(DMSoundExTest("kooks", "554000")); }
        [TestMethod] public void DMSoundExTest277() { Assert.IsTrue(DMSoundExTest("endocrinol", "063596 063496")); }
        [TestMethod] public void DMSoundExTest278() { Assert.IsTrue(DMSoundExTest("mesages", "645400")); }
        [TestMethod] public void DMSoundExTest279() { Assert.IsTrue(DMSoundExTest("Disney", "346000")); }
        [TestMethod] public void DMSoundExTest280() { Assert.IsTrue(DMSoundExTest("speedline", "473860")); }
        [TestMethod] public void DMSoundExTest281() { Assert.IsTrue(DMSoundExTest("gabaldon", "578360")); }
        [TestMethod] public void DMSoundExTest282() { Assert.IsTrue(DMSoundExTest("margaux", "695540")); }
        [TestMethod] public void DMSoundExTest283() { Assert.IsTrue(DMSoundExTest("housecalls", "545840 544840")); }
        [TestMethod] public void DMSoundExTest284() { Assert.IsTrue(DMSoundExTest("siltation", "483360")); }
        [TestMethod] public void DMSoundExTest285() { Assert.IsTrue(DMSoundExTest("automatic", "036350 036340")); }
        [TestMethod] public void DMSoundExTest286() { Assert.IsTrue(DMSoundExTest("freakshow", "795457")); }
        [TestMethod] public void DMSoundExTest287() { Assert.IsTrue(DMSoundExTest("gorojovsky", "591745")); }
        [TestMethod] public void DMSoundExTest288() { Assert.IsTrue(DMSoundExTest("milwaukie", "687500")); }
        [TestMethod] public void DMSoundExTest289() { Assert.IsTrue(DMSoundExTest("wwwgap", "757000")); }
        [TestMethod] public void DMSoundExTest290() { Assert.IsTrue(DMSoundExTest("autm", "36000")); }
        [TestMethod] public void DMSoundExTest291() { Assert.IsTrue(DMSoundExTest("borromini", "796600")); }
        [TestMethod] public void DMSoundExTest292() { Assert.IsTrue(DMSoundExTest("zpp", "470000")); }
        [TestMethod] public void DMSoundExTest293() { Assert.IsTrue(DMSoundExTest("Penovich", "767500 767400")); }
        [TestMethod] public void DMSoundExTest294() { Assert.IsTrue(DMSoundExTest("domine", "366000")); }
        [TestMethod] public void DMSoundExTest295() { Assert.IsTrue(DMSoundExTest("caves", "574000 474000")); }
        [TestMethod] public void DMSoundExTest296() { Assert.IsTrue(DMSoundExTest("uato", "30000")); }
        [TestMethod] public void DMSoundExTest297() { Assert.IsTrue(DMSoundExTest("tiltonsville", "383647")); }
        [TestMethod] public void DMSoundExTest298() { Assert.IsTrue(DMSoundExTest("Ken", "560000")); }
        [TestMethod] public void DMSoundExTest299() { Assert.IsTrue(DMSoundExTest("Delonais", "386400")); }
        [TestMethod] public void DMSoundExTest300() { Assert.IsTrue(DMSoundExTest("conniption", "567360 467360")); }
        [TestMethod] public void DMSoundExTest301() { Assert.IsTrue(DMSoundExTest("Wenske", "764500")); }
        [TestMethod] public void DMSoundExTest302() { Assert.IsTrue(DMSoundExTest("cautery", "539000 439000")); }
        [TestMethod] public void DMSoundExTest303() { Assert.IsTrue(DMSoundExTest("barbourville", "797978")); }
        [TestMethod] public void DMSoundExTest304() { Assert.IsTrue(DMSoundExTest("departamentos", "379366")); }
        [TestMethod] public void DMSoundExTest305() { Assert.IsTrue(DMSoundExTest("weaponry", "776900")); }
        [TestMethod] public void DMSoundExTest306() { Assert.IsTrue(DMSoundExTest("Haggett", "553000")); }
        [TestMethod] public void DMSoundExTest307() { Assert.IsTrue(DMSoundExTest("martinville", "693678")); }
        [TestMethod] public void DMSoundExTest308() { Assert.IsTrue(DMSoundExTest("stina", "260000")); }
        [TestMethod] public void DMSoundExTest309() { Assert.IsTrue(DMSoundExTest("Rubottom", "973600")); }
        [TestMethod] public void DMSoundExTest310() { Assert.IsTrue(DMSoundExTest("Soyke", "450000")); }
        [TestMethod] public void DMSoundExTest311() { Assert.IsTrue(DMSoundExTest("corse", "594000 494000")); }
        [TestMethod] public void DMSoundExTest312() { Assert.IsTrue(DMSoundExTest("totalling", "338650")); }
        [TestMethod] public void DMSoundExTest313() { Assert.IsTrue(DMSoundExTest("kuppam", "576000")); }
        [TestMethod] public void DMSoundExTest314() { Assert.IsTrue(DMSoundExTest("ftpadmin", "737366")); }
        [TestMethod] public void DMSoundExTest315() { Assert.IsTrue(DMSoundExTest("Forst", "794300")); }
        [TestMethod] public void DMSoundExTest316() { Assert.IsTrue(DMSoundExTest("Ashauer", "47900")); }
        [TestMethod] public void DMSoundExTest317() { Assert.IsTrue(DMSoundExTest("ashtanga", "43650")); }
        [TestMethod] public void DMSoundExTest318() { Assert.IsTrue(DMSoundExTest("alonissos", "86440")); }
        [TestMethod] public void DMSoundExTest319() { Assert.IsTrue(DMSoundExTest("Chimilio", "568000 468000")); }
        [TestMethod] public void DMSoundExTest320() { Assert.IsTrue(DMSoundExTest("deafbazon", "374600")); }
        [TestMethod] public void DMSoundExTest321() { Assert.IsTrue(DMSoundExTest("psmb", "746700")); }
        [TestMethod] public void DMSoundExTest322() { Assert.IsTrue(DMSoundExTest("Ferraraccio", "799500 799450 799540 799400")); }
        [TestMethod] public void DMSoundExTest323() { Assert.IsTrue(DMSoundExTest("fractionated", "795363 794363")); }
        [TestMethod] public void DMSoundExTest324() { Assert.IsTrue(DMSoundExTest("rrd", "930000")); }
        [TestMethod] public void DMSoundExTest325() { Assert.IsTrue(DMSoundExTest("luminosities", "866434")); }
        [TestMethod] public void DMSoundExTest326() { Assert.IsTrue(DMSoundExTest("Platzer", "784900")); }
        [TestMethod] public void DMSoundExTest327() { Assert.IsTrue(DMSoundExTest("modulator", "638390")); }
        [TestMethod] public void DMSoundExTest328() { Assert.IsTrue(DMSoundExTest("tyneside", "364300")); }
        [TestMethod] public void DMSoundExTest329() { Assert.IsTrue(DMSoundExTest("nonuser", "664900")); }
        [TestMethod] public void DMSoundExTest330() { Assert.IsTrue(DMSoundExTest("haspel", "547800")); }
        [TestMethod] public void DMSoundExTest331() { Assert.IsTrue(DMSoundExTest("cortex", "593540 493540")); }
        [TestMethod] public void DMSoundExTest332() { Assert.IsTrue(DMSoundExTest("vechicle", "755800 745800 754800 744800")); }
        [TestMethod] public void DMSoundExTest333() { Assert.IsTrue(DMSoundExTest("epinions", "76640")); }
        [TestMethod] public void DMSoundExTest334() { Assert.IsTrue(DMSoundExTest("wwwusmintgov", "746635")); }
        [TestMethod] public void DMSoundExTest335() { Assert.IsTrue(DMSoundExTest("katies", "534000")); }
        [TestMethod] public void DMSoundExTest336() { Assert.IsTrue(DMSoundExTest("aminosalicylic", "066485 066484")); }
        [TestMethod] public void DMSoundExTest337() { Assert.IsTrue(DMSoundExTest("mmegi", "650000")); }
        [TestMethod] public void DMSoundExTest338() { Assert.IsTrue(DMSoundExTest("sbir", "479000")); }
        [TestMethod] public void DMSoundExTest339() { Assert.IsTrue(DMSoundExTest("learned", "896300")); }
        [TestMethod] public void DMSoundExTest340() { Assert.IsTrue(DMSoundExTest("supris", "479400")); }
        [TestMethod] public void DMSoundExTest341() { Assert.IsTrue(DMSoundExTest("mozarteum", "649360")); }
        [TestMethod] public void DMSoundExTest342() { Assert.IsTrue(DMSoundExTest("selzer", "484900")); }
        [TestMethod] public void DMSoundExTest343() { Assert.IsTrue(DMSoundExTest("tmug", "365000")); }
        [TestMethod] public void DMSoundExTest344() { Assert.IsTrue(DMSoundExTest("Kisto", "543000")); }
        [TestMethod] public void DMSoundExTest345() { Assert.IsTrue(DMSoundExTest("iib", "70000")); }
        [TestMethod] public void DMSoundExTest346() { Assert.IsTrue(DMSoundExTest("wwwincredimail", "765936 764936")); }
        [TestMethod] public void DMSoundExTest347() { Assert.IsTrue(DMSoundExTest("izpack", "047500 047450")); }
        [TestMethod] public void DMSoundExTest348() { Assert.IsTrue(DMSoundExTest("gaymovie", "567000")); }
        [TestMethod] public void DMSoundExTest349() { Assert.IsTrue(DMSoundExTest("Buchholz", "758400 745840")); }
        [TestMethod] public void DMSoundExTest350() { Assert.IsTrue(DMSoundExTest("culcairn", "585960 485960 584960 484960")); }
        [TestMethod] public void DMSoundExTest351() { Assert.IsTrue(DMSoundExTest("barys", "794000")); }
        [TestMethod] public void DMSoundExTest352() { Assert.IsTrue(DMSoundExTest("burets", "794000")); }
        [TestMethod] public void DMSoundExTest353() { Assert.IsTrue(DMSoundExTest("Sorce", "495000 494000")); }
        [TestMethod] public void DMSoundExTest354() { Assert.IsTrue(DMSoundExTest("chimique", "565000 465000")); }
        [TestMethod] public void DMSoundExTest355() { Assert.IsTrue(DMSoundExTest("kitada", "533000")); }
        [TestMethod] public void DMSoundExTest356() { Assert.IsTrue(DMSoundExTest("Engeman", "65660")); }
        [TestMethod] public void DMSoundExTest357() { Assert.IsTrue(DMSoundExTest("raveendran", "976396")); }
        [TestMethod] public void DMSoundExTest358() { Assert.IsTrue(DMSoundExTest("Bendele", "763800")); }
        [TestMethod] public void DMSoundExTest359() { Assert.IsTrue(DMSoundExTest("Pawlitschek", "778450")); }
        [TestMethod] public void DMSoundExTest360() { Assert.IsTrue(DMSoundExTest("hogfather", "557390")); }
        [TestMethod] public void DMSoundExTest361() { Assert.IsTrue(DMSoundExTest("eyedrum", "39600")); }
        [TestMethod] public void DMSoundExTest362() { Assert.IsTrue(DMSoundExTest("Ferrara", "799000")); }
        [TestMethod] public void DMSoundExTest363() { Assert.IsTrue(DMSoundExTest("zhuh", "400000")); }
        [TestMethod] public void DMSoundExTest364() { Assert.IsTrue(DMSoundExTest("zodiak", "435000")); }
        [TestMethod] public void DMSoundExTest365() { Assert.IsTrue(DMSoundExTest("pchcom", "756000 745600 754600 746000")); }
        [TestMethod] public void DMSoundExTest366() { Assert.IsTrue(DMSoundExTest("conferees", "567940 467940")); }
        [TestMethod] public void DMSoundExTest367() { Assert.IsTrue(DMSoundExTest("Decillis", "358400 348400")); }
        [TestMethod] public void DMSoundExTest368() { Assert.IsTrue(DMSoundExTest("downloar", "376890")); }
        [TestMethod] public void DMSoundExTest369() { Assert.IsTrue(DMSoundExTest("shard", "493000")); }
        [TestMethod] public void DMSoundExTest370() { Assert.IsTrue(DMSoundExTest("fajr", "790000")); }
        [TestMethod] public void DMSoundExTest371() { Assert.IsTrue(DMSoundExTest("quisque", "545000")); }
        [TestMethod] public void DMSoundExTest372() { Assert.IsTrue(DMSoundExTest("winterizing", "763946")); }
        [TestMethod] public void DMSoundExTest373() { Assert.IsTrue(DMSoundExTest("referate", "979300")); }
        [TestMethod] public void DMSoundExTest374() { Assert.IsTrue(DMSoundExTest("nuxone", "654600")); }
        [TestMethod] public void DMSoundExTest375() { Assert.IsTrue(DMSoundExTest("Racheal", "958000 948000")); }
        [TestMethod] public void DMSoundExTest376() { Assert.IsTrue(DMSoundExTest("bizben", "747600")); }
        [TestMethod] public void DMSoundExTest377() { Assert.IsTrue(DMSoundExTest("musictalk", "645385 644385")); }
        [TestMethod] public void DMSoundExTest378() { Assert.IsTrue(DMSoundExTest("ogoal", "58000")); }
        [TestMethod] public void DMSoundExTest379() { Assert.IsTrue(DMSoundExTest("Skoff", "457000")); }
        [TestMethod] public void DMSoundExTest380() { Assert.IsTrue(DMSoundExTest("Seehusen", "454600")); }
        [TestMethod] public void DMSoundExTest381() { Assert.IsTrue(DMSoundExTest("Reim", "960000")); }
        [TestMethod] public void DMSoundExTest382() { Assert.IsTrue(DMSoundExTest("Yoh", "100000")); }
        [TestMethod] public void DMSoundExTest383() { Assert.IsTrue(DMSoundExTest("gapi", "570000")); }
        [TestMethod] public void DMSoundExTest384() { Assert.IsTrue(DMSoundExTest("Brightbill", "795378")); }
        [TestMethod] public void DMSoundExTest385() { Assert.IsTrue(DMSoundExTest("shuey", "410000")); }
        [TestMethod] public void DMSoundExTest386() { Assert.IsTrue(DMSoundExTest("posium", "746000")); }
        [TestMethod] public void DMSoundExTest387() { Assert.IsTrue(DMSoundExTest("Falknor", "785690")); }
        [TestMethod] public void DMSoundExTest388() { Assert.IsTrue(DMSoundExTest("ucat", "053000 043000")); }
        [TestMethod] public void DMSoundExTest389() { Assert.IsTrue(DMSoundExTest("properspace", "797947")); }
        [TestMethod] public void DMSoundExTest390() { Assert.IsTrue(DMSoundExTest("showimg", "476500")); }
        [TestMethod] public void DMSoundExTest391() { Assert.IsTrue(DMSoundExTest("Montecillo", "663580 663480")); }
        [TestMethod] public void DMSoundExTest392() { Assert.IsTrue(DMSoundExTest("sajak", "415000")); }
        [TestMethod] public void DMSoundExTest393() { Assert.IsTrue(DMSoundExTest("maturbation", "639736")); }
        [TestMethod] public void DMSoundExTest394() { Assert.IsTrue(DMSoundExTest("Lettie", "830000")); }
        [TestMethod] public void DMSoundExTest395() { Assert.IsTrue(DMSoundExTest("clauss", "584000 484000")); }
        [TestMethod] public void DMSoundExTest396() { Assert.IsTrue(DMSoundExTest("scheiber", "479000")); }
        [TestMethod] public void DMSoundExTest397() { Assert.IsTrue(DMSoundExTest("matp", "637000")); }
        [TestMethod] public void DMSoundExTest398() { Assert.IsTrue(DMSoundExTest("mervtormel", "697396")); }
        [TestMethod] public void DMSoundExTest399() { Assert.IsTrue(DMSoundExTest("guanahani", "565600")); }
        [TestMethod] public void DMSoundExTest400() { Assert.IsTrue(DMSoundExTest("citement", "536630 436630")); }
        [TestMethod] public void DMSoundExTest401() { Assert.IsTrue(DMSoundExTest("immu", "60000")); }
        [TestMethod] public void DMSoundExTest402() { Assert.IsTrue(DMSoundExTest("virulence", "798650 798640")); }
        [TestMethod] public void DMSoundExTest403() { Assert.IsTrue(DMSoundExTest("kussa", "540000")); }
        [TestMethod] public void DMSoundExTest404() { Assert.IsTrue(DMSoundExTest("gso", "540000")); }
        [TestMethod] public void DMSoundExTest405() { Assert.IsTrue(DMSoundExTest("webviajes", "774000 774400")); }
        [TestMethod] public void DMSoundExTest406() { Assert.IsTrue(DMSoundExTest("ldwork", "837950")); }
        [TestMethod] public void DMSoundExTest407() { Assert.IsTrue(DMSoundExTest("verses", "794400")); }
        [TestMethod] public void DMSoundExTest408() { Assert.IsTrue(DMSoundExTest("sondes", "463400")); }
        [TestMethod] public void DMSoundExTest409() { Assert.IsTrue(DMSoundExTest("freshpair", "794790")); }
        [TestMethod] public void DMSoundExTest410() { Assert.IsTrue(DMSoundExTest("licorne", "859600 849600")); }
        [TestMethod] public void DMSoundExTest411() { Assert.IsTrue(DMSoundExTest("Vanis", "764000")); }
        [TestMethod] public void DMSoundExTest412() { Assert.IsTrue(DMSoundExTest("Buena", "760000")); }
        [TestMethod] public void DMSoundExTest413() { Assert.IsTrue(DMSoundExTest("Tauares", "379400")); }
        [TestMethod] public void DMSoundExTest414() { Assert.IsTrue(DMSoundExTest("ikus", "54000")); }
        [TestMethod] public void DMSoundExTest415() { Assert.IsTrue(DMSoundExTest("genrebox", "569754")); }
        [TestMethod] public void DMSoundExTest416() { Assert.IsTrue(DMSoundExTest("rth", "930000")); }
        [TestMethod] public void DMSoundExTest417() { Assert.IsTrue(DMSoundExTest("Spaun", "476000")); }
        [TestMethod] public void DMSoundExTest418() { Assert.IsTrue(DMSoundExTest("wiersbe", "794700")); }
        [TestMethod] public void DMSoundExTest419() { Assert.IsTrue(DMSoundExTest("dad", "330000")); }
        [TestMethod] public void DMSoundExTest420() { Assert.IsTrue(DMSoundExTest("tus", "340000")); }
        [TestMethod] public void DMSoundExTest421() { Assert.IsTrue(DMSoundExTest("Weidert", "739300")); }
        [TestMethod] public void DMSoundExTest422() { Assert.IsTrue(DMSoundExTest("rijke", "950000 945000")); }
        [TestMethod] public void DMSoundExTest423() { Assert.IsTrue(DMSoundExTest("fischbein", "747600")); }
        [TestMethod] public void DMSoundExTest424() { Assert.IsTrue(DMSoundExTest("fbeye", "710000")); }
        [TestMethod] public void DMSoundExTest425() { Assert.IsTrue(DMSoundExTest("hagerty", "559300")); }
        [TestMethod] public void DMSoundExTest426() { Assert.IsTrue(DMSoundExTest("Hollendonner", "586369")); }
        [TestMethod] public void DMSoundExTest427() { Assert.IsTrue(DMSoundExTest("Huitzacua", "545000 544000")); }
        [TestMethod] public void DMSoundExTest428() { Assert.IsTrue(DMSoundExTest("subjunctive", "476537 474653 476437 474643")); }
        [TestMethod] public void DMSoundExTest429() { Assert.IsTrue(DMSoundExTest("implored", "67893")); }
        [TestMethod] public void DMSoundExTest430() { Assert.IsTrue(DMSoundExTest("pompeius", "767140")); }
        [TestMethod] public void DMSoundExTest431() { Assert.IsTrue(DMSoundExTest("afleet", "78300")); }
        [TestMethod] public void DMSoundExTest432() { Assert.IsTrue(DMSoundExTest("iacc", "150000 145000 154000 140000")); }
        [TestMethod] public void DMSoundExTest433() { Assert.IsTrue(DMSoundExTest("sabalos", "478400")); }
        [TestMethod] public void DMSoundExTest434() { Assert.IsTrue(DMSoundExTest("Abdi", "73000")); }
        [TestMethod] public void DMSoundExTest435() { Assert.IsTrue(DMSoundExTest("microinsurance", "659649 649649")); }
        [TestMethod] public void DMSoundExTest436() { Assert.IsTrue(DMSoundExTest("audiobook", "37500")); }
        [TestMethod] public void DMSoundExTest437() { Assert.IsTrue(DMSoundExTest("fulpmes", "787640")); }
        [TestMethod] public void DMSoundExTest438() { Assert.IsTrue(DMSoundExTest("registreren", "954399")); }
        [TestMethod] public void DMSoundExTest439() { Assert.IsTrue(DMSoundExTest("megapath", "657300")); }
        [TestMethod] public void DMSoundExTest440() { Assert.IsTrue(DMSoundExTest("prestidigitation", "794335")); }
        [TestMethod] public void DMSoundExTest441() { Assert.IsTrue(DMSoundExTest("wby", "700000")); }
        [TestMethod] public void DMSoundExTest442() { Assert.IsTrue(DMSoundExTest("vorschau", "794000")); }
        [TestMethod] public void DMSoundExTest443() { Assert.IsTrue(DMSoundExTest("Albertha", "87930")); }
        [TestMethod] public void DMSoundExTest444() { Assert.IsTrue(DMSoundExTest("aspalpha", "47870")); }
        [TestMethod] public void DMSoundExTest445() { Assert.IsTrue(DMSoundExTest("Belden", "783600")); }
        [TestMethod] public void DMSoundExTest446() { Assert.IsTrue(DMSoundExTest("deterministic", "339664")); }
        [TestMethod] public void DMSoundExTest447() { Assert.IsTrue(DMSoundExTest("philosophique", "784750")); }
        [TestMethod] public void DMSoundExTest448() { Assert.IsTrue(DMSoundExTest("kleinen", "586600")); }
        [TestMethod] public void DMSoundExTest449() { Assert.IsTrue(DMSoundExTest("logouri", "859000")); }
        [TestMethod] public void DMSoundExTest450() { Assert.IsTrue(DMSoundExTest("xbl", "578000")); }
        [TestMethod] public void DMSoundExTest451() { Assert.IsTrue(DMSoundExTest("iz", "40000")); }
        [TestMethod] public void DMSoundExTest452() { Assert.IsTrue(DMSoundExTest("throckmorton", "395693 394569")); }
        [TestMethod] public void DMSoundExTest453() { Assert.IsTrue(DMSoundExTest("condesa", "563400 463400")); }
        [TestMethod] public void DMSoundExTest454() { Assert.IsTrue(DMSoundExTest("Mook", "650000")); }
        [TestMethod] public void DMSoundExTest455() { Assert.IsTrue(DMSoundExTest("medseek", "645000")); }
        [TestMethod] public void DMSoundExTest456() { Assert.IsTrue(DMSoundExTest("chugalug", "558500 458500")); }
        [TestMethod] public void DMSoundExTest457() { Assert.IsTrue(DMSoundExTest("olms", "86400")); }
        [TestMethod] public void DMSoundExTest458() { Assert.IsTrue(DMSoundExTest("bogo", "750000")); }
        [TestMethod] public void DMSoundExTest459() { Assert.IsTrue(DMSoundExTest("essing", "46500")); }
        [TestMethod] public void DMSoundExTest460() { Assert.IsTrue(DMSoundExTest("rarden", "993600")); }
        [TestMethod] public void DMSoundExTest461() { Assert.IsTrue(DMSoundExTest("mittlere", "638900")); }
        [TestMethod] public void DMSoundExTest462() { Assert.IsTrue(DMSoundExTest("Borsa", "794000")); }
        [TestMethod] public void DMSoundExTest463() { Assert.IsTrue(DMSoundExTest("Ainslie", "64800")); }
        [TestMethod] public void DMSoundExTest464() { Assert.IsTrue(DMSoundExTest("Ganska", "564500")); }
        [TestMethod] public void DMSoundExTest465() { Assert.IsTrue(DMSoundExTest("miv", "670000")); }
        [TestMethod] public void DMSoundExTest466() { Assert.IsTrue(DMSoundExTest("concessions", "565464 465464 564464 464464")); }
        [TestMethod] public void DMSoundExTest467() { Assert.IsTrue(DMSoundExTest("distinguished", "343654")); }
        [TestMethod] public void DMSoundExTest468() { Assert.IsTrue(DMSoundExTest("cano", "560000 460000")); }
        [TestMethod] public void DMSoundExTest469() { Assert.IsTrue(DMSoundExTest("plumper", "786790")); }
        [TestMethod] public void DMSoundExTest470() { Assert.IsTrue(DMSoundExTest("mindoro", "663900")); }
        [TestMethod] public void DMSoundExTest471() { Assert.IsTrue(DMSoundExTest("cephalopod", "578730 478730")); }
        [TestMethod] public void DMSoundExTest472() { Assert.IsTrue(DMSoundExTest("Klunk", "586500")); }
        [TestMethod] public void DMSoundExTest473() { Assert.IsTrue(DMSoundExTest("Bumbrey", "767900")); }
        [TestMethod] public void DMSoundExTest474() { Assert.IsTrue(DMSoundExTest("causeless", "548400 448400")); }
        [TestMethod] public void DMSoundExTest475() { Assert.IsTrue(DMSoundExTest("Lothrop", "839700")); }
        [TestMethod] public void DMSoundExTest476() { Assert.IsTrue(DMSoundExTest("bodystyles", "734384")); }
        [TestMethod] public void DMSoundExTest477() { Assert.IsTrue(DMSoundExTest("beaatiality", "738300")); }
        [TestMethod] public void DMSoundExTest478() { Assert.IsTrue(DMSoundExTest("Bobrowski", "779745")); }
        [TestMethod] public void DMSoundExTest479() { Assert.IsTrue(DMSoundExTest("erudition", "93360")); }
        [TestMethod] public void DMSoundExTest480() { Assert.IsTrue(DMSoundExTest("huttner", "536900")); }
        [TestMethod] public void DMSoundExTest481() { Assert.IsTrue(DMSoundExTest("wwwaskjeevescom", "745746 745474")); }
        [TestMethod] public void DMSoundExTest482() { Assert.IsTrue(DMSoundExTest("Duross", "394000")); }
        [TestMethod] public void DMSoundExTest483() { Assert.IsTrue(DMSoundExTest("Colemen", "586600 486600")); }
        [TestMethod] public void DMSoundExTest484() { Assert.IsTrue(DMSoundExTest("Duncomb", "365670 364670")); }
        [TestMethod] public void DMSoundExTest485() { Assert.IsTrue(DMSoundExTest("tripe", "397000")); }
        [TestMethod] public void DMSoundExTest486() { Assert.IsTrue(DMSoundExTest("dorrigo", "395000")); }
        [TestMethod] public void DMSoundExTest487() { Assert.IsTrue(DMSoundExTest("Mascall", "648000")); }
        [TestMethod] public void DMSoundExTest488() { Assert.IsTrue(DMSoundExTest("jetplane", "137860 437860")); }
        [TestMethod] public void DMSoundExTest489() { Assert.IsTrue(DMSoundExTest("nswrl", "647980")); }
        [TestMethod] public void DMSoundExTest490() { Assert.IsTrue(DMSoundExTest("didates", "333400")); }
        [TestMethod] public void DMSoundExTest491() { Assert.IsTrue(DMSoundExTest("Jena", "160000 460000")); }
        [TestMethod] public void DMSoundExTest492() { Assert.IsTrue(DMSoundExTest("kctu", "530000 543000")); }
        [TestMethod] public void DMSoundExTest493() { Assert.IsTrue(DMSoundExTest("cualquiera", "585190 485190")); }
        [TestMethod] public void DMSoundExTest494() { Assert.IsTrue(DMSoundExTest("plutella", "783800")); }
        [TestMethod] public void DMSoundExTest495() { Assert.IsTrue(DMSoundExTest("Liebig", "875000")); }
        [TestMethod] public void DMSoundExTest496() { Assert.IsTrue(DMSoundExTest("Llorens", "896400")); }
        [TestMethod] public void DMSoundExTest497() { Assert.IsTrue(DMSoundExTest("mraz", "694000")); }
        [TestMethod] public void DMSoundExTest498() { Assert.IsTrue(DMSoundExTest("shadyac", "435000 434000")); }
        [TestMethod] public void DMSoundExTest499() { Assert.IsTrue(DMSoundExTest("lossphentermine", "847639")); }
        [TestMethod] public void DMSoundExTest500() { Assert.IsTrue(DMSoundExTest("Dreith", "393000")); }

    }
}
