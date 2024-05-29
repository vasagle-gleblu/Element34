using Element34.StringMetrics.Similarity;
using System;

namespace Element34.StringMetrics
{
    /// <summary>
    /// Jaro-Winkler distance:
    /// In computer science and statistics, the Jaro–Winkler distance is a string metric for measuring 
    /// the edit distance between two sequences.  It is a variant proposed in 1990 by William E.Winkler 
    /// of the Jaro distance metric.  Informally, the Jaro distance between two words is the minimum 
    /// number of single-character transpositions required to change one word into the other.
    /// 
    /// <a href="https://en.wikipedia.org/wiki/Jaro%E2%80%93Winkler_distance" />
    /// </summary>
    public class JaroWinklerMetric : StringMetric<double?>
    {
        private static readonly JaroMetric m_jaroDist = new JaroMetric();

        public override double? Compute(string source, string target)
        {
            double? jaroDistance = m_jaroDist.Compute(source, target);
            double? commonPrefixLength = CommonPrefixLength(source, target);

            return jaroDistance + commonPrefixLength * 0.1 * (1 - jaroDistance);
        }

        public override double? Compute(char[] a, char[] b) => Compute(a.ToString(), b.ToString());

        public double? ComputeWithPrefixScale(string source, string target, double prefixScale)
        {
            if (prefixScale > 0.25) prefixScale = 0.25;
            else if (prefixScale < 0) prefixScale = 0;

            double? jaroDistance = m_jaroDist.Compute(source, target);
            double? commonPrefixLength = CommonPrefixLength(source, target);

            return jaroDistance + commonPrefixLength * prefixScale * (1 - jaroDistance);
        }

        private double? CommonPrefixLength(string source, string target)
        {
            int maximumPrefixLength = 4;
            int commonPrefixLength = 0;
            if (source.Length <= 4 || target.Length <= 4) maximumPrefixLength = Math.Min(source.Length, target.Length);

            for (int i = 0; i < maximumPrefixLength; i++)
                if (source[i].Equals(target[i])) commonPrefixLength++;
                else return commonPrefixLength;

            return commonPrefixLength;
        }

        /// <summary>
        /// Returns the normalized Jaro-Winkler distance between two strings.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="other">The other string.</param>
        /// <param name="maxPrefixLength">The maximum length of common prefixes.</param>
        /// <returns>
        /// A value between <c>0</c> and <c>1</c> such that <c>0</c> equates to no similarity
        /// and <c>1</c> is an exact match.
        /// </returns>
        public double? Normalized(string source, string other, int maxPrefixLength = 4)
        {
            // The scaling factor for how much the score is adjusted upwards for having common prefixes.
            // This value should not exceed 0.25; otherwise, the result can become larger than 1.
            const double PrefixScale = 0.1;

            double? jaroScore = m_jaroDist.Compute(source, other);
            return jaroScore + (PrefixLength() * PrefixScale * (1.0 - jaroScore));

            int PrefixLength()
            {
                var maxLength = Math.Min(Math.Min(source.Length, other.Length), maxPrefixLength);
                for (var i = 0; i < maxLength; i++)
                    if (source[i] != other[i])
                        return i;
                return maxLength;
            }
        }
    }
}
