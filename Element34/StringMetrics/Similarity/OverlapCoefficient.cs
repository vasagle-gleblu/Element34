using System.Linq;
using System;
using System.Text;

namespace Element34.StringMetrics
{
    public static class OverlapCoefficient
    {
        public static double Compute(string source, string target)
        {
            return Convert.ToDouble(source.Intersect(target).Count()) /
                   Convert.ToDouble(Math.Min(source.Length, target.Length));
        }
    }
}
