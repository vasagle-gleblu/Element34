using System;
using System.Linq;

namespace Element34.StringMetrics
{
    /// <summary>
    /// Jaccard index: 
    /// The Jaccard index, also known as Intersection over Union and the Jaccard similarity
    /// coefficient, is a statistic used for gauging the similarity and diversity of sample sets.
    /// <a hef="https://en.wikipedia.org/wiki/Jaccard_index" />
    /// </summary>
    public class JaccardIndex
    {
        /// <summary>
        /// Jaccard the index.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public double Compute(string source, string target)
        {
            return Convert.ToDouble(source.Intersect(target).Count()) / Convert.ToDouble(source.Union(target).Count());
        }

        /// <summary>
        /// Distances the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public double Distance(string source, string target)
        {
            return 1 - Compute(source, target);
        }
    }
}
