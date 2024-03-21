using System;
using System.Collections.Generic;
using System.Linq;

namespace Element34.StringMetrics.Similarity
{
    // Placeholder
    public class CosineMetric
    {
        private Dictionary<string, double[]> wordVectors = new Dictionary<string, double[]>();

        public double Compute(string wordA, string wordB)
        {
            if (!wordVectors.ContainsKey(wordA) || !wordVectors.ContainsKey(wordB))
                throw new ArgumentException("Words not found in the vocabulary.");

            double[] vectorA = wordVectors[wordA];
            double[] vectorB = wordVectors[wordB];

            return CalculateCosineSimilarity(vectorA, vectorB);
        }

        private double CalculateCosineSimilarity(double[] vectorA, double[] vectorB)
        {
            double dotProduct = DotProduct(vectorA, vectorB);
            double magnitudeA = Magnitude(vectorA);
            double magnitudeB = Magnitude(vectorB);

            if (magnitudeA == 0 || magnitudeB == 0)
                return 0;  // Handle division by zero

            return dotProduct / (magnitudeA * magnitudeB);
        }

        private double DotProduct(double[] vectorA, double[] vectorB)
        {
            return vectorA.Select((a, i) => a * vectorB[i]).Sum();
        }

        private double Magnitude(double[] vector)
        {
            return Math.Sqrt(vector.Select(a => a * a).Sum());
        }
    }
}