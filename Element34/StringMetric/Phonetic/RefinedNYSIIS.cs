using System;
using System.Text;

namespace Element34.StringMetric
{
    public class RefinedNYSIIS : IStringEncoder, IStringComparison
    {
        private const string Vowels = "AEIOU";
        private readonly string[][] first = { new string[] { "MAC", "MC" }, new string[] { "PF", "F" } };
        private readonly string[][] last = { new string[] { "IX", "IC" }, new string[] { "EX", "EC" },
                                             new string[] { "EE", "Y" }, new string[] { "IE", "Y" },
                                             new string[] { "DT", "D" }, new string[] { "RT", "D" }, new string[] { "RD", "D" },
                                             new string[] { "NT", "D" }, new string[] { "ND", "D" } };

        public string Encode(string sInput)
        {
            int len = sInput.Length;
            StringBuilder sbKey = new StringBuilder(len);
            char preStartCode, startCode = ' ', prev, curr, next;

            for (int i = 0; i < len; i++)
            {
                char c = char.ToUpper(sInput[i]);

                if (c >= 'A' && c <= 'Z')
                {
                    sbKey.Append(c);
                }
            }

            preStartCode = sbKey[0];

            if (sbKey[sbKey.Length] == 'S' || sbKey[sbKey.Length] == 'Z')
                sbKey = sbKey.Substring(0, sbKey.Length - 1);

            Replace(sbKey, 0, first);
            Replace(sbKey, sbKey.Length - 2, last);

            len = sbKey.Length;
            for (int i = 2; i < (len - 1); i++)
            {
                prev = sbKey[i - 1];
                curr = sbKey[i];
                next = sbKey[i + 1];

                if (curr == 'E' && next == 'V')
                {
                    sbKey = Replace(sbKey, i, i + 2, "EF");
                }

                startCode = sbKey[0];

                if (curr == 'W' && isVowel(prev))
                {
                    sbKey[i] = prev;
                }

                if (isVowel(curr))
                {
                    sbKey[i] = 'A';
                }

                if (sbKey.ToString().IndexOf("GHT", i) == i)
                {
                    sbKey = Replace(sbKey, i, i + 3, "GT");
                }

                if (curr == 'D' && next == 'G')
                {
                    sbKey = Replace(sbKey, i, i + 2, "G");
                }

                if (curr == 'P' && next == 'H')
                {
                    sbKey = Replace(sbKey, i, i + 2, "F");
                }

                if (curr == 'H' && (!isVowel(prev) || !isVowel(next)))
                {
                    sbKey[i] = prev;
                }

                else if (curr == 'K' && next == 'N')
                {
                    sbKey = Replace(sbKey, i, i + 2, "N");
                }
                else if (curr == 'K')
                {
                    sbKey[i] = 'C';
                }

                if (curr == 'M')
                {
                    sbKey[i] = 'N';
                }

                if (curr == 'Q')
                {
                    sbKey[i] = 'G';
                }

                if (curr == 'S' && next == 'H')
                {
                    sbKey = Replace(sbKey, i, i + 2, "S");
                }

                if (sbKey.ToString().IndexOf("SCH", i) == i)
                {
                    sbKey = Replace(sbKey, i, i + 3, "S");
                }

                if (curr == 'Y' && next == 'W')
                {
                    sbKey = Replace(sbKey, i, i + 2, "Y");
                }

                if (curr == 'Y')
                {
                    sbKey[i] = 'A';
                }

                if (curr == 'W' && next == 'R')
                {
                    sbKey = Replace(sbKey, i, i + 2, "R");
                }

                if (curr == 'Z')
                {
                    sbKey[i] = 'S';
                }

                if (curr == 'A' && next == 'Y')
                {
                    sbKey = Replace(sbKey, i, i + 2, "A");
                }
            }

            if (isVowel(sbKey[sbKey.Length]))
                sbKey.Length = sbKey[sbKey.Length - 1];

            for (int j = 1; j < sbKey.Length; j++)
            {
                prev = sbKey[j - 1];

                if (sbKey[j] == prev)
                {
                    sbKey.Remove(j--);
                }
            }

            sbKey.Prepend(startCode.ToString());

            if (isVowel(preStartCode))
                sbKey.Prepend(preStartCode.ToString());
            else
            {
                int k = sbKey.IndexOf('A');

                if (k >= 0)
                    sbKey[k] = preStartCode;
            }


            if (sbKey.Length > 6)
            {
                sbKey.Insert(6, '[').Append(']');
            }
            return sbKey.ToString();
        }

        private static void Replace(StringBuilder sb, int start, string[][] maps)
        {
            if (start >= 0)
            {
                foreach (string[] map in maps)
                {
                    if (sb.ToString().IndexOf(map[0]) == start)
                    {
                        sb = Replace(sb, start, start + map[0].Length, map[1]);
                        break;
                    }
                }
            }
        }

        private static StringBuilder Replace(StringBuilder sbSource, int index, int length, string substitute)
        {
            int range = index + length;

            StringBuilder sbResult = sbSource.Substring(0, index);
            sbResult.Append(substitute);
            sbResult.Append(sbSource.Substring(range, sbSource.Length - range));

            return sbResult;
        }

        private static bool isVowel(char c)
        {
            return Vowels.IndexOf(c) != -1;
        }

        public bool Compare(string value1, string value2)
        {
            throw new NotImplementedException();
        }
    }
}
