using System;

namespace Element34.StringMetrics.Similarity
{
    /// <summary>
    /// Weighted Levenshtein:
    /// The algorithm is a modification of the traditional Levenshtein 
    /// distance algorithm, which is used to calculate the minimum number of edit operations 
    /// (insertions, deletions, or substitutions) required to transform one string into another. 
    /// It introduces additional weights or costs for each type of edit operation, allowing for 
    /// more flexibility in measuring the similarity between two strings. This algorithm is usually 
    /// used for keyboard typing auto-correction and optical character recognition 
    /// (OCR) applications.
    /// </summary>
    public class WeightedLevenshteinMetric : StringMetric<double?>
    {
        private readonly decimal delete;
        private readonly decimal insert;
        private readonly decimal substitute;

        public WeightedLevenshteinMetric(decimal delete, decimal insert, decimal substitute)
        {
            this.delete = delete;
            this.insert = insert;
            this.substitute = substitute;
        }

        public WeightedLevenshteinMetric() : this(1.0m, 1.0m, 1.0m)
        { }

        public override double? Compute(string a, string b)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
                return null;

            if (a == b)
                return 0;

            return (double)WeightedLevenshtein((a.ToCharArray(), b.ToCharArray()), (delete, insert, substitute));
        }

        public override double? Compute(char[] a, char[] b)
        {
            return Compute(a.ToString(), b.ToString());
        }

        private static decimal WeightedLevenshtein((char[], char[]) ct, (decimal, decimal, decimal) w)
        {
            decimal[,] matrix = new decimal[ct.Item1.Length + 1, ct.Item2.Length + 1];

            for (int r = 0; r <= ct.Item1.Length; r++)
                matrix[r, 0] = w.Item1 * r;

            for (int c = 0; c <= ct.Item2.Length; c++)
                matrix[0, c] = w.Item2 * c;

            for (int r = 1; r <= ct.Item1.Length; r++)
            {
                for (int c = 1; c <= ct.Item2.Length; c++)
                {
                    if (ct.Item1[r - 1] == ct.Item2[c - 1])
                        matrix[r, c] = matrix[r - 1, c - 1];
                    else
                        matrix[r, c] = MathExtensions.Min(matrix[r - 1, c] + w.Item1, matrix[r, c - 1] + w.Item2, matrix[r - 1, c - 1] + w.Item3);
                }
            }

            return matrix[ct.Item1.Length, ct.Item2.Length];
        }
    }
}
