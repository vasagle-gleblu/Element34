using System;
using System.Collections.Generic;
using System.Linq;

namespace Element34.StringMetrics.Similarity
{

    [Flags]
    public enum StringComparisonOption
    {
        UseHammingDistance = 1,
        UseJaccardDistance = 2,
        UseJaroDistance = 4,
        UseJaroWinklerDistance = 8,
        UseLevenshteinDistance = 16,
        UseLongestCommonSubsequence = 32,
        UseLongestCommonSubstring = 64,
        UseNormalizedLevenshteinDistance = 128,
        UseOverlapCoefficient = 256,
        UseRatcliffObershelpSimilarity = 512,
        UseSorensenDiceDistance = 1024,
        UseTanimotoCoefficient = 2048,
        CaseSensitive = 4096
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
        public static bool IsSimilar(string source, string target,
    StringComparisonTolerance tolerance,
    StringComparisonOption options)
        {
            var diff = DiffPercent(source, target, options);

            switch (tolerance)
            {
                case StringComparisonTolerance.Extreme:
                    return diff < 0.05;
                case StringComparisonTolerance.VeryStrong:
                    return diff < 0.1;
                case StringComparisonTolerance.Strong:
                    return diff < 0.2;
                case StringComparisonTolerance.ModeratelyStrong:
                    return diff < 0.35;
                case StringComparisonTolerance.Normal:
                    return diff < 0.5;
                case StringComparisonTolerance.ModeratelyWeak:
                    return diff < 0.65;
                case StringComparisonTolerance.Weak:
                    return diff < 0.8;
                case StringComparisonTolerance.VeryWeak:
                    return diff < 0.9;
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
                    comparisonResults.Add(HammingDistance.Compute(source, target) / target.Length);

            // Min: 0    Max: 1
            if (options.HasFlag(StringComparisonOption.UseJaccardDistance))
                comparisonResults.Add(JaccardDistance.Compute(source, target));

            // Min: 0    Max: 1
            if (options.HasFlag(StringComparisonOption.UseJaroDistance))
                comparisonResults.Add(JaroDistance.Compute(source, target));

            // Min: 0    Max: 1
            if (options.HasFlag(StringComparisonOption.UseJaroWinklerDistance))
                comparisonResults.Add(JaroWinklerDistance.Compute(source, target));

            // Min: 0    Max: LevenshteinDistanceUpperBounds - LevenshteinDistanceLowerBounds
            // Min: LevenshteinDistanceLowerBounds    Max: LevenshteinDistanceUpperBounds
            if (options.HasFlag(StringComparisonOption.UseNormalizedLevenshteinDistance))
                comparisonResults.Add(Convert.ToDouble(LevenshteinDistance.Normalized(source, target)) /
                                      Convert.ToDouble(Math.Max(source.Length, target.Length) -
                                                       LevenshteinDistance.LowerBounds(source, target)));
            else if (options.HasFlag(StringComparisonOption.UseLevenshteinDistance))
                comparisonResults.Add(1 - LevenshteinDistance.Percentage(source, target));

            if (options.HasFlag(StringComparisonOption.UseLongestCommonSubsequence))
                comparisonResults.Add(1 -
                                      Convert.ToDouble(LongestCommonSequence.Compute(source, target).Length /
                                                       Convert.ToDouble(Math.Min(source.Length, target.Length))));

            if (options.HasFlag(StringComparisonOption.UseLongestCommonSubstring))
                comparisonResults.Add(1 -
                                      Convert.ToDouble(LongestCommonSubstring.Compute(source, target).Length /
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
