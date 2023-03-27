using System;
using System.Text;

namespace Element34.StringMetric
{
    public class NYSIIS : IStringEncoder, IStringComparison
    {
        private const string Vowels = "AEIOU";
        private readonly string[][] first = { new string[] { "MAC", "MCC" }, new string[] { "KN", "N" }, new string[] { "K", "C" },
                                              new string[] { "PH", "FF" }, new string[] { "PF", "FF" }, new string[] { "SCH", "SSS" } };
        private readonly string[][] last = { new string[] { "EE", "Y" }, new string[] { "IE", "Y" }, new string[] { "DT", "D" },
                                             new string[] { "RT", "D" }, new string[] { "RD", "D" }, new string[] { "NT", "D" },
                                             new string[] { "ND", "D" } };

        public string Encode(string sInput)
        {
            int len = sInput.Length;
            StringBuilder sbKey = new StringBuilder(len);

            for (int i = 0; i < len; i++)
            {
                char c = char.ToUpper(sInput[i]);

                if (c >= 'A' && c <= 'Z')
                {
                    sbKey.Append(c);
                }
            }

            Replace(sbKey, 0, first);
            Replace(sbKey, sbKey.Length - 2, last);

            len = sbKey.Length;
            sbKey.Append(" ");

            for (int i = 1; i < len; i++)
            {
                char prev = sbKey[i - 1];
                char curr = sbKey[i];
                char next = sbKey[i + 1];

                if (curr == 'E' && next == 'V')
                {
                    sbKey = Replace(sbKey, i, i + 2, "AF");
                }
                else if (isVowel(curr))
                {
                    sbKey[i] = 'A';
                }
                else if (curr == 'Q')
                {
                    sbKey[i] = 'G';
                }
                else if (curr == 'Z')
                {
                    sbKey[i] = 'S';
                }
                else if (curr == 'M')
                {
                    sbKey[i] = 'N';
                }
                else if (curr == 'K' && next == 'N')
                {
                    sbKey[i] = 'N';
                }
                else if (curr == 'K')
                {
                    sbKey[i] = 'C';
                }
                else if (sbKey.ToString().IndexOf("SCH", i) == i)
                {
                    sbKey = Replace(sbKey, i, i + 3, "SSS");
                }
                else if (curr == 'P' && next == 'H')
                {
                    sbKey = Replace(sbKey, i, i + 2, "FF");
                }
                else if (curr == 'H' && (!isVowel(prev) || !isVowel(next)))
                {
                    sbKey[i] = prev;
                }
                else if (curr == 'W' && isVowel(prev))
                {
                    sbKey[i] = prev;
                }

                if (sbKey[i] == prev)
                {
                    sbKey.Remove(i--);
                    len--;
                }
            }

            sbKey.Length = (sbKey.Length - 1);
            int lastPos = sbKey.Length - 1;

            if (lastPos > 1)
            {
                if (sbKey.ToString().LastIndexOf("AY") == lastPos - 1)
                {
                    sbKey.Remove(lastPos - 1, lastPos + 1).Append("Y");
                }
                else if (sbKey[lastPos] == 'S')
                {
                    sbKey.Length = lastPos;
                }
                else if (sbKey[lastPos] == 'A')
                {
                    sbKey.Length = lastPos;
                }
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
