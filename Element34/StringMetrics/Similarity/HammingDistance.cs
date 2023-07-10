using System;

namespace Element34.StringMetrics
{
    /// <summary>
    /// In information theory, the Hamming distance between two strings of EQUAL LENGTH 
    /// is the number of positions at which the corresponding symbols are different. In 
    /// other words, it measures the minimum number of substitutions required to change 
    /// one string into the other, or the minimum number of errors that could have 
    /// transformed one string into the other.
    /// <a href="https://en.wikipedia.org/wiki/Hamming_distance" />
    /// </summary>

    public class HammingDistance
    {
        /// <summary>
        /// Computes the specified source.
        /// </summary>
        /// <param name="source">string A</param>
        /// <param name="target">string B</param>
        /// <returns>The Hamming Distance between the given strings</returns>
        /// <exception cref="System.Exception">Strings must be equal length</exception>
        public int Compute(string source, string target)
        {
            if (source.Length != target.Length)
            {
                throw new Exception("Strings must be equal length");
            }

            // Calculating Hamming Distance using the XOR Operator
            int distance = 0;

            for (int i = 0; i < source.Length; i++)
            {   
                if ((source[i] ^ target[i]) != 0)
                    distance++;
            }

            return distance;

            // Using the naive approach: if (source[i] != target[i]) distance++;
            // Using LINQ in one line: source.Where((t, i) => !t.Equals(target[i])).Count();
        }

        /// <summary>
        /// Normalized version of the Hamming Distance.
        /// </summary>
        /// <param name="source">string A</param>
        /// <param name="target">string B</param>
        /// <returns>The Normalized Hamming Distance</returns>
        public double Normalized(string source, string target)
        {
            return Convert.ToDouble(Compute(source, target)) / Convert.ToDouble(source.Length + target.Length);
        }
    }
}
