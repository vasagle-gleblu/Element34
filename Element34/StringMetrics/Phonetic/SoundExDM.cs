using Element34.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Element34.StringMetrics
{
    //**---===WORK IN PROGRESS===---**//
    public class SoundExDM : IStringEncoder, IStringComparison
    {
        /** The code Length of a DM SoundEx value. */
        protected static int MAX_LENGTH = 6;

        /** Whether to use ASCII folding prior to encoding. */
        private bool _folding;

        protected static string _vowels = "aeioujy";

        private static Dictionary<char, char> FOLDINGS = new Dictionary<char, char>() {
          { 'ß', 's' }, { 'à', 'a' }, { 'á', 'a' }, { 'â', 'a' }, { 'ã', 'a' },
          { 'ä', 'a' }, { 'å', 'a' }, { 'æ', 'a' }, { 'ç', 'c' }, { 'è', 'e' },
          { 'é', 'e' }, { 'ê', 'e' }, { 'ë', 'e' }, { 'ì', 'i' }, { 'í', 'i' },
          { 'î', 'i' }, { 'ï', 'i' }, { 'ð', 'd' }, { 'ñ', 'n' }, { 'ò', 'o' },
          { 'ó', 'o' }, { 'ô', 'o' }, { 'õ', 'o' }, { 'ö', 'o' }, { 'ø', 'o' },
          { 'ù', 'u' }, { 'ú', 'u' }, { 'û', 'u' }, { 'ý', 'y' }, { 'ỳ', 'y' },
          { 'þ', 'b' }, { 'ÿ', 'y' }, { 'ć', 'c' }, { 'ł', 'l' }, { 'ś', 's' },
          { 'ż', 'z' }, { 'ź', 'z' }
        };

        private static Dictionary<Pattern, Replacement> RULES = new Dictionary<Pattern, Replacement>() { 
                // Vowels
              { new Pattern("a"), new Replacement("0", "", "") },
              { new Pattern("e"), new Replacement("0", "", "") },
              { new Pattern("i"), new Replacement("0", "", "") },
              { new Pattern("o"), new Replacement("0", "", "") },
              { new Pattern("u"), new Replacement("0", "", "") },
              { new Pattern("y"), new Replacement("1", "", "") },
              { new Pattern("j"), new Replacement("1", "", "") },

              // Consonants
              { new Pattern("b"), new Replacement("7", "7", "7") },
              { new Pattern("d"), new Replacement("3", "3", "3") },
              { new Pattern("f"), new Replacement("7", "7", "7") },
              { new Pattern("g"), new Replacement("5", "5", "5") },
              { new Pattern("h"), new Replacement("5", "5", "")  },
              { new Pattern("k"), new Replacement("5", "5", "5") },
              { new Pattern("l"), new Replacement("8", "8", "8") },
              { new Pattern("m"), new Replacement("6", "6", "6") },
              { new Pattern("n"), new Replacement("6", "6", "6") },
              { new Pattern("p"), new Replacement("7", "7", "7") },
              { new Pattern("q"), new Replacement("5", "5", "5") },
              { new Pattern("r"), new Replacement("9", "9", "9") },
              { new Pattern("s"), new Replacement("4", "4", "4") },
              { new Pattern("t"), new Replacement("3", "3", "3") },
              { new Pattern("v"), new Replacement("7", "7", "7") },
              { new Pattern("w"), new Replacement("7", "7", "7") },
              { new Pattern("x"), new Replacement("5", "54", "54") },
              //{ new Pattern("y"), new Replacement("1", "", "") },
              { new Pattern("z"), new Replacement("4", "4", "4") },

              // Romanian t-cedilla and t-comma should be equivalent
              { new Pattern("ţ"), new Replacement("3|4", "3|4", "3|4")  },
              { new Pattern("ț"), new Replacement("3|4", "3|4", "3|4") },

              // Polish characters (e-ogonek and a-ogonek): default case branch either
              // not coded or 6
              { new Pattern("ę"), new Replacement("", "", "|6") },
              { new Pattern("ą"), new Replacement("", "", "|6") },

              // Other terms
              { new Pattern("schtsch"), new Replacement("2", "4", "4") },
              { new Pattern("schtsh"), new Replacement("2", "4", "4") },
              { new Pattern("schtch"), new Replacement("2", "4", "4") },
              { new Pattern("shtch"), new Replacement("2", "4", "4") },
              { new Pattern("shtsh"), new Replacement("2", "4", "4") },
              { new Pattern("stsch"), new Replacement("2", "4", "4") },
              { new Pattern("ttsch"), new Replacement("4", "4", "4") },
              { new Pattern("zhdzh"), new Replacement("2", "4", "4") },
              { new Pattern("shch"), new Replacement("2", "4", "4") },
              { new Pattern("scht"), new Replacement("2", "43", "43") },
              { new Pattern("schd"), new Replacement("2", "43", "43") },
              { new Pattern("stch"), new Replacement("2", "4", "4") },
              { new Pattern("strz"), new Replacement("2", "4", "4") },
              { new Pattern("strs"), new Replacement("2", "4", "4") },
              { new Pattern("stsh"), new Replacement("2", "4", "4") },
              { new Pattern("szcz"), new Replacement("2", "4", "4") },
              { new Pattern("szcs"), new Replacement("2", "4", "4") },
              { new Pattern("ttch"), new Replacement("4", "4", "4") },
              { new Pattern("tsch"), new Replacement("4", "4", "4") },
              { new Pattern("ttsz"), new Replacement("4", "4", "4") },
              { new Pattern("zdzh"), new Replacement("2", "4", "4") },
              { new Pattern("zsch"), new Replacement("4", "4", "4") },
              { new Pattern("chs"), new Replacement("5", "54", "54") },
              { new Pattern("csz"), new Replacement("4", "4", "4") },
              { new Pattern("czs"), new Replacement("4", "4", "4") },
              { new Pattern("drz"), new Replacement("4", "4", "4") },
              { new Pattern("drs"), new Replacement("4", "4", "4") },
              { new Pattern("dsh"), new Replacement("4", "4", "4") },
              { new Pattern("dsz"), new Replacement("4", "4", "4") },
              { new Pattern("dzh"), new Replacement("4", "4", "4") },
              { new Pattern("dzs"), new Replacement("4", "4", "4") },
              { new Pattern("sch"), new Replacement("4", "4", "4") },
              { new Pattern("sht"), new Replacement("2", "43", "43") },
              { new Pattern("szt"), new Replacement("2", "43", "43") },
              { new Pattern("shd"), new Replacement("2", "43", "43") },
              { new Pattern("szd"), new Replacement("2", "43", "43") },
              { new Pattern("tch"), new Replacement("4", "4", "4") },
              { new Pattern("trz"), new Replacement("4", "4", "4") },
              { new Pattern("trs"), new Replacement("4", "4", "4") },
              { new Pattern("tsh"), new Replacement("4", "4", "4") },
              { new Pattern("tts"), new Replacement("4", "4", "4") },
              { new Pattern("ttz"), new Replacement("4", "4", "4") },
              { new Pattern("tzs"), new Replacement("4", "4", "4") },
              { new Pattern("tsz"), new Replacement("4", "4", "4") },
              { new Pattern("zdz"), new Replacement("2", "4", "4") },
              { new Pattern("zhd"), new Replacement("2", "43", "43") },
              { new Pattern("zsh"), new Replacement("4", "4", "4") },
              { new Pattern("ai"), new Replacement("0", "1", "") },
              { new Pattern("aj"), new Replacement("0", "1", "") },
              { new Pattern("ay"), new Replacement("0", "1", "") },
              { new Pattern("au"), new Replacement("0", "7", "") },
              { new Pattern("cz"), new Replacement("4", "4", "4") },
              { new Pattern("cs"), new Replacement("4", "4", "4") },
              { new Pattern("ds"), new Replacement("4", "4", "4") },
              { new Pattern("dz"), new Replacement("4", "4", "4") },
              { new Pattern("dt"), new Replacement("3", "3", "3") },
              { new Pattern("ei"), new Replacement("0", "1", "") },
              { new Pattern("ej"), new Replacement("0", "1", "") },
              { new Pattern("ey"), new Replacement("0", "1", "") },
              { new Pattern("eu"), new Replacement("1", "1", "") },
              { new Pattern("fb"), new Replacement("7", "7", "7") },
              { new Pattern("ia"), new Replacement("1", "", "") },
              { new Pattern("ie"), new Replacement("1", "", "") },
              { new Pattern("io"), new Replacement("1", "", "") },
              { new Pattern("iu"), new Replacement("1", "", "") },
              { new Pattern("ks"), new Replacement("5", "54", "54") },
              { new Pattern("kh"), new Replacement("5", "5", "5") },
              { new Pattern("mn"), new Replacement("66", "66", "66") },
              { new Pattern("nm"), new Replacement("66", "66", "66") },
              { new Pattern("oi"), new Replacement("0", "1", "") },
              { new Pattern("oj"), new Replacement("0", "1", "") },
              { new Pattern("oy"), new Replacement("0", "1", "") },
              { new Pattern("pf"), new Replacement("7", "7", "7") },
              { new Pattern("ph"), new Replacement("7", "7", "7") },
              { new Pattern("sh"), new Replacement("4", "4", "4") },
              { new Pattern("sc"), new Replacement("2", "4", "4") },
              { new Pattern("st"), new Replacement("2", "43", "43") },
              { new Pattern("sd"), new Replacement("2", "43", "43") },
              { new Pattern("sz"), new Replacement("4", "4", "4") },
              { new Pattern("th"), new Replacement("3", "3", "3") },
              { new Pattern("ts"), new Replacement("4", "4", "4") },
              { new Pattern("tc"), new Replacement("4", "4", "4") },
              { new Pattern("tz"), new Replacement("4", "4", "4") },
              { new Pattern("ui"), new Replacement("0", "1", "") },
              { new Pattern("uj"), new Replacement("0", "1", "") },
              { new Pattern("uy"), new Replacement("0", "1", "") },
              { new Pattern("ue"), new Replacement("0", "1", "") },
              { new Pattern("zd"), new Replacement("2", "43", "43") },
              { new Pattern("zh"), new Replacement("4", "4", "4") },
              { new Pattern("zs"), new Replacement("4", "4", "4") },

              // Branching cases
              { new Pattern("c"), new Replacement("4|5", "4|5", "4|5") },
              { new Pattern("ch"), new Replacement("4|5", "4|5", "4|5") },
              { new Pattern("ck"), new Replacement("5|45", "5|45", "5|45") },
              //{ new Pattern("rs"), new Replacement("4|94", "4|94", "4|94") },
              { new Pattern("rz"), new Replacement("4|94", "4|94", "4|94") },
              //{ new Pattern("j"), new Replacement( "1|4", "|4", "|4") }
            };

        /**
         * Creates a new instance with ASCII-folding enabled.
         */
        public SoundExDM()
        {
            this._folding = true;
        }

        /**
         * Creates a new instance.
         * <p>
         * With ASCII-folding enabled, certain accented characters will be transformed to equivalent
         * ASCII characters, e.g. è -> e.
         * </p>
         *
         * @param folding
         *            if ASCII-folding shall be performed before encoding
         */
        public SoundExDM(bool folding)
        {
            this._folding = folding;
        }

        public bool Compare(string value1, string value2)
        {
            SoundExDM dsm = new SoundExDM();
            value1 = dsm.Encode(value1);
            value2 = dsm.Encode(value2);

            return value1.Equals(value2);
        }

        public string Encode(string source)
        {
            if (source == null)
            {
                return null;
            }
            return SoundEx(source, false)[0];
        }

        /**
         * Encodes a string using the Daitch-Mokotoff SoundEx algorithm with branching.
         * <p>
         * In case a string is encoded into multiple codes (see branching rules), the result will contain all codes,
         * separated by '|'.
         * </p>
         * <p>
         * Example: the name "AUERBACH" is encoded as both
         * </p>
         * <ul>
         * <li>097400</li>
         * <li>097500</li>
         * </ul>
         * <p>
         * Thus the result will be "097400|097500".
         * </p>
         *
         * @param source
         *            A string object to encode
         * @return A string containing a set of DM Soundex codes corresponding to the string supplied
         * @throws IllegalArgumentException
         *             if a character is not mapped
         */
        private string SoundEx(string source)
        {
            string[] branches = SoundEx(source, true);
            StringBuilder sb = new StringBuilder();
            int index = 0;
            foreach (string branch in branches)
            {
                sb.Append(branch);
                if (++index < branches.Length)
                {
                    sb.Append('|');
                }
            }
            return sb.ToString();
        }

        /**
       * Perform the actual DM Soundex algorithm on the input string.
       *
       * @param source
       *            A string object to encode
       * @param branching
       *            If branching shall be performed
       * @return A string array containing all DM Soundex codes corresponding to the string supplied depending on the
       *         selected branching mode
       */
        private string[] SoundEx(string source, bool branching)
        {
            if (source == null)
            {
                return null;
            }

            string input = CleanUp(source);

            List<Branch> currentBranches = new List<Branch>();
            currentBranches.Add(new Branch());

            string lastChar = "\0";
            for (int i = 0; i < input.Length; i++)
            {
                string ch = input[i].ToString();

                // ignore whitespace inside a name
                if (ch.IsWhiteSpace())
                {
                    continue;
                }

                string inputContext = input.Substring(i);
                Replacement rule = RULES[new Pattern(ch)];
                if (rule == null)
                {
                    continue;
                }

                // use an EMPTY_LIST to avoid false positive warnings with potential null pointer access
                List<Branch> nextBranches = branching ? new List<Branch>() : null;

                if (rule.pattern.matches(inputContext))
                {
                    if (branching)
                    {
                        nextBranches.Clear();
                    }
                    string[] replacements = rule.getReplacements(inputContext, lastChar == "\0");
                    bool branchingRequired = replacements.Length > 1 && branching;

                    foreach (Branch branch in currentBranches)
                    {
                        foreach (string nextReplacement in replacements)
                        {
                            // if we have multiple replacements, always create a new branch
                            Branch nextBranch = branchingRequired ? branch.createBranch() : branch;

                            // special rule: occurrences of mn or nm are treated differently
                            bool force = (lastChar == "m" && ch == "n") || (lastChar == "n" && ch == "m");

                            nextBranch.processNextReplacement(nextReplacement, force);

                            if (!branching)
                            {
                                break;
                            }
                            nextBranches.Add(nextBranch);
                        }
                    }

                    if (branching)
                    {
                        currentBranches.Clear();
                        currentBranches.AddRange(nextBranches);
                    }
                    i += rule.pattern.getPatternLength() - 1;
                    break;
                }


                lastChar = ch;
            }

            string[] result = new string[currentBranches.Capacity];
            int index = 0;
            foreach (Branch branch in currentBranches)
            {
                branch.finish();
                result[index++] = branch.ToString();
            }

            return result;
        }

        /**
         * Performs a cleanup of the input string before the actual SoundEx transformation.
         * <p>
         * Removes all whitespace characters and performs ASCII folding if enabled.
         * </p>
         *
         * @param input
         *            the input string to clean up
         * @return a cleaned up string
         */
        private string CleanUp(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char ch in input.ToCharArray())
            {
                if (char.IsWhiteSpace(ch))
                {
                    continue;
                }

                char _ch = char.ToLower(ch);
                char character = FOLDINGS.GetValue(_ch);

                if (_folding && character != '\0')
                {
                    _ch = (char)character;
                }
                sb.Append(_ch);
            }
            return sb.ToString();
        }

        /**
         * Inner class representing a branch during DM SoundEx encoding.
         */
        internal class Branch
        {
            private StringBuilder builder;
            private string cachedstring;
            private string lastReplacement;

            public Branch()
            {
                builder = new StringBuilder();
                lastReplacement = null;
                cachedstring = null;
            }

            /**
             * Creates a new branch, identical to this branch.
             *
             * @return a new, identical branch
             */
            public Branch createBranch()
            {
                Branch branch = new Branch();
                branch.builder.Append(ToString());
                branch.lastReplacement = this.lastReplacement;
                return branch;
            }

            public override bool Equals(object other)
            {
                if (this == other)
                {
                    return true;
                }

                if (other.GetType() != typeof(Branch))
                {
                    return false;
                }

                return ToString().Equals(((Branch)other).ToString());
            }

            /**
             * Finish this branch by appending '0's until the maximum code Length has been reached.
             */
            public void finish()
            {
                while (builder.Length < MAX_LENGTH)
                {
                    builder.Append('0');
                    cachedstring = null;
                }
            }

            public override int GetHashCode()
            {
                return ToString().GetHashCode();
            }

            /**
             * Process the next replacement to be added to this branch.
             *
             * @param replacement
             *            the next replacement to append
             * @param forceAppend
             *            indicates if the default processing shall be overridden
             */
            public void processNextReplacement(string replacement, bool forceAppend)
            {
                bool append = lastReplacement == null || !lastReplacement.EndsWith(replacement) || forceAppend;

                if (append && builder.Length < MAX_LENGTH)
                {
                    builder.Append(replacement);
                    // remove all characters after the maximum Length
                    if (builder.Length > MAX_LENGTH)
                    {
                        builder.Remove(MAX_LENGTH, builder.Length);
                    }
                    cachedstring = null;
                }

                lastReplacement = replacement;
            }

            public override string ToString()
            {
                if (cachedstring == null)
                {
                    cachedstring = builder.ToString();
                }

                return cachedstring;
            }
        }

        internal class Pattern
        {
            internal string _pattern { get; }

            public Pattern(string str)
            {
                _pattern = str;
            }

            public char getPattern(int pos)
            {
                return _pattern[pos];
            }

            public int getPatternLength()
            {
                return _pattern.Length;
            }

            public bool matches(string context)
            {
                return context.StartsWith(_pattern);
            }
        }

        internal class Replacement
        {
            internal Pattern pattern;
            private char VERTICAL_BAR = '|';
            private string[] replacementAtStart;
            private string[] replacementBeforeVowel;
            private string[] replacementDefault;

            public Replacement(string replacementAtStart, string replacementBeforeVowel, string replacementDefault)
            {
                this.replacementAtStart = replacementAtStart.Split(VERTICAL_BAR);
                this.replacementBeforeVowel = replacementBeforeVowel.Split(VERTICAL_BAR);
                this.replacementDefault = replacementDefault.Split(VERTICAL_BAR);
            }

            public string[] getReplacements(string context, bool atStart)
            {
                if (atStart)
                {
                    return replacementAtStart;
                }

                int nextIndex = pattern.getPatternLength();
                bool nextCharIsVowel = nextIndex < context.Length && isVowel(context[nextIndex]);
                if (nextCharIsVowel)
                {
                    return replacementBeforeVowel;
                }

                return replacementDefault;
            }

            private bool isVowel(char ch)
            {
                return _vowels.Contains(ch);
            }

            public override string ToString()
            {
                return string.Format("{0}=({1},{2},{3})", pattern._pattern, replacementAtStart.ToList<string>(), replacementBeforeVowel.ToList<string>(), replacementDefault.ToList<string>());
            }
        }

        /**
         * Inner class for storing rules.
         */
        internal class _Rule
        {
            private string pattern;
            private char VERTICAL_BAR = '|';
            private string[] replacementAtStart;
            private string[] replacementBeforeVowel;
            private string[] replacementDefault;

            public _Rule(string pattern, string replacementAtStart, string replacementBeforeVowel, string replacementDefault)
            {
                this.pattern = pattern;
                this.replacementAtStart = replacementAtStart.Split(VERTICAL_BAR);
                this.replacementBeforeVowel = replacementBeforeVowel.Split(VERTICAL_BAR);
                this.replacementDefault = replacementDefault.Split(VERTICAL_BAR);
            }

            public char getPattern(int pos)
            {
                return pattern[pos];
            }

            public int getPatternLength()
            {
                return pattern.Length;
            }

            public string[] getReplacements(string context, bool atStart)
            {
                if (atStart)
                {
                    return replacementAtStart;
                }

                int nextIndex = getPatternLength();
                bool nextCharIsVowel = nextIndex < context.Length && isVowel(context[nextIndex]);
                if (nextCharIsVowel)
                {
                    return replacementBeforeVowel;
                }

                return replacementDefault;
            }

            private bool isVowel(char ch)
            {
                return _vowels.Contains(ch);
            }

            public bool matches(string context)
            {
                return context.StartsWith(pattern);
            }

            public override string ToString()
            {
                return string.Format("{0}=({1},{2},{3})", pattern, replacementAtStart.ToList<string>(), replacementBeforeVowel.ToList<string>(), replacementDefault.ToList<string>());
            }
        }
    }
}
