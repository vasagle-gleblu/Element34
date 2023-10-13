using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Element34.StringMetrics.Similarity
{
    /// <summary>
    /// "Tanimoto Distance" is often stated as being a proper distance metric, probably 
    /// because of its confusion with Jaccard distance. The Tanimoto Distance algorithm, 
    /// is a measure of similarity between two sets. It is commonly used in data mining, 
    /// machine learning, and bioinformatics to compare the similarity of two sets of data, 
    /// regardless of their specific elements or order. The coefficient ranges from 0 to 1, 
    /// where 0 indicates no similarity between the sets (i.e., they have no common elements), 
    /// and 1 indicates that the sets are identical (i.e., all elements are the same).
    /// 
    /// <a href="https://en.wikipedia.org/wiki/Jaccard_index" />
    /// </summary>
    public class TanimotoMetric : StringMetric<double>
    {
        public override double Compute(char[] a, char[] b)
        {
            return Compute(a.ToString(), b.ToString());
        }

        public override double Compute(string source, string target)
        {
            double na = source.Length;
            double nb = target.Length;
            double nc = source.Intersect(target).Count();

            return nc / (na + nb - nc);
        }
    }
}
