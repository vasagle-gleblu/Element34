using System.Linq;
using System.Text;

namespace Element34.StringMetrics
{
    public static class JaroDistance
    {
        public static double Compute(string source, string target)
        {
            var m = source.Intersect(target).Count();

            if (m == 0) return 0;

            var sourceTargetIntersetAsString = "";
            var targetSourceIntersetAsString = "";
            var sourceIntersectTarget = source.Intersect(target);
            var targetIntersectSource = target.Intersect(source);
            foreach (var character in sourceIntersectTarget) sourceTargetIntersetAsString += character;
            foreach (var character in targetIntersectSource) targetSourceIntersetAsString += character;
            var t = LevenshteinDistance.Compute(sourceTargetIntersetAsString, targetSourceIntersetAsString) / 2;
            return (m / source.Length + m / target.Length + (m - t) / m) / 3;
        }
    }
}
