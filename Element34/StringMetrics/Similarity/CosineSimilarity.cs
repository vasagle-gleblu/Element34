using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Element34.StringMetrics.Similarity
{
    /// <summary>
    /// Cosine Similarity:
    /// In NLP, Cosine similarity is a metric used to measure how similar the documents are 
    /// irrespective of their size.  A word is represented into a vector form. The text documents 
    /// are represented in n-dimensional vector space.  Mathematically, it calculates the 
    /// cosine of the angle between two vectors projected in the n-dimensional space.
    /// <a href="https://en.wikipedia.org/wiki/Cosine_similarity" />
    /// 
    /// This algorithm was converted from Java to C# from the following repository.
    /// <a href="https://gist.github.com/vamsigp/e2c7857a9aae4b653778" /> 
    /// </summary>


    public class CosineSimilarity :StringComparison<string>, IStringEncoder<string>
    {
        /// <summary>
        /// The cosine similarity is a measure of similarity between two words.
        /// </summary>
        /// <param name="source">string A</param>
        /// <param name="target">string B</param>
        /// <returns>The cosine similarity of source and target</returns>
        public double Compute(string source, string target)
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

        /// <summary>
        /// The cosine distance is commonly used as the complement of cosine similarity.
        /// </summary>
        /// <param name="source">string A</param>
        /// <param name="target">string B</param>
        /// <returns>The cosine distance of source and target</returns>
        public double Distance(string source, string target)
        {
            return 1 - Compute(source, target);
        }

        /// <summary>
        /// Term frequency–inverse document frequency; TF-IDF.
        /// </summary>
        /// <param name="terms">term values to analyze</param>
        /// <returns>A dictionary containing unique terms and their frequency</returns>
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
