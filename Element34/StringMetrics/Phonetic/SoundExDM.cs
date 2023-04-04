using Element34.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Element34.StringMetrics
{
    public class SoundExDM : IStringEncoder, IStringComparison
    {
        /** The code Length of a DM SoundEx value. */
        protected static int MAX_LENGTH = 6;
        protected static char SEPARATOR = ' ';
        protected static char GROUPSEPARATOR = ' ';

        /** Whether to use ASCII folding prior to encoding. */
        private bool _folding;

        protected static char[] _vowels = ((char[])AlphabetSoup.Vowels.getChars()).Append('j').Append('y');

        private static Dictionary<char, char> FOLDINGS = (Dictionary<char, char>)AlphabetSoup.Foldings.getChars();

        private static Dictionary<string, Replacement> RULES = new Dictionary<string, Replacement>() { 
                // Vowels
              { "a", new Replacement(new Pattern("a"), "0", "", "") },
              { "e", new Replacement(new Pattern("e"), "0", "", "") },
              { "i", new Replacement(new Pattern("i"), "0", "", "") },
              { "o", new Replacement(new Pattern("o"), "0", "", "") },
              { "u", new Replacement(new Pattern("u"), "0", "", "") },
              { "y", new Replacement(new Pattern("y"), "1", "", "") },
              { "j", new Replacement(new Pattern("j"), "1", "", "") },

              // Consonants
              { "b", new Replacement(new Pattern("b"), "7", "7", "7") },
              { "d", new Replacement(new Pattern("d"), "3", "3", "3") },
              { "f", new Replacement(new Pattern("f"), "7", "7", "7") },
              { "g", new Replacement(new Pattern("g"), "5", "5", "5") },
              { "h", new Replacement(new Pattern("h"), "5", "5", "")  },
              { "k", new Replacement(new Pattern("k"), "5", "5", "5") },
              { "l", new Replacement(new Pattern("l"), "8", "8", "8") },
              { "m", new Replacement(new Pattern("m"), "6", "6", "6") },
              { "n", new Replacement(new Pattern("n"), "6", "6", "6") },
              { "p", new Replacement(new Pattern("p"), "7", "7", "7") },
              { "q", new Replacement(new Pattern("q"), "5", "5", "5") },
              { "r", new Replacement(new Pattern("r"), "9", "9", "9") },
              { "s", new Replacement(new Pattern("s"), "4", "4", "4") },
              { "t", new Replacement(new Pattern("t"), "3", "3", "3") },
              { "v", new Replacement(new Pattern("v"), "7", "7", "7") },
              { "w", new Replacement(new Pattern("w"), "7", "7", "7") },
              { "x", new Replacement(new Pattern("x"), "5", "54", "54") },
              //{ "y", new Replacement(new Pattern("y"), "1", "", "") },
              { "z", new Replacement(new Pattern("z"), "4", "4", "4") },

              // Romanian t-cedilla and t-comma should be equivalent
              { "ţ", new Replacement(new Pattern("ţ"), "3|4", "3|4", "3|4")  },
              { "ț", new Replacement(new Pattern("ț"), "3|4", "3|4", "3|4") },

              // Polish characters (e-ogonek and a-ogonek): default case branch either
              // not coded or 6
              { "ę", new Replacement(new Pattern("ę"), "", "", "|6") },
              { "ą", new Replacement(new Pattern("ą"), "", "", "|6") },

              // Other terms
              { "schtsch", new Replacement(new Pattern("schtsch"), "2", "4", "4") },
              { "schtsh", new Replacement(new Pattern("schtsh"), "2", "4", "4") },
              { "schtch", new Replacement(new Pattern("schtch"), "2", "4", "4") },
              { "shtch", new Replacement(new Pattern("shtch"), "2", "4", "4") },
              { "shtsh", new Replacement(new Pattern("shtsh"), "2", "4", "4") },
              { "stsch", new Replacement(new Pattern("stsch"), "2", "4", "4") },
              { "ttsch", new Replacement(new Pattern("ttsch"), "4", "4", "4") },
              { "zhdzh", new Replacement(new Pattern("zhdzh"), "2", "4", "4") },
              { "shch", new Replacement(new Pattern("shch"), "2", "4", "4") },
              { "scht", new Replacement(new Pattern("scht"), "2", "43", "43") },
              { "schd", new Replacement(new Pattern("schd"), "2", "43", "43") },
              { "stch", new Replacement(new Pattern("stch"), "2", "4", "4") },
              { "strz", new Replacement(new Pattern("strz"), "2", "4", "4") },
              { "strs", new Replacement(new Pattern("strs"),"2", "4", "4") },
              { "stsh", new Replacement(new Pattern("stsh"), "2", "4", "4") },
              { "szcz", new Replacement(new Pattern("szcz"), "2", "4", "4") },
              { "szcs", new Replacement(new Pattern("szcs"), "2", "4", "4") },
              { "ttch", new Replacement(new Pattern("ttch"), "4", "4", "4") },
              { "tsch", new Replacement(new Pattern("tsch"), "4", "4", "4") },
              { "ttsz", new Replacement(new Pattern("ttsz"), "4", "4", "4") },
              { "zdzh", new Replacement(new Pattern("zdzh"), "2", "4", "4") },
              { "zsch", new Replacement(new Pattern("zsch"), "4", "4", "4") },
              { "chs", new Replacement(new Pattern("chs"), "5", "54", "54") },
              { "csz", new Replacement(new Pattern("csz"), "4", "4", "4") },
              { "czs", new Replacement(new Pattern("czs"), "4", "4", "4") },
              { "drz", new Replacement(new Pattern("drz"), "4", "4", "4") },
              { "drs", new Replacement(new Pattern("drs"), "4", "4", "4") },
              { "dsh", new Replacement(new Pattern("dsh"), "4", "4", "4") },
              { "dsz", new Replacement(new Pattern("dsz"), "4", "4", "4") },
              { "dzh", new Replacement(new Pattern("dzh"), "4", "4", "4") },
              { "dzs", new Replacement(new Pattern("dzs"), "4", "4", "4") },
              { "sch", new Replacement(new Pattern("sch"), "4", "4", "4") },
              { "sht", new Replacement(new Pattern("sht"), "2", "43", "43") },
              { "szt", new Replacement(new Pattern("szt"), "2", "43", "43") },
              { "shd", new Replacement(new Pattern("shd"), "2", "43", "43") },
              { "szd", new Replacement(new Pattern("szd"), "2", "43", "43") },
              { "tch", new Replacement(new Pattern("tch"), "4", "4", "4") },
              { "trz", new Replacement(new Pattern("trz"), "4", "4", "4") },
              { "trs", new Replacement(new Pattern("trs"), "4", "4", "4") },
              { "tsh", new Replacement(new Pattern("tsh"), "4", "4", "4") },
              { "tts", new Replacement(new Pattern("tts"), "4", "4", "4") },
              { "ttz", new Replacement(new Pattern("ttz"), "4", "4", "4") },
              { "tzs", new Replacement(new Pattern("tzs"), "4", "4", "4") },
              { "tsz", new Replacement(new Pattern("tsz"), "4", "4", "4") },
              { "zdz", new Replacement(new Pattern("zdz"), "2", "4", "4") },
              { "zhd", new Replacement(new Pattern("zhd"), "2", "43", "43") },
              { "zsh", new Replacement(new Pattern("zsh"), "4", "4", "4") },
              { "ai", new Replacement(new Pattern("ai"), "0", "1", "") },
              { "aj", new Replacement(new Pattern("aj"), "0", "1", "") },
              { "ay", new Replacement(new Pattern("ay"), "0", "1", "") },
              { "au", new Replacement(new Pattern("au"), "0", "7", "") },
              { "cz", new Replacement(new Pattern("cz"), "4", "4", "4") },
              { "cs", new Replacement(new Pattern("cs"), "4", "4", "4") },
              { "ds", new Replacement(new Pattern("ds"), "4", "4", "4") },
              { "dz", new Replacement(new Pattern("dz"), "4", "4", "4") },
              { "dt", new Replacement(new Pattern("dt"), "3", "3", "3") },
              { "ei", new Replacement(new Pattern("ei"), "0", "1", "") },
              { "ej", new Replacement(new Pattern("ej"), "0", "1", "") },
              { "ey", new Replacement(new Pattern("ey"), "0", "1", "") },
              { "eu", new Replacement(new Pattern("eu"), "1", "1", "") },
              { "fb", new Replacement(new Pattern("fb"), "7", "7", "7") },
              { "ia", new Replacement(new Pattern("ia"), "1", "", "") },
              { "ie", new Replacement(new Pattern("ie"), "1", "", "") },
              { "io", new Replacement(new Pattern("io"), "1", "", "") },
              { "iu", new Replacement(new Pattern("iu"), "1", "", "") },
              { "ks", new Replacement(new Pattern("ks"), "5", "54", "54") },
              { "kh", new Replacement(new Pattern("kh"), "5", "5", "5") },
              { "mn", new Replacement(new Pattern("mn"), "66", "66", "66") },
              { "nm", new Replacement(new Pattern("nm"), "66", "66", "66") },
              { "oi", new Replacement(new Pattern("oi"), "0", "1", "") },
              { "oj", new Replacement(new Pattern("oj"), "0", "1", "") },
              { "oy", new Replacement(new Pattern("oy"), "0", "1", "") },
              { "pf", new Replacement(new Pattern("pf"), "7", "7", "7") },
              { "ph", new Replacement(new Pattern("ph"), "7", "7", "7") },
              { "sh", new Replacement(new Pattern("sh"), "4", "4", "4") },
              { "sc", new Replacement(new Pattern("sc"), "2", "4", "4") },
              { "st", new Replacement(new Pattern("st"), "2", "43", "43") },
              { "sd", new Replacement(new Pattern("sd"), "2", "43", "43") },
              { "sz", new Replacement(new Pattern("sz"), "4", "4", "4") },
              { "th", new Replacement(new Pattern("th"), "3", "3", "3") },
              { "ts", new Replacement(new Pattern("ts"), "4", "4", "4") },
              { "tc", new Replacement(new Pattern("tc"), "4", "4", "4") },
              { "tz", new Replacement(new Pattern("tz"), "4", "4", "4") },
              { "ui", new Replacement(new Pattern("ui"), "0", "1", "") },
              { "uj", new Replacement(new Pattern("uj"), "0", "1", "") },
              { "uy", new Replacement(new Pattern("uy"), "0", "1", "") },
              { "ue", new Replacement(new Pattern("ue"), "0", "1", "") },
              { "zd", new Replacement(new Pattern("zd"), "2", "43", "43") },
              { "zh", new Replacement(new Pattern("zh"), "4", "4", "4") },
              { "zs", new Replacement(new Pattern("zs"), "4", "4", "4") },

              // Branching cases
              { "c", new Replacement(new Pattern("c"), "4|5", "4|5", "4|5") },
              { "ch", new Replacement(new Pattern("ch"), "4|5", "4|5", "4|5") },
              { "ck", new Replacement(new Pattern("ck"), "5|45", "5|45", "5|45") },
              //{ "rs", new Replacement(new Pattern("rs"), "4|94", "4|94", "4|94") },
              { "rz", new Replacement(new Pattern("rz"), "4|94", "4|94", "4|94") },
              //{ "j", new Replacement(new Pattern("j"), "1|4", "|4", "|4") }
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
       * Perform the actual DM Soundex algorithm on the beta string.
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

            string beta = CleanUp(source);

            List<Branch> currentBranches = new List<Branch>();
            currentBranches.Add(new Branch());

            string lastChar = "\0";
            for (int i = 0; i < beta.Length; i++)
            {
                string ch = beta[i].ToString();

                // ignore whitespace inside a name
                if (ch.IsWhiteSpace())
                {
                    continue;
                }

                string betaContext = beta.Substring(i);
                Replacement rule = RULES.GetValue(ch);
                if (rule == null)
                {
                    continue;
                }

                // use an EMPTY_LIST to avoid false positive warnings with potential null pointer access
                List<Branch> nextBranches = branching ? new List<Branch>() : null;

                if (rule._pattern.matches(betaContext))
                {
                    if (branching)
                    {
                        nextBranches.Clear();
                    }
                    string[] replacements = rule.getReplacements(betaContext, lastChar == "\0");
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
                    i += rule._pattern.getPatternLength() - 1;
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
         * Performs a cleanup of the beta string before the actual SoundEx transformation.
         * <p>
         * Removes all whitespace characters and performs ASCII folding if enabled.
         * </p>
         *
         * @param beta
         *            the beta string to clean up
         * @return a cleaned up string
         */
        private string CleanUp(string beta)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char ch in beta.ToCharArray())
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

        internal class Branch
        {
            private StringBuilder builder;
            private string cachedString;
            private string lastReplacement;

            public Branch()
            {
                builder = new StringBuilder();
                lastReplacement = null;
                cachedString = null;
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
                    cachedString = null;
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
                    cachedString = null;
                }

                lastReplacement = replacement;
            }

            public override string ToString()
            {
                if (cachedString == null)
                {
                    cachedString = builder.ToString();
                }

                return cachedString;
            }
        }

        internal class Pattern
        {
            private string _pattern;

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

            public override string ToString()
            {
                return _pattern;
            }
        }

        internal class Replacement
        {
            internal Pattern _pattern;
            private char VERTICAL_BAR = '|';
            private string[] replacementAtStart;
            private string[] replacementBeforeVowel;
            private string[] replacementDefault;

            public Replacement(Pattern pattern, string replacementAtStart, string replacementBeforeVowel, string replacementDefault)
            {
                this._pattern = pattern;
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

                int nextIndex = _pattern.getPatternLength();
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
                return string.Format("{0}=({1},{2},{3})", _pattern.ToString(), replacementAtStart.ToList<string>(), replacementBeforeVowel.ToList<string>(), replacementDefault.ToList<string>());
            }
        }

        private string process(string sInput)
        {
            string alpha = sInput.ToLower();
            string beta = sInput;

            string dm3 = "", tmp, key, xr;
            bool allblank;
            int len, dim_dm2, first;
            char[] dm2, lastdm;

            //** beta.Length > 0 **//
            while (beta.Length > 0)
            {
                tmp = "";
                len = beta.Length;
                int i;

                for (i = 0; i < beta.Length; i++)
                {
                    if ((beta[i] >= 'a' && beta[i] <= 'z') || beta[i] == '/')
                    {
                        if (beta[i] == '/')
                        {
                            beta = beta.Substring(i + 1);
                            break;
                        }
                        else
                        {
                            tmp = tmp + beta[i];
                        }
                    }
                    else
                    {
                        if (alpha[i] == '(' || alpha[i] == SEPARATOR)
                        {
                            break;
                        }
                    }
                }

                if (i == len)
                {
                    beta = ""; // finished
                }

                alpha = tmp;
                key = "";
                allblank = true;
                for (int k = 0; k < alpha.Length; k++)
                {
                    if (alpha[k] != ' ')
                    {
                        allblank = false;
                        break;
                    }
                }

                //** NOT ALLBLANK**//
                if(!allblank)
                {
                    dim_dm2 = 1;
                    dm2 = new char[16];
                    dm2[0] = '\0';

                    first = 1;
                    lastdm = new char[16];
                    lastdm[0] = '\0';

                    /** alpha.Length > 0 **/
                    while (alpha.Length > 0)
                    {
                        // loop through the rules
                        for (int j = 0; j < RULES.Count; j++)
                        {
                            // match found
                            //if(alpha.Substring(0, RULES[j][0].Length) == RULES[j][0])
                            //{

                            //}
                        }

                    } // alpha.Length > 0
                } // NOT ALLBLANK
            } // beta.Length > 0

            key = dm3;
            return key;
        }
    }
}
