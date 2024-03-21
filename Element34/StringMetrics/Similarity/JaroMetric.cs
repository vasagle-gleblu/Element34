using System.Collections.Generic;
using System.Linq;

namespace Element34.StringMetrics.Similarity
{
    /// <summary>
    /// Jaro Similarity: 
    /// The Jaro similarity algorithm is a measure of the similarity between two strings. It is 
    /// commonly used in natural language processing and information retrieval to calculate the 
    /// similarity between two strings of text. The Jaro similarity algorithm compares the two 
    /// strings character by character, taking into account the number of matching characters 
    /// and the number of transpositions (or character swaps) needed to transform one string 
    /// into the other. The resulting similarity score ranges from 0, indicating that the two 
    /// strings are completely different, to 1, indicating that the two strings are identical. 
    /// 
    /// <a href="https://en.wikipedia.org/wiki/Jaro%E2%80%93Winkler_distance" />
    /// </summary>
    public class JaroMetric : StringMetric<double?>
    {
        private LevenshteinMetric m_levenshteinDist = new LevenshteinMetric();

        /// <summary>
        /// Jaro Similarity is the measure of similarity between two strings. The value ranges 
        /// from 0 to 1. where 1 means the strings are equal and 0 means no similarity between the two strings.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="target">The target string.</param>
        /// <returns>
        /// A value between <c>0</c> and <c>1</c> such that <c>0</c> equates to no similarity
        /// and <c>1</c> is an exact match.
        /// </returns>
        public override double? Compute(string source, string target)
        {
            int m = source.Intersect(target).Count();

            if (m == 0)
                return 0;

            string sourceTargetIntersetAsString = string.Empty;
            string targetSourceIntersetAsString = string.Empty;

            IEnumerable<char> sourceIntersectTarget = source.Intersect(target);
            IEnumerable<char> targetIntersectSource = target.Intersect(source);

            foreach (char character in sourceIntersectTarget)
                sourceTargetIntersetAsString += character;

            foreach (char character in targetIntersectSource)
                targetSourceIntersetAsString += character;

            double? t = m_levenshteinDist.Compute(sourceTargetIntersetAsString, targetSourceIntersetAsString) / 2;

            return (m / source.Length + m / (target.Length + (m - t)) / m) / 3.0;
        }

        public override double? Compute(char[] a, char[] b) => Compute(a.ToString(), b.ToString());

    }
}
