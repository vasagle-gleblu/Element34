using System;
using System.Collections.Generic;
using System.Linq;

namespace Element34.StringMetrics.Similarity
{
    /// <summary>
    /// Ratcliff/Obershelp similarity algorithm:
    /// This is a string similarity metric used to quantify the similarity between two strings. It's 
    /// particularly effective for comparing relatively long strings and finding the similarity between 
    /// them based on common substrings. The main idea behind the algorithm is to find the longest 
    /// common substring between the two input strings and then compute a similarity score based on 
    /// the length of this common substring and the lengths of the original strings.  It was 
    /// developed in 1983 by John W.Ratcliff and John A. Obershelp and published in the 
    /// Dr. Dobb's Journal in July 1988.
    /// 
    /// <a href="https://en.wikipedia.org/wiki/Gestalt_pattern_matching"/>
    /// </summary>
    public class RatcliffObershelpMetric : StringMetric<double?>
    {
        public override double? Compute(char[] a, char[] b)
        {
            if (a.Length == 0 || b.Length == 0)
                return null;
            else if (a.Equals(b))
                return 1.0;
            else
                return 2.0 * GetCommonSequences(a, b).TotalLength / (a.Length + b.Length);
        }

        public override double? Compute(string a, string b) => Compute(a.ToCharArray(), b.ToCharArray());

        private (int Length, int Row, int Column) LongestCommonSubsequence((char[], char[]) ct)
        {
            int[,] m = new int[ct.Item1.Length + 1, ct.Item2.Length + 1];
            (int Length, int Row, int Column) lrc = (0, 0, 0);

            for (int r = 0; r < ct.Item1.Length; r++)
            {
                for (int c = 0; c < ct.Item2.Length; c++)
                {
                    if (ct.Item1[r] == ct.Item2[c])
                    {
                        int l = m[r, c] + 1;
                        m[r + 1, c + 1] = l;
                        if (l > lrc.Length)
                            lrc = (l, r + 1, c + 1);
                    }
                }
            }

            return lrc;
        }

        private List<char[]> GetCommonSequences((char[], char[]) ct)
        {
            var lcs = LongestCommonSubsequence(ct);
            List<char[]> commonSeqList = new List<char[]>();

            if (lcs.Length == 0)
                return commonSeqList;
            else
            {
                var sct1 = (new ArraySegment<char>(ct.Item1, 0, lcs.Row - lcs.Length).ToArray(), new ArraySegment<char>(ct.Item1, lcs.Row, ct.Item1.Length - lcs.Row).ToArray());
                var sct2 = (new ArraySegment<char>(ct.Item2, 0, lcs.Column - lcs.Length).ToArray(), new ArraySegment<char>(ct.Item2, lcs.Column, ct.Item2.Length - lcs.Column).ToArray());

                commonSeqList.Add(new ArraySegment<char>(ct.Item1, lcs.Row - lcs.Length, lcs.Length).ToArray());
                commonSeqList.AddRange(GetCommonSequences(sct1));
                commonSeqList.AddRange(GetCommonSequences(sct2));
            }

            return commonSeqList;
        }

        private class CommonSequencesResult
        {
            public List<char[]> CommonSequences { get; set; }
            public int TotalLength { get; set; }
        }

        private CommonSequencesResult GetCommonSequences(char[] a, char[] b)
        {
            var lcs = LongestCommonSubsequence((a, b));
            List<char[]> commonSeqList = new List<char[]>();

            if (lcs.Length == 0)
                return new CommonSequencesResult { CommonSequences = commonSeqList, TotalLength = 0 };
            else
            {
                var sct1 = (new ArraySegment<char>(a, 0, lcs.Row - lcs.Length).ToArray(), new ArraySegment<char>(a, lcs.Row, a.Length - lcs.Row).ToArray());
                var sct2 = (new ArraySegment<char>(b, 0, lcs.Column - lcs.Length).ToArray(), new ArraySegment<char>(b, lcs.Column, b.Length - lcs.Column).ToArray());

                commonSeqList.Add(new ArraySegment<char>(a, lcs.Row - lcs.Length, lcs.Length).ToArray());
                commonSeqList.AddRange((IEnumerable<char[]>)GetCommonSequences(sct1.Item1, sct2.Item1));
                commonSeqList.AddRange((IEnumerable<char[]>)GetCommonSequences(sct1.Item2, sct2.Item2));

                int totalLength = lcs.Length + GetCommonSequences(sct1).Count + GetCommonSequences(sct2).Count;
                return new CommonSequencesResult { CommonSequences = commonSeqList, TotalLength = totalLength };
            }
        }
    }
}
