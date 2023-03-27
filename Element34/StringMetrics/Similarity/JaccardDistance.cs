using System;
using System.Linq;

namespace Element34.StringMetrics
{
    public static class JaccardDistance
    {
        public static double Compute(string source, string target)
        {
            return 1 - JaccardIndex(source, target);
        }

        public static double JaccardIndex(string source, string target)
        {
            return Convert.ToDouble(source.Intersect(target).Count()) /
                   Convert.ToDouble(source.Union(target).Count());
        }
    }
}
