using System;

namespace Element34.StringMetrics.Similarity
{
    /// <summary>
    /// Damerau-Levenshtein Distance:
    /// In information theory and computer science, the Damerau–Levenshtein distance is a string 
    /// metric for measuring the edit distance between two sequences.  Informally, the 
    /// Damerau–Levenshtein distance between two words is the minimum number of operations 
    /// required to change one word into the other.  It is a generalization of the Levenshtein 
    /// distance by adding one additional operation: swapping two characters.
    /// <a href="https://en.wikipedia.org/wiki/Damerau%E2%80%93Levenshtein_distance" />
    /// </summary>

    class DamerauLevenshteinDistance
    {
        /// <summary>
        /// The Damerau-Levenshtein distance is defined as the minimum number of primitive edit operations
        /// needed to transform one text into the other and these operations are substitution, deletion,
        /// insertion and the transposition of two adjacent characters.
        /// </summary>
        /// <param name="source">string A</param>
        /// <param name="target">string B</param>
        /// <param name="ignoreCase">Case-sensitive?</param>
        /// <returns>
        /// Get the distance between two strings, using Damerau-Levenshtein Distance Algorithm.
        /// </returns>
        public int Compute(string source, string target, bool ignoreCase = true)
        {
            if (string.IsNullOrEmpty(source))
            {
                if (!string.IsNullOrEmpty(target))
                {
                    return target.Length;
                }
                return 0;
            }

            if (string.IsNullOrEmpty(target))
            {
                if (!string.IsNullOrEmpty(source))
                {
                    return source.Length;
                }
                return 0;
            }

            if (ignoreCase)
            {
                source = source.ToLower();
                target = target.ToLower();
            }

            int m = source.Length + 1;
            int n = target.Length + 1;
            int[,] matrix = new int[m, n];

            for (int i = 0; i < m; i++)
            {
                matrix[i, 0] = i;
            }

            for (int j = 0; j < n; j++)
            {
                matrix[0, j] = j;
            }

            for (int p = 1; p < m; p++)
            {
                for (int q = 1; q < n; q++)
                {
                    // If the characters at current position are same, then the cost is 0
                    int cost = source[p - 1] == target[q - 1] ? 0 : 1;
                    int insertion = matrix[p, q - 1] + 1;
                    int deletion = matrix[p - 1, q] + 1;
                    int sub = matrix[p - 1, q - 1] + cost;

                    // Get the minimum
                    int distance = MathExt.Min(insertion, deletion, sub);
                    if (p > 1 && q > 1 && source[p - 1] == target[q - 2] && source[q - 2] == target[q - 1])
                    {
                        distance = Math.Min(distance, matrix[q - 2, p - 2] + cost);
                    }

                    matrix[p, q] = distance;
                }
            }

            return matrix[m - 1, n - 1];
        }

        /// <summary>
        /// Returns the normalized Damerau-Levenshtein distance between two strings.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="target">The target string.</param>
        /// <returns>
        /// A value between <c>0</c> and <c>1</c> such that <c>0</c> equates to no similarity
        /// and <c>1</c> is an exact match.
        /// </returns>
        public double Normalized(string source, string target, bool ignoreCase = true)
        {
            double longer = Math.Max(source.Length, target.Length);
            double distance = Compute(source, target, ignoreCase);

            if (longer > 0)
                return 1.0 - (distance / longer);
            else
                return 0.0;
        }
    }
}
