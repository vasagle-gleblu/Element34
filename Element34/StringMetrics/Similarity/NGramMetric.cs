using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
    public class NGramMetric : StringMetric<double?>
    {
        private readonly int N;

        /// <summary>
        /// Creates an N-Gram distance measure using n-grams of the specified size. </summary>
        /// <param name="size"> The size of the n-gram to be used to compute the string distance. </param>

        public NGramMetric(int size)
        {
            this.N = size;
        }

        /// <summary>
        /// Creates an N-Gram distance measure using n-grams of size 2.
        /// </summary>
        public NGramMetric() : this(2)
        {
        }

        public override double? Compute(char[] a, char[] b)
        {
            return Compute(a.ToString(), b.ToString());
        }

        public override double? Compute(string a, string b)
        {
            if (N <= 0 || a.Length < N || b.Length < N)
                return null;

            if (a == b)
                return (double)1.0;

            NGramTokenizer tokenizer = new NGramTokenizer(N);
            List<string> tokensA = tokenizer.Tokenizer(a).ToList();
            List<string> tokensB = tokenizer.Tokenizer(b).ToList();

            int matches = ScoreMatches((tokensA, tokensB));

            return matches / (double)Math.Max(tokensA.Count, tokensB.Count);
        }

        private static int ScoreMatches((List<string>, List<string>) matchTuple)
        {
            var (tokensA, tokensB) = matchTuple;
            var intersection = tokensA.Intersect(tokensB);

            return intersection.Count();
        }
    }

    internal class NGramTokenizer
    {
        private readonly int N;

        public NGramTokenizer(int size)
        {
            this.N = size;
        }

        public List<string> Tokenizer(string input)
        {
            ArrayList tokens = new ArrayList();

            if (input == null || input.Length == 0)
                return null;

            int length = input.Length;
            if (length < N)
            {
                string gram;
                for (int i = 1; i <= length; i++)
                {
                    gram = input.Substring(0, (i) - (0));
                    if (tokens.IndexOf(gram) == -1)
                        tokens.Add(gram);
                }

                gram = input.Substring(length - 1, (length) - (length - 1));
                if (tokens.IndexOf(gram) == -1)
                    tokens.Add(gram);
            }
            else
            {
                for (int i = 1; i <= N - 1; i++)
                {
                    string gram = input.Substring(0, (i) - (0));
                    if (tokens.IndexOf(gram) == -1)
                        tokens.Add(gram);

                }

                for (int i = 0; i < (length - N) + 1; i++)
                {
                    string gram = input.Substring(i, (i + N) - (i));
                    if (tokens.IndexOf(gram) == -1)
                        tokens.Add(gram);
                }

                for (int i = (length - N) + 1; i < length; i++)
                {
                    string gram = input.Substring(i, (length) - (i));
                    if (tokens.IndexOf(gram) == -1)
                        tokens.Add(gram);
                }
            }

            return tokens.Cast<string>().ToList();
        }
    }

}
