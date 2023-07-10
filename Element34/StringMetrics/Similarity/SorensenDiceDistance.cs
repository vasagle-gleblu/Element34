using System.Linq;
using System;
using System.Text;

namespace Element34.StringMetrics
{
    public static class SorensenDiceDistance
    {
        public static double Normalized(string source, string target)
        {
            return 1 - Compute(source, target);
        }

        public static double Compute(string source, string target)
        {   // Sorensen-Dice Index
            int count = source.Intersect(target).Count();
            int TotalLength = source.Length + target.Length;

            return 2 * Convert.ToDouble(count) / Convert.ToDouble(TotalLength);
        }
    }
}
