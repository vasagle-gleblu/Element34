using System;
using System.Collections.Generic;
using System.Linq;

namespace Element34.StringMetrics.Similarity
{
    /// <summary>
    /// Jaccard index: 
    /// The Jaccard index, also known as Intersection over Union and the Jaccard similarity
    /// coefficient, is a statistic used for gauging the similarity and diversity of sample sets.
    /// 
    /// <a hef="https://en.wikipedia.org/wiki/Jaccard_index" />
    /// </summary>
    public class JaccardMetric : StringMetric<double?>
    {
        private readonly int n;

        public JaccardMetric(int n)
        {
            this.n = n;
        }

        public JaccardMetric() : this(2)
        {
        }

        /// <summary>
        /// Jaccard the index.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public override double? Compute(string a, string b)
        {
            return Compute(a.ToCharArray(), b.ToCharArray());
        }

        public override double? Compute(char[] a, char[] b)
        {
            if (n <= 0 || a.Length < n || b.Length < n)
            {
                return null;
            }
            else if (a.SequenceEqual(b))
            {
                return (double)1.0;
            }
            else
            {
                List<string> ca1bg = new NGramTokenizer(n).Tokenizer(a.ToString());
                ca1bg = (ca1bg == null) ? new List<string>() : ca1bg;

                List<string> ca2bg = new NGramTokenizer(n).Tokenizer(b.ToString());
                ca2bg = (ca2bg == null) ? new List<string>() : ca2bg;

                int intersection = ca1bg.Intersect(ca2bg).Count();

                return (double)intersection / (ca1bg.Count + ca2bg.Count - intersection);
            }
        }

        /// <summary>
        /// Distances the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public double? Distance(string source, string target)
        {
            return 1 - Compute(source, target);
        }
    }
}
