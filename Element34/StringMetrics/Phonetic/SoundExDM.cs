using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using static Element34.StringMetrics.Alphabets;

namespace Element34.StringMetrics.Phonetic
{

    /// <summary>
    /// Daitch-Mokotoff Soundex:
    /// Daitch–Mokotoff Soundex is a phonetic algorithm invented in 1985 by Jewish 
    /// genealogists Gary Mokotoff and Randy Daitch.It is a refinement of the Russell 
    /// and American Soundex algorithms designed to allow greater accuracy in matching 
    /// of Slavic and Yiddish surnames with similar pronunciation but differences in spelling.
    /// <a href="https://en.wikipedia.org/wiki/Daitch%E2%80%93Mokotoff_Soundex" />
    /// 
    /// This algorithm was converted from Java to C# from the following repository.
    /// <a href="https://github.com/netomi/dm-soundex/blob/master/src/main/java/org/netomi/codec/DMSoundex.java" /> 
    /// </summary>
    public class SoundExDM : IStringEncoder, IStringComparison
    {
        #region [Fields]
        private const string RESOURCE_RULES = "DMrules";
        private const string COMMENT = "//";
        private const string DOUBLE_QUOTE = "\"";
        private const string MULTILINE_COMMENT_END = "*/";
        private const string MULTILINE_COMMENT_START = "/*";

        private static readonly Dictionary<char, List<Rule>> RULES = new Dictionary<char, List<Rule>>();
        private static readonly Dictionary<char, char> FOLDINGS = new Dictionary<char, char>();
        #endregion

        static SoundExDM()
        {
            ResourceManager rm = new ResourceManager("items", Assembly.GetExecutingAssembly());
            string rulesIS = rm.GetString(RESOURCE_RULES);
            {
                if (string.IsNullOrEmpty(rulesIS))
                {
                    throw new ArgumentException("Unable to load resource: " + RESOURCE_RULES);
                }

                using (StreamReader scanner = new StreamReader(rulesIS))
                {
                    ParseRules(scanner, RESOURCE_RULES, RULES, FOLDINGS);
                }
            }
            rulesIS = null;
            rm = null;

            // Sort RULES by pattern length in descending order
            foreach (var item in RULES)
            {
                List<Rule> ruleList = item.Value;
                ruleList.Sort((r1, r2) => r2.GetPatternLength().CompareTo(r1.GetPatternLength()));
            }
        }

        #region [Public Methods]

        /// <summary>
        /// Compares the specified values using DMSoundEx algorithm.
        /// </summary>
        /// <param name="value1">string A</param>
        /// <param name="value2">string B</param>
        /// <returns>Results in true if the encoded input strings match.</returns>
        public bool Compare(string value1, string value2)
        {
            SoundExDM sdx = new SoundExDM();
            value1 = sdx.Encode(value1);
            value2 = sdx.Encode(value2);

            return value1.Equals(value2);
        }

        public char[] Encode(char[] buffer)
        {
            return Encode(buffer.ToString()).ToCharArray();
        }

        public string Encode(string source)
        {
            if (source == null)
            {
                return null;
            }

            string input = Cleanup(source);
            HashSet<SoundexContext> branches = new HashSet<SoundexContext>
            {
                new SoundexContext()
            };
            char lastChar = '\0';

            for (int index = 0; index < input.Length; index++)
            {
                char ch = input[index];

                if (char.IsWhiteSpace(ch))
                {
                    continue;
                }

                string inputContext = input.Substring(index);
                if (!RULES.TryGetValue(ch, out List<Rule> rules))
                {
                    continue;
                }

                foreach (Rule rule in rules)
                {
                    if (rule.Matches(inputContext))
                    {
                        string replacements = rule.GetReplacement(inputContext, lastChar == '\0');
                        string[] replacementsArray = replacements.Split('|');

                        HashSet<SoundexContext> newSet = new HashSet<SoundexContext>();
                        foreach (SoundexContext branch in branches)
                        {
                            SoundexContext originalBranch = replacementsArray.Length > 1 ? branch.CreateBranch() : branch;
                            int replacementIndex = 0;

                            foreach (string nextReplacement in replacementsArray)
                            {
                                SoundexContext currentBranch = replacementIndex > 0 ? originalBranch.CreateBranch() : originalBranch;

                                bool addReplacement =
                                    currentBranch.LastReplacement == null ||
                                    !currentBranch.LastReplacement.EndsWith(nextReplacement) ||
                                    (lastChar == 'm' && ch == 'n') ||
                                    (lastChar == 'n' && ch == 'm');

                                if (addReplacement)
                                {
                                    currentBranch.stringBuilder.Append(nextReplacement);
                                }

                                currentBranch.LastReplacement = nextReplacement;
                                if (currentBranch.stringBuilder.Length > 6)
                                {
                                    currentBranch.stringBuilder.Remove(6, currentBranch.stringBuilder.Length - 6);
                                }
                                newSet.Add(currentBranch);
                                replacementIndex++;
                            }
                        }

                        branches = newSet;
                        index += rule.GetPatternLength() - 1;
                        break;
                    }
                }

                lastChar = ch;
            }

            StringBuilder result = new StringBuilder();
            foreach (SoundexContext branch in branches)
            {
                while (branch.stringBuilder.Length < 6)
                {
                    branch.stringBuilder.Append('0');
                }

                if (result.Length > 0)
                {
                    result.Append('|');
                }

                result.Append(branch.stringBuilder.ToString().Substring(0, 6));
            }

            return result.ToString();
        }
        #endregion

