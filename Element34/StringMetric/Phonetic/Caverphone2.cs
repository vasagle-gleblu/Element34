using System.Text.RegularExpressions;

namespace Element34.StringMetric
{
    public class Caverphone2 : IStringEncoder, IStringComparison
    {
        const string SIX_1 = "111111";
        const int tokenLength = 10;

        public bool Compare(string value1, string value2)
        {
            Caverphone2 cav2 = new Caverphone2();
            value1 = cav2.Encode(value1);
            value2 = cav2.Encode(value2);

            return value1.Equals(value2);
        }

        public string Encode(string source)
        {
            if (source == null || source.Length == 0)
            {
                return SIX_1 + "1111";
            }

            // 1. Convert to lowercase
            // 2. Remove anything not A-Z
            var txt = RegexReplaceG(source.ToLower(), @"[^a-z]", @"");

            // 2.5. Remove final e
            txt = RegexReplaceN(txt, @"e$", @"", 1); // 2.0 only

            // 3. Handle various start options
            txt = RegexReplaceN(txt, @"^cough", @"cou2f", 1);
            txt = RegexReplaceN(txt, @"^rough", @"rou2f", 1);
            txt = RegexReplaceN(txt, @"^tough", @"tou2f", 1);
            txt = RegexReplaceN(txt, @"^enough", @"enou2f", 1); // 2.0 only
            txt = RegexReplaceN(txt, @"^trough", @"trou2f", 1); // 2.0 only - note the spec says ^enough here again, c+p error I assume
            txt = RegexReplaceN(txt, @"^gn", @"2n", 1);

            // 3.5 Handle various ending options
            txt = RegexReplaceN(txt, @"mb$", "m2", 1);

            // 4. Handle replacements
            txt = RegexReplaceG(txt, @"cq", @"2q");
            txt = RegexReplaceG(txt, @"ci", @"si");
            txt = RegexReplaceG(txt, @"ce", @"se");
            txt = RegexReplaceG(txt, @"cy", @"sy");
            txt = RegexReplaceG(txt, @"tch", @"2ch");
            txt = RegexReplaceG(txt, @"c", @"k");
            txt = RegexReplaceG(txt, @"q", @"k");
            txt = RegexReplaceG(txt, @"x", @"k");
            txt = RegexReplaceG(txt, @"v", @"f");
            txt = RegexReplaceG(txt, @"dg", @"2g");
            txt = RegexReplaceG(txt, @"tio", @"sio");
            txt = RegexReplaceG(txt, @"tia", @"sia");
            txt = RegexReplaceG(txt, @"d", @"t");
            txt = RegexReplaceG(txt, @"ph", @"fh");
            txt = RegexReplaceG(txt, @"b", @"p");
            txt = RegexReplaceG(txt, @"sh", @"s2");
            txt = RegexReplaceG(txt, @"z", @"s");
            txt = RegexReplaceG(txt, @"^[aeiou]", @"A");
            txt = RegexReplaceG(txt, @"[aeiou]", @"3");
            txt = RegexReplaceG(txt, @"j", @"y"); // 2.0 only
            txt = RegexReplaceG(txt, @"^y3", @"Y3"); // 2.0 only
            txt = RegexReplaceG(txt, @"^y", @"A"); // 2.0 only
            txt = RegexReplaceG(txt, @"y", @"3"); // 2.0 only
            txt = RegexReplaceG(txt, @"3gh3", @"3kh3");
            txt = RegexReplaceG(txt, @"gh", @"22");
            txt = RegexReplaceG(txt, @"g", @"k");
            txt = RegexReplaceG(txt, @"s+", @"S");
            txt = RegexReplaceG(txt, @"t+", @"T");
            txt = RegexReplaceG(txt, @"p+", @"P");
            txt = RegexReplaceG(txt, @"k+", @"K");
            txt = RegexReplaceG(txt, @"f+", @"F");
            txt = RegexReplaceG(txt, @"m+", @"M");
            txt = RegexReplaceG(txt, @"n+", @"N");
            txt = RegexReplaceG(txt, @"w3", @"W3");
            //txt = RegexReplaceG(txt, @"wy", @"Wy");    // 1.0 only
            txt = RegexReplaceG(txt, @"wh3", @"Wh3");
            txt = RegexReplaceG(txt, @"w$", @"3"); // 2.0 only
            //txt = RegexReplaceG(txt, @"why", @"Why");  // 1.0 only
            txt = RegexReplaceG(txt, @"w", @"2");
            txt = RegexReplaceG(txt, @"^h", @"A");
            txt = RegexReplaceG(txt, @"h", @"2");
            txt = RegexReplaceG(txt, @"r3", @"R3");
            txt = RegexReplaceG(txt, @"r$", @"3"); // 2.0 only
            //txt = RegexReplaceG(txt, @"ry", @"Ry");    // 1.0 only
            txt = RegexReplaceG(txt, @"r", @"2");
            txt = RegexReplaceG(txt, @"l3", @"L3");
            txt = RegexReplaceG(txt, @"l$", @"3"); // 2.0 only
            //txt = RegexReplaceG(txt, @"ly", @"Ly");    // 1.0 only
            txt = RegexReplaceG(txt, @"l", @"2");
            //txt = RegexReplaceG(txt, @"j", @"y");      // 1.0 only
            //txt = RegexReplaceG(txt, @"y3", @"Y3");    // 1.0 only
            //txt = RegexReplaceG(txt, @"y", @"2");      // 1.0 only

            // 5. Handle removals
            txt = RegexReplaceG(txt, @"2", @"");
            txt = RegexReplaceN(txt, @"3$", @"A", 1); // 2.0 only
            txt = RegexReplaceG(txt, @"3", @"");

            // 6. put ten 1s on the end
            txt = txt + SIX_1 + "1111"; // 1.0 only has 6 1s

            // 7. take the first six characters as the code
            return txt.Substring(0, tokenLength); // 1.0 truncates to 6
        }

        private string RegexReplaceN(string input, string pattern, string substitution, int N)
        {
            Regex regex = new Regex(pattern);
            return regex.Replace(input, substitution, N);
        }

        private string RegexReplaceG(string input, string pattern, string substitution)
        {
            Regex regex = new Regex(pattern);
            return regex.Replace(input, substitution);
        }
    }
}
