using System.Linq;
using System;
using System.Text;

namespace Element34.StringMetrics
{
    public static class RatcliffObershelpSimilarity
    {
        public static double Compute(string source, string target)
        {
            return 2 * Convert.ToDouble(source.Intersect(target).Count()) / Convert.ToDouble(source.Length + target.Length);
        }
    }
}
