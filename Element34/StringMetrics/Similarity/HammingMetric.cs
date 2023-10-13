using System;
using System.Linq;

namespace Element34.StringMetrics.Similarity
{
    /// <summary>
    /// In information theory, the Hamming distance between two strings of EQUAL LENGTH 
    /// is the number of positions at which the corresponding symbols are different. In 
    /// other words, it measures the minimum number of substitutions required to change 
    /// one string into the other, or the minimum number of errors that could have 
    /// transformed one string into the other.
    /// <a href="https://en.wikipedia.org/wiki/Hamming_distance" />
    /// </summary>

    public class HammingMetric : StringMetric<int?>
    {
        /// <summary>
        /// Computes the specified source.
        /// </summary>
        /// <param name="source">string A</param>
        /// <param name="target">string B</param>
        /// <returns>The Hamming Distance between the given strings</returns>
        /// <exception cref="System.Exception">Strings must be equal length</exception>
        public override int? Compute(string a, string b)
        {
            return Compute(a.ToCharArray(), b.ToCharArray());
        }

        public override int? Compute(char[] source, char[] target)
        {
            if (source.Length == 0 || target.Length == 0 || source.Length != target.Length)
            {
                return null;
            }
            else if (source.SequenceEqual(target))
            {
                return 0;
            }
            else
            {
                return Hamming((source, target));
            }
        }

        private static int? Hamming((char[], char[]) ct)
        {
            // Using the XOR operator: if ((source[i] ^ target[i]) != 0) distance++;
            // Using the naive approach: if (source[i] != target[i]) distance++;
            // Using LINQ in one line: source.Where((t, i) => !t.Equals(target[i])).Count();

            return ct.Item1.Zip(ct.Item2, (x, y) => x == y ? 0 : 1).Count();
        }

        /// <summary>
        /// Normalized version of the Hamming Distance.
        /// </summary>
        /// <param name="source">string A</param>
        /// <param name="target">string B</param>
        /// <returns>The Normalized Hamming Distance</returns>
        public double Normalized(string source, string target)
        {
            return Convert.ToDouble(Compute(source.ToCharArray(), target.ToCharArray())) / Convert.ToDouble(source.Length + target.Length);
        }
    }
}
