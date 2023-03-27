using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Element34.StringMetrics
{
    public interface ISimilarityMetric
    {
        int ComputeMetric(string source, string target);
    }

    public interface ISimilarityCoefficient
    {
        double ComputeCoefficient(string source, string target);
    }
}
