using System.Linq;
using System;
using System.Text;
using SharpAvi;

namespace Element34.StringMetrics
{
    public static class HammingDistance
    {
        public static int Compute(string source, string target)
        {
            if (source.Length != target.Length)
            {
                throw new Exception("Strings must be equal length");
            }

            return source.Where((t, i) => !t.Equals(target[i])).Count();
        }
    }
}
