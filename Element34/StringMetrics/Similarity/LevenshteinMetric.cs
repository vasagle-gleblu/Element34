using System;

namespace Element34.StringMetrics.Similarity
{
    /// <summary>
    /// Levenshtein Distance:
    /// In information theory, linguistics, and computer science, the Levenshtein distance 
    /// is a string metric for measuring the difference between two sequences. Informally, 
    /// the Levenshtein distance between two words is the minimum number of single-character 
    /// edits required to change one word into the other.
    /// 
    /// <a href="https://en.wikipedia.org/wiki/Levenshtein_distance" />
    /// </summary>
    public class LevenshteinMetric : StringMetric<double?>
    {
        private static HammingMetric m_hamDist = new HammingMetric();

        /// <summary>
        ///Calculate the minimum number of single-character edits needed to change the source into the target,
        ///allowing insertions, deletions, and substitutions.
        ///Time complexity: at least O(n^2), where n is the length of each string
        ///Accordingly, this algorithm is most efficient when at least one of the strings is very short
        /// </summary>
        /// <param name="source">string A</param>
        /// <param name="target">string B</param>
        /// <returns>
        ///The number of edits required to transform the source into the target. This is at most the length of the
        ///longest string, and at least the difference in length between the two strings
        /// </returns>
        public override double? Compute(string source, string target)
        {
            source = source?.Trim();
            target = target?.Trim();

            if (source == null || target == null) return 0;
            if (source.Length == 0 || target.Length == 0) return 0;
            if (source == target) return 0;

            int sourceWordCount = source.Length;
            int targetWordCount = target.Length;

            // Step 1
            if (sourceWordCount == 0)
                return targetWordCount;

            if (targetWordCount == 0)
                return sourceWordCount;

            int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

            // Step 2
            for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++) { }
            for (int j = 0; j <= targetWordCount; distance[0, j] = j++) { }

            for (int i = 1; i <= sourceWordCount; i++)
                for (int j = 1; j <= targetWordCount; j++)
                {
                    // Step 3
                    int cost = target[j - 1] == source[i - 1] ? 0 : 1;

                    // Step 4
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
                        distance[i - 1, j - 1] + cost);
                }

            return distance[sourceWordCount, targetWordCount];
        }

        public override double? Compute(char[] a, char[] b)
        {
            return Compute(a.ToString(), b.ToString());
        }

        /// <summary>
        ///     Calculate the minimum number of single-character edits needed to change the source into the target,
        ///     allowing insertions, deletions, and substitutions.
        ///     <br /><br />
        ///     Time complexity: at least O(n^2), where n is the length of each string
        ///     Accordingly, this algorithm is most efficient when at least one of the strings is very short
        /// </summary>
        /// <param name="source">string A</param>
        /// <param name="target">string B</param>
        /// <returns>
        ///     The Levenshtein distance, normalized so that the lower bound is always zero, rather than the difference in
        ///     length between the two strings
        /// </returns>
        public double? Normalized(string source, string target)
        {
            double? unnormalizedLevenshteinDistance = Compute(source, target);

            return unnormalizedLevenshteinDistance - LowerBounds(source, target);
        }

        /// <summary>
        ///     The upper bounds is either the length of the longer string, or the Hamming distance.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        internal protected double UpperBounds(string source, string target)
        {
            // If the two strings are the same length then the Hamming Distance is the upper bounds of the Levenshtein Distance.
            if (source.Length == target.Length) return (double)m_hamDist.Compute(source, target);

            // Otherwise, the upper bound is the length of the longer string.
            if (source.Length > target.Length) return source.Length;
            if (target.Length > source.Length) return target.Length;

            return 9999;
        }

        /// <summary>
        ///     The lower bounds is the difference in length between the two strings
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        internal protected double LowerBounds(string source, string target)
        {
            // If the two strings are different lengths then the lower bounds is the difference in length.
            return Math.Abs(source.Length - target.Length);
        }

        /// <summary>
        ///     Calculate percentage similarity of two strings
        ///     <param name="source">Source String to Compare with</param>
        ///     <param name="target">Targeted String to Compare</param>
        ///     <returns>Return Similarity between two strings from 0 to 1.0</returns>
        /// </summary>
        public double? Percentage(string source, string target)
        {
            if (source == null || target == null) return 0.0;
            if (source.Length == 0 || target.Length == 0) return 0.0;
            if (source == target) return 1.0;

            double? stepsToSame = Compute(source, target);
            return 1.0 - stepsToSame / Math.Max(source.Length, target.Length);
        }
    }
}
