using System;
using System.Text;

namespace Element34.StringMetrics
{
    public class WeightedLevenshteinDistance
    {
        public static double Compute(string source, string target)
        {
            source = source.ToUpper();
            target = target.ToUpper();

            double[,] matrix = new double[source.Length + 1, target.Length + 1];

            for (int i = 1; i <= source.Length; i++)
            {
                matrix[i, 0] = i;
            }

            for (int i = 1; i <= target.Length; i++)
            {
                matrix[0, i] = i;
            }

            for (int i = 1; i <= source.Length; i++)
            {
                for (int j = 1; j <= target.Length; j++)
                {
                    double distance_replace = matrix[(i - 1), (j - 1)];
                    if (source[i - 1] != target[j - 1])
                    {
                        // Cost of replace
                        distance_replace += Math.Abs((float)(source[i - 1]) - target[j - 1]) / ('Z' - 'A');
                    }

                    // Cost of remove = 1 
                    double distance_remove = matrix[(i - 1), j] + 1;
                    // Cost of add = 1
                    double distance_add = matrix[i, (j - 1)] + 1;

                    matrix[i, j] = MathExt.Min(distance_replace, distance_add, distance_remove);
                }
            }

            return matrix[source.Length, target.Length];
        }
    }
}
