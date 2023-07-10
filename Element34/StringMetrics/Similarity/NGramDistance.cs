using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Element34.StringMetrics.Similarity
{
    /// <summary>
    /// N-Gram version of edit distance based on paper by Grzegorz Kondrak, 
    /// "N-gram similarity and distance". Proceedings of the Twelfth International 
    /// Conference on String Processing and Information Retrieval (SPIRE 2005), pp. 115-126, 
    /// Buenos Aires, Argentina, November 2005. 
    /// <a href="http://www.cs.ualberta.ca/~kondrak/papers/spire05.pdf" />
    /// 
    /// This implementation uses the position-based optimization to compute partial
    /// matches of n-gram sub-strings and adds a null-character prefix of size n-1 
    /// so that the first character is contained in the same number of n-grams as 
    /// a middle character.  Null-character prefix matches are discounted so that 
    /// strings with no matching characters will return a distance of 0.
    /// </summary>
    public class NGramDistance
    {
        private readonly int n;

        /// <summary>
        /// Creates an N-Gram distance measure using n-grams of the specified size. </summary>
        /// <param name="size"> The size of the n-gram to be used to compute the string distance. </param>
        public NGramDistance(int size)
        {
            this.n = size;
        }

        /// <summary>
        /// Creates an N-Gram distance measure using n-grams of size 2.
        /// </summary>
        public NGramDistance() : this(2)
        {
        }

        public double Compute(string source, string target)
        {
            int srcLen = source.Length;
            int tgtLen = target.Length;

            if (srcLen == 0 || tgtLen == 0)
            {
                if (srcLen == tgtLen)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

            int c = 0;
            if (srcLen < n || tgtLen < n)
            {
                for (int k = 0, ni = Math.Min(srcLen, tgtLen); k < ni; k++)
                {
                    if (source[k] == target[k])
                    {
                        c++;
                    }
                }
                return (double)c / Math.Max(srcLen, tgtLen);
            }

            char[] SrcArray = new char[srcLen + n - 1];
            double[] Prev; //'previous' cost array, horizontally
            double[] cost; // cost array, horizontally
            double[] tmp; //placeholder to assist in swapping Prev and d

            //construct SrcArray with prefix
            for (int l = 0; l < SrcArray.Length; l++)
            {
                if (l < n - 1)
                {
                    SrcArray[l] = (char)0; //add prefix
                }
                else
                {
                    SrcArray[l] = source[l - n + 1];
                }
            }
            Prev = new double[srcLen + 1];
            cost = new double[srcLen + 1];

            // indexes into strings s and t
            int i; // iterates through source
            int j; // iterates through target

            char[] t_j = new char[n]; // jth n-gram of t

            for (i = 0; i <= srcLen; i++)
            {
                Prev[i] = i;
            }

            for (j = 1; j <= tgtLen; j++)
            {
                //construct t_j n-gram 
                if (j < n)
                {
                    for (int ti = 0; ti < n - j; ti++)
                    {
                        t_j[ti] = (char)0; //add prefix
                    }
                    for (int ti = n - j; ti < n; ti++)
                    {
                        t_j[ti] = target[ti - (n - j)];
                    }
                }
                else
                {
                    t_j = target.Substring(j - n, j - (j - n)).ToCharArray();
                }
                cost[0] = j;
                for (i = 1; i <= srcLen; i++)
                {
                    c = 0;
                    int tn = n;
                    //compare SrcArray to t_j
                    for (int ni = 0; ni < n; ni++)
                    {
                        if (SrcArray[i - 1 + ni] != t_j[ni])
                        {
                            c++;
                        }
                        else if (SrcArray[i - 1 + ni] == 0) //discount matches on prefix
                        {
                            tn--;
                        }
                    }
                    double ec = (double)c / tn;
                    // minimum of cell to the left + 1, to the top + 1, diagonally left and up +cost
                    cost[i] = Math.Min(Math.Min(cost[i - 1] + 1, Prev[i] + 1), Prev[i - 1] + ec);
                }
                // copy current distance counts to 'previous row' distance counts
                tmp = Prev;
                Prev = cost;
                cost = tmp;
            }

            // our last action in the above loop was to switch d and p, so p now
            // actually has the most recent cost counts
            return 1.0f - (Prev[srcLen] / Math.Max(tgtLen, srcLen));
        }
    }
}
