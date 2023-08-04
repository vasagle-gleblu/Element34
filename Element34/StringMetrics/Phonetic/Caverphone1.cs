using System.Text.RegularExpressions;

namespace Element34.StringMetrics.Phonetic
{

    /// <summary>
    /// The Caverphone within linguistics and computing, is a phonetic matching algorithm 
    /// invented to identify English names with their sounds, originally built to process 
    /// a custom dataset compound between 1893 and 1938 in southern Dunedin, New Zealand. 
    /// Started from a similar concept as metaphone, it has been developed to accommodate 
    /// and process general English since then.
    /// <a href="https://en.wikipedia.org/wiki/Caverphone" />
    /// </summary>

    public class Caverphone1 : IStringEncoder, IStringComparison
    {
        const string SIX_1 = "111111";
        const int tokenLength = 6;

        /// <summary>
        /// Compares the specified values using Caverphone 1.0 algorithm.
        /// </summary>
        /// <param name="value1">string A</param>
        /// <param name="value2">string B</param>
        /// <returns>Results in true if the encoded input strings match.</returns>
        public bool Compare(string value1, string value2)
        {
            Caverphone1 cav1 = new Caverphone1();
            value1 = cav1.Encode(value1);
            value2 = cav1.Encode(value2);

            return value1.Equals(value2);
        }

        public char[] Encode(char[] buffer)
        {
            return Encode(buffer.ToString()).ToCharArray();
        }

        /// <summary>
        /// Encodes a string with the Caverphone 1.0 specification.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <returns>The encoded string.</returns>
        public string Encode(string source)
        {
            if (source == null || source.Length == 0)
            {
                return SIX_1;
            }

            // 1. Convert to lowercase
            // 2. Remove anything not A-Z
            var txt = RegexReplaceG(source.ToLower(), @"[^a-z]", @"");

            // 3. Handle various start options
            // "2" is a temporary placeholder to indicate a consonant which we are no longer interested in.
            txt = RegexReplaceN(txt, @"^cough", @"cou2f", 1);
            txt = RegexReplaceN(txt, @"^rough", @"rou2f", 1);
            txt = RegexReplaceN(txt, @"^tough", @"tou2f", 1);
            txt = RegexReplaceN(txt, @"^enough", @"enou2f", 1);
            txt = RegexReplaceN(txt, @"^gn", @"2n", 1);

            // End
            txt = RegexReplaceN(txt, @"mb$", @"m2", 1);

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

            // 3 is a temporary placeholder marking a vowel
            txt = RegexReplaceG(txt, @"[aeiou]", @"3");
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
            txt = RegexReplaceG(txt, @"wy", @"Wy"); // 1.0 only
            txt = RegexReplaceG(txt, @"wh3", @"Wh3");
            txt = RegexReplaceG(txt, @"why", @"Why"); // 1.0 only
            txt = RegexReplaceG(txt, @"w", @"2");
            txt = RegexReplaceG(txt, @"^h", @"A");
            txt = RegexReplaceG(txt, @"h", @"2");
            txt = RegexReplaceG(txt, @"r3",@"R3");
            txt = RegexReplaceG(txt, @"ry", @"Ry"); // 1.0 only
            txt = RegexReplaceG(txt, @"r", @"2");
            txt = RegexReplaceG(txt, @"l3", @"L3");
            txt = RegexReplaceG(txt, @"ly", @"Ly"); // 1.0 only
            txt = RegexReplaceG(txt, @"l", @"2");
            txt = RegexReplaceG(txt, @"j", @"y"); // 1.0 only
            txt = RegexReplaceG(txt, @"y3", @"Y3"); // 1.0 only
            txt = RegexReplaceG(txt, @"y", @"2"); // 1.0 only

            // 5. Handle removals
            txt = RegexReplaceG(txt, @"2", @"");
            txt = RegexReplaceG(txt, @"3", @"");

            // 6. put six 1s on the end
            txt = txt + SIX_1;

            // 7. take the first six characters as the code
            return txt.Substring(0, tokenLength);
        }

        /// <summary>
        /// The input string, replaces a specified maximum number of strings that match a regular expression 
        /// pattern with a specified replacement string.  This method is useful for replacing a regular expression match 
        /// if any of the following conditions is true:         
        /// - The replacement string cannot readily be specified by a regular expression replacement pattern.         
        /// - The replacement string results from some processing done on the matched string.     
        /// - The replacement string results from conditional processing.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="substitution">The substitution.</param>
        /// <param name="N">The specified maximum.</param>
        /// <returns>The substitution string</returns>
        private string RegexReplaceN(string input, string pattern, string substitution, int N)
        {
            Regex regex = new Regex(pattern);
            return regex.Replace(input, substitution, N);
        }

        /// <summary>
        /// The input string, replaces all strings that match a regular expression pattern with a specified 
        /// replacement string.  This method is useful for replacing a regular expression match if any of the following 
        /// conditions is true:         
        /// - The replacement string cannot readily be specified by a regular expression replacement pattern.         
        /// - The replacement string results from some processing done on the matched string.     
        /// - The replacement string results from conditional processing.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="substitution">The substitution.</param>
        /// <returns>The substitution string</returns>
        private string RegexReplaceG(string input, string pattern, string substitution)
        {
            Regex regex = new Regex(pattern);
            return regex.Replace(input, substitution);
        }
    }
}
