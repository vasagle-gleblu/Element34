using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Element34.StringMetrics
{
    public class JaroDistance
    {
        private LevenshteinDistance m_levenshteinDist = new LevenshteinDistance();

        /// <summary>
        /// Jaro Distance is the measure of similarity between two strings. The value ranges 
        /// from 0 to 1. where 1 means the strings are equal and 0 means no similarity between the two strings.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="target">The target string.</param>
        /// <returns>
        /// A value between <c>0</c> and <c>1</c> such that <c>0</c> equates to no similarity
        /// and <c>1</c> is an exact match.
        /// </returns>
        public double Compute(string source, string target)
        {
            int m = source.Intersect(target).Count();

            if (m == 0) 
                return 0;

            string sourceTargetIntersetAsString = string.Empty;
            string targetSourceIntersetAsString = string.Empty;

            IEnumerable<char> sourceIntersectTarget = source.Intersect(target);
            IEnumerable<char> targetIntersectSource = target.Intersect(source);

            foreach (char character in sourceIntersectTarget) 
                sourceTargetIntersetAsString += character;

            foreach (char character in targetIntersectSource) 
                targetSourceIntersetAsString += character;

            double t = m_levenshteinDist.Distance(sourceTargetIntersetAsString, targetSourceIntersetAsString) / 2;

            return (m/source.Length + m/(target.Length + (m - t)) / m) / 3.0;
        }

    }
}
