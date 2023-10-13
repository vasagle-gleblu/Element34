using System.Linq;
using System;
using System.Text;
using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OpenQA.Selenium.Interactions;

namespace Element34.StringMetrics.Similarity
{
    /// <summary>
    /// Overlap coefficient: The overlap coefficient, or Szymkiewicz–Simpson coefficient, 
    /// is a similarity measure that measures the overlap between two finite sets.  It is related 
    /// to the Jaccard index and is defined as the size of the intersection divided by the 
    /// smaller of the size of the two sets.
    /// 
    /// <a href="https://en.wikipedia.org/wiki/Overlap_coefficient"/>
    /// </summary>
    public class OverlapMetric : StringMetric<double?>
    {
        private readonly int n;

        public OverlapMetric()
        {
            n = 1;
        }

        public OverlapMetric(int n)
        {
            this.n = n;
        }

        public override double? Compute(char[] a, char[] b)
        {
            return Compute(a.ToString(), b.ToString());
        }

        public override double? Compute(string a, string b)
        {
            if (n <= 0 || a.Length < n || b.Length < n)
                return null;

            if (a == b)
                return 1d;

            var tokensA = Tokenize(a);
            var tokensB = Tokenize(b);

            var matches = ScoreMatches((tokensA, tokensB));

            return (double)matches / Math.Min(tokensA.Count, tokensB.Count);
        }

        private List<string> Tokenize(string input)
        {
            var tokens = new List<string>();

            for (int i = 0; i <= input.Length - n; i++)
            {
                string token = input.Substring(i, n);
                tokens.Add(token);
            }

            return tokens;
        }

        private static int ScoreMatches((List<string>, List<string>) matchTuple)
        {
            var (tokensA, tokensB) = matchTuple;
            var intersection = tokensA.Intersect(tokensB);

            return intersection.Count();
        }
    }
}
