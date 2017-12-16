﻿using System;
using System.Collections.Generic;
using System.Linq;
using StringCompration.Core.Enums;

namespace StringCompration.Core
{
    public static partial class ComparisonMetrics
    {
        public static bool IsSimilar(this string source, string target, StringComparisonTolerance tolerance, StringComparisonOption options)
        {
            var diff = DiffPercent(source, target, options);

            switch (tolerance)
            {
                case StringComparisonTolerance.Strong:
                    return diff < 0.25;
                case StringComparisonTolerance.Normal:
                    return diff < 0.5;
                case StringComparisonTolerance.Weak:
                    return diff < 0.75;
                default:
                    return false;
            }
        }

        public static double DiffPercent(this string source, string target, StringComparisonOption options)
        {
            var comparisonResults = new List<double>();

            if (!options.HasFlag(StringComparisonOption.CaseSensitive))
            {
                source = source.Capitalize();
                target = target.Capitalize();
            }

            // Min: 0    Max: source.Length = target.Length
            if (options.HasFlag(StringComparisonOption.UseHammingDistance))
            {
                if (source.Length == target.Length)
                {
                    comparisonResults.Add(source.HammingDistance(target) / target.Length);
                }
            }

            // Min: 0    Max: 1
            if (options.HasFlag(StringComparisonOption.UseJaccardDistance))
            {
                comparisonResults.Add(source.JaccardDistance(target));
            }

            // Min: 0    Max: 1
            if (options.HasFlag(StringComparisonOption.UseJaroDistance))
            {
                comparisonResults.Add(source.JaroDistance(target));
            }

            // Min: 0    Max: 1
            if (options.HasFlag(StringComparisonOption.UseJaroWinklerDistance))
            {
                comparisonResults.Add(source.JaroWinklerDistance(target));
            }

            // Min: 0    Max: LevenshteinDistanceUpperBounds - LevenshteinDistanceLowerBounds
            // Min: LevenshteinDistanceLowerBounds    Max: LevenshteinDistanceUpperBounds
            if (options.HasFlag(StringComparisonOption.UseNormalizedLevenshteinDistance))
            {
                comparisonResults.Add(Convert.ToDouble(source.NormalizedLevenshteinDistance(target)) / Convert.ToDouble((Math.Max(source.Length, target.Length) - source.LevenshteinDistanceLowerBounds(target))));
            }
            else if (options.HasFlag(StringComparisonOption.UseLevenshteinDistance))
            {
                comparisonResults.Add(1 - source.LevenshteinDistancePercentage(target));
            }

            if (options.HasFlag(StringComparisonOption.UseLongestCommonSubsequence))
            {
                comparisonResults.Add(1 - Convert.ToDouble((source.LongestCommonSubsequence(target).Length) / Convert.ToDouble(Math.Min(source.Length, target.Length))));
            }

            if (options.HasFlag(StringComparisonOption.UseLongestCommonSubstring))
            {
                comparisonResults.Add(1 - Convert.ToDouble((source.LongestCommonSubstring(target).Length) / Convert.ToDouble(Math.Min(source.Length, target.Length))));
            }

            // Min: 0    Max: 1
            if (options.HasFlag(StringComparisonOption.UseSorensenDiceDistance))
            {
                comparisonResults.Add(source.SorensenDiceDistance(target));
            }

            // Min: 0    Max: 1
            if (options.HasFlag(StringComparisonOption.UseOverlapCoefficient))
            {
                comparisonResults.Add(1 - source.OverlapCoefficient(target));
            }

            // Min: 0    Max: 1
            if (options.HasFlag(StringComparisonOption.UseRatcliffObershelpSimilarity))
            {
                comparisonResults.Add(1 - source.RatcliffObershelpSimilarity(target));
            }

            return comparisonResults.Average();
        }

        public static double Similarity(this string source, string target, StringComparisonOption options)
        {
            return 1 - DiffPercent(source, target, options);
        }
    }
}
