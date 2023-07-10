using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Element34.StringMetrics.Similarity
{
    public  class LongestCommonSequence
    {
        public  string Compute(string source, string target)
        {
            int[,] c = LongestCommonSequenceLengthTable(source, target);

            return Backtrack(c, source, target, source.Length, target.Length);
        }

        private  int[,] LongestCommonSequenceLengthTable(string source, string target)
        {
            int[,] c = new int[source.Length + 1, target.Length + 1];

            for (int i = 0; i < source.Length + 1; i++) c[i, 0] = 0;
            for (int j = 0; j < target.Length + 1; j++) c[0, j] = 0;

            for (int i = 1; i < source.Length + 1; i++)
                for (int j = 1; j < target.Length + 1; j++)
                    if (source[i - 1].Equals(target[j - 1]))
                        c[i, j] = c[i - 1, j - 1] + 1;
                    else
                        c[i, j] = Math.Max(c[i, j - 1], c[i - 1, j]);

            return c;
        }

        private  string Backtrack(int[,] c, string source, string target, int i, int j)
        {
            if (i == 0 || j == 0)
                return "";

            if (source[i - 1].Equals(target[j - 1]))
                return Backtrack(c, source, target, i - 1, j - 1) + source[i - 1];

            if (c[i, j - 1] > c[i - 1, j])
                return Backtrack(c, source, target, i, j - 1);

            return Backtrack(c, source, target, i - 1, j);
        }

    }
}