        #region [Private Methods]
        private static void ParseRules(StreamReader scanner, string location,
            Dictionary<char, List<Rule>> ruleMapping, Dictionary<char, char> asciiFoldings)
        {
            int currentLine = 0;
            bool inMultilineComment = false;

            while (!scanner.EndOfStream)
            {
                currentLine++;
                string rawLine = scanner.ReadLine();
                string line = rawLine;

                if (inMultilineComment)
                {
                    if (line.EndsWith(MULTILINE_COMMENT_END))
                    {
                        inMultilineComment = false;
                    }
                    continue;
                }

                if (line.StartsWith(MULTILINE_COMMENT_START))
                {
                    inMultilineComment = true;
                }
                else
                {
                    int cmtI = line.IndexOf(COMMENT, StringComparison.Ordinal);
                    if (cmtI >= 0)
                    {
                        line = line.Substring(0, cmtI);
                    }

                    line = line.Trim();

                    if (line.Length == 0)
                    {
                        continue;
                    }

                    if (line.Contains("="))
                    {
                        string[] parts = line.Split('=');
                        if (parts.Length != 2)
                        {
                            throw new ArgumentException("Malformed folding statement split into " +
                                                        parts.Length + " parts: " + rawLine + " in " + location);
                        }
                        string leftCharacter = parts[0].Trim();
                        string rightCharacter = parts[1].Trim();

                        if (leftCharacter.Length != 1 || rightCharacter.Length != 1)
                        {
                            throw new ArgumentException("Malformed folding statement - patterns are not " +
                                                        "single characters: " + rawLine + " in " + location);
                        }

                        asciiFoldings[leftCharacter[0]] = rightCharacter[0];
                    }
                    else
                    {
                        string[] parts = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length != 4)
                        {
                            throw new ArgumentException("Malformed rule statement split into " +
                                                        parts.Length + " parts: " + rawLine + " in " + location);
                        }

                        try
                        {
                            string pattern = StripQuotes(parts[0]);
                            string replacement1 = StripQuotes(parts[1]);
                            string replacement2 = StripQuotes(parts[2]);
                            string replacement3 = StripQuotes(parts[3]);

                            Rule r = new Rule(pattern, replacement1, replacement2, replacement3);
                            char patternKey = r.Pattern[0];
                            if (!ruleMapping.TryGetValue(patternKey, out List<Rule> rules))
                            {
                                rules = new List<Rule>();
                                ruleMapping[patternKey] = rules;
                            }
                            rules.Add(r);
                        }
                        catch (ArgumentException e)
                        {
                            throw new InvalidOperationException(
                                "Problem parsing line '" + currentLine + "' in " + location, e);
                        }
                    }
                }
            }
        }

        private static string StripQuotes(string str)
        {
            if (str.StartsWith(DOUBLE_QUOTE))
            {
                str = str.Substring(1);
            }

            if (str.EndsWith(DOUBLE_QUOTE))
            {
                str = str.Substring(0, str.Length - 1);
            }

            return str;
        }

        private string Cleanup(string input)
        {
            var sb = new StringBuilder();
            foreach (char ch in input)
            {
                if (char.IsWhiteSpace(ch))
                {
                    continue;
                }

                char lowerCaseCh = char.ToLower(ch);
                if (FOLDINGS.TryGetValue(lowerCaseCh, out char foldedCh))
                {
                    sb.Append(foldedCh);
                }
                else
                {
                    sb.Append(lowerCaseCh);
                }
            }
            return sb.ToString();
        }

        private class SoundexContext
        {
            public StringBuilder stringBuilder { get; } = new StringBuilder();
            public string LastReplacement { get; set; }

            public SoundexContext CreateBranch()
            {
                SoundexContext context = new SoundexContext();
                context.stringBuilder.Append(stringBuilder.ToString());
                context.LastReplacement = LastReplacement;
                return context;
            }

            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }
                SoundexContext other = (SoundexContext)obj;
                return stringBuilder.ToString().Equals(other.stringBuilder.ToString());
            }

            public override int GetHashCode()
            {
                return stringBuilder.ToString().GetHashCode();
            }
        }
        #endregion

        private class Rule
        {
            public string Pattern { get; }
            public string ReplacementAtStart { get; }
            public string ReplacementBeforeVowel { get; }
            public string ReplacementDefault { get; }

            public Rule(string pattern, string replacementAtStart, string replacementBeforeVowel, string replacementDefault)
            {
                Pattern = pattern;
                ReplacementAtStart = replacementAtStart;
                ReplacementBeforeVowel = replacementBeforeVowel;
                ReplacementDefault = replacementDefault;
            }

            public int GetPatternLength()
            {
                return Pattern.Length;
            }

            public bool Matches(string context)
            {
                return context.StartsWith(Pattern, StringComparison.Ordinal);
            }

            public string GetReplacement(string context, bool atStart)
            {
                if (atStart)
                {
                    return ReplacementAtStart;
                }

                int nextIndex = GetPatternLength();
                bool nextCharIsVowel = nextIndex < context.Length && IsVowel(context[nextIndex]);
                if (nextCharIsVowel)
                {
                    return ReplacementBeforeVowel;
                }

                return ReplacementDefault;
            }

            private bool IsVowel(char ch)
            {
                return LowercaseVowel.IsContained(ch);
            }
        }
    }
}