using System;
using System.Collections.Generic;
using System.Linq;

namespace Element34.StringMetrics.Similarity
{

    [Flags]
    public enum StringComparisonOption
    {
        UseCosineSimilarity                         = 2^0,
        UseDamerauLevenshteinDistance               = 2^1,
        UseHammingDistance                          = 2^2,
        UseJaccardDistance                          = 2^3,
        UseJaroDistance                             = 2^4,
        UseJaroWinklerDistance                      = 2^5,
        UseLongestCommonSubsequence                 = 2^6,
        UseLongestCommonSubstring                   = 2^7,
        UseNormalizedLevenshteinDistance            = 2^8,
        UseNGramDistance                            = 2^9,
        UseOverlapCoefficient                       = 2^10,
        UseRatcliffObershelpSimilarity              = 2^11,
        UseSorensenDiceDistance                     = 2^12,
        UseTanimotoCoefficient                      = 2^13,
        UseWeightedLevenshteinDistance              = 2^14,
        CaseSensitive                               = 2^15
    }

    public enum StringComparisonTolerance
    {
        Extreme,
        VeryStrong,
        Strong,
        ModeratelyStrong,
        Normal,
        ModeratelyWeak,
        Weak,
        VeryWeak,
        Lame
    }

    public static class CheckSimilarity
    {
        private static HammingDistance m_hamDist = new HammingDistance();
        private static JaroDistance m_jaroDist = new JaroDistance();
        private static JaccardIndex m_jaccardIndex = new JaccardIndex();
        private static JaroWinklerDistance m_jaroWinklerDist = new JaroWinklerDistance();
        private static LevenshteinDistance m_levenshteinDist = new LevenshteinDistance();
        private static LongestCommonSequence m_longestCommonSeq = new LongestCommonSequence();
        private static LongestCommonSubstring m_longestCommonSub = new LongestCommonSubstring();


        public static bool IsSimilar(string source, string target, StringComparisonTolerance tolerance, StringComparisonOption options)
        {
            var diff = DiffPercent(source, target, options);

            switch (tolerance)
            {
                case StringComparisonTolerance.Extreme:
                    return diff < 0.05;
                case StringComparisonTolerance.VeryStrong:
                    return diff < 0.2;
                case StringComparisonTolerance.Strong:
                    return diff < 0.35;
                case StringComparisonTolerance.ModeratelyStrong:
                    return diff < 0.4;
                case StringComparisonTolerance.Normal:
                    return diff < 0.5;
                case StringComparisonTolerance.ModeratelyWeak:
                    return diff < 0.6;
                case StringComparisonTolerance.Weak:
                    return diff < 0.75;
                case StringComparisonTolerance.VeryWeak:
                    return diff < 0.8;
                case StringComparisonTolerance.Lame:
                    return diff < 0.95;
                default:
                    return false;
            }
        }

        public static double DiffPercent(string source, string target, StringComparisonOption options)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
                return 1;

            var comparisonResults = new List<double>();

            if (!options.HasFlag(StringComparisonOption.CaseSensitive))
            {
                source = source.ToUpper();
                target = target.ToUpper();
            }

            // Min: 0    Max: source.Length = target.Length
            if (options.HasFlag(StringComparisonOption.UseHammingDistance))
                if (source.Length == target.Length)
                    comparisonResults.Add(m_hamDist.Compute(source, target) / target.Length);

            // Min: 0    Max: 1
            if (options.HasFlag(StringComparisonOption.UseJaccardDistance))
                comparisonResults.Add(m_jaccardIndex.Compute(source, target));

            // Min: 0    Max: 1
            if (options.HasFlag(StringComparisonOption.UseJaroDistance))
                comparisonResults.Add(m_jaroDist.Compute(source, target));

            // Min: 0    Max: 1
            if (options.HasFlag(StringComparisonOption.UseJaroWinklerDistance))
                comparisonResults.Add(m_jaroWinklerDist.Compute(source, target));

            // Min: 0    Max: LevenshteinDistanceUpperBounds - LevenshteinDistanceLowerBounds
            // Min: LevenshteinDistanceLowerBounds    Max: LevenshteinDistanceUpperBounds
            if (options.HasFlag(StringComparisonOption.UseNormalizedLevenshteinDistance))
                comparisonResults.Add(Convert.ToDouble(m_levenshteinDist.Normalized(source, target)) /
                                      Convert.ToDouble(Math.Max(source.Length, target.Length) -
                                                       m_levenshteinDist.LowerBounds(source, target)));
            else if (options.HasFlag(StringComparisonOption.UseNormalizedLevenshteinDistance))
                comparisonResults.Add(1 - m_levenshteinDist.Percentage(source, target));

            if (options.HasFlag(StringComparisonOption.UseLongestCommonSubsequence))
                comparisonResults.Add(1 -
                                      Convert.ToDouble(m_longestCommonSeq.Compute(source, target).Length /
                                                       Convert.ToDouble(Math.Min(source.Length, target.Length))));

            if (options.HasFlag(StringComparisonOption.UseLongestCommonSubstring))
                comparisonResults.Add(1 -
                                      Convert.ToDouble(m_longestCommonSub.Compute(source, target).Length /
                                                       Convert.ToDouble(Math.Min(source.Length, target.Length))));

            // Min: 0    Max: 1
            if (options.HasFlag(StringComparisonOption.UseSorensenDiceDistance))
                comparisonResults.Add(SorensenDiceDistance.Compute(source, target));

            // Min: 0    Max: 1
            if (options.HasFlag(StringComparisonOption.UseOverlapCoefficient))
                comparisonResults.Add(1 - OverlapCoefficient.Compute(source, target));

            // Min: 0    Max: 1
            if (options.HasFlag(StringComparisonOption.UseRatcliffObershelpSimilarity))
                comparisonResults.Add(1 - RatcliffObershelpSimilarity.Compute(source, target));

            return comparisonResults.Average();
        }

        public static double Similarity(string source, string target, StringComparisonOption options)
        {
            return 1 - DiffPercent(source, target, options);
        }

    }
}
