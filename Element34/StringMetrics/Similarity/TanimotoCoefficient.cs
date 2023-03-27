using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Element34.StringMetrics.Similarity
{
    public static class TanimotoCoefficient
    {
        public static double Compute(string source, string target)
        {
            double na = source.Length;
            double nb = target.Length;
            double nc = source.Intersect(target).Count();

            return nc / (na + nb - nc);
        }
    }
}
