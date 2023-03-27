using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Element34.StringMetrics.Similarity
{
    public static class CosineSimilarity
    {
        /**
         * @param source 
         * @param target 
         * @return cosine similarity of source and target
         */
        public static double Compute(string source, string target)
        {
            //Get vectors
            Dictionary<string, int> src = getTermFrequencyMap(Regex.Split(source, "\\W+"));
            Dictionary<string, int> tgt = getTermFrequencyMap(Regex.Split(target, "\\W+"));

            //Get unique words from both sequences
            HashSet<string> intersection = new HashSet<string>(src.Keys.Intersect(tgt.Keys));

            double dotProduct = 0, magnitudeS = 0, magnitudeT = 0;

            //Calculate dot product
            foreach (string item in intersection)
            {
                dotProduct += src.GetValue(item) * tgt.GetValue(item);
            }

            //Calculate magnitude a
            foreach (string item in src.Keys)
            {
                magnitudeS += Math.Pow(src.GetValue(item), 2);
            }

            //Calculate magnitude b
            foreach (string item in tgt.Keys)
            {
                magnitudeT += Math.Pow(tgt.GetValue(item), 2);
            }

            //return cosine similarity
            return dotProduct / Math.Sqrt(magnitudeS * magnitudeT);
        }


        /**
        * @param terms values to analyze
        * @return a map containing unique 
        * terms and their frequency
        */
        private static Dictionary<string, int> getTermFrequencyMap(String[] terms)
        {
            Dictionary<string, int> termFrequencyMap = new Dictionary<string, int>();
            foreach (string term in terms)
            {
                int? n = termFrequencyMap.GetValue(term);
                n = (n == null) ? 1 : ++n;
                termFrequencyMap.Add(term, (int)n);
            }

            return termFrequencyMap;
        }
    }
}
