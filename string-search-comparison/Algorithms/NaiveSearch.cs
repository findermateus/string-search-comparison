using System.Diagnostics;
using string_search_comparison.Interfaces;
using string_search_comparison.Models;

namespace string_search_comparison.Algorithms;

public class NaiveSearch : IStringSearchStrategy
{
    public string AlgorithmName => "Naive (Brute Force)";
    public string TheoreticalComplexity => "O(n × m)";
    public string ShortComplexity => "O(n × m)";
    public string BestUseCase => "Small texts, simple use cases, no preprocessing needed";

    public SearchResult Search(string text, string pattern, bool stepByStep = false)
    {
        var result = new SearchResult
        {
            AlgorithmName = AlgorithmName,
            TheoreticalComplexity = TheoreticalComplexity,
            ShortComplexity = ShortComplexity,
            TextLength = text.Length,
            PatternLength = pattern.Length
        };

        var sw = Stopwatch.StartNew();
        int n = text.Length;
        int m = pattern.Length;

        if (stepByStep)
        {
            result.Steps.Add("=== NAIVE (BRUTE FORCE) SEARCH ===");
            result.Steps.Add($"Text length n = {n} | Pattern length m = {m}");
            result.Steps.Add("Strategy: slide a window of size m over the text, compare char by char left→right");
            result.Steps.Add(new string('─', 60));
        }

        if (m > 0 && m <= n)
        {
            for (int i = 0; i <= n - m; i++)
            {
                if (stepByStep)
                {
                    result.Steps.Add($"\n[Window i={i}]  {ShowAlignment(text, pattern, i)}");
                }

                int j = 0;
                while (j < m)
                {
                    result.Comparisons++;
                    char tc = text[i + j];
                    char pc = pattern[j];

                    if (tc == pc)
                    {
                        if (stepByStep)
                            result.Steps.Add($"  text[{i + j}]='{tc}' == pattern[{j}]='{pc}'  ✓ match");
                        j++;
                    }
                    else
                    {
                        if (stepByStep)
                            result.Steps.Add(
                                $"  text[{i + j}]='{tc}' != pattern[{j}]='{pc}'  ✗ mismatch → shift window");
                        break;
                    }
                }

                if (j == m)
                {
                    result.Positions.Add(i);
                    if (stepByStep)
                        result.Steps.Add($"  ★ PATTERN FOUND at index {i}!");
                }
            }
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        result.ElapsedNanoseconds = (long)sw.Elapsed.TotalNanoseconds;

        if (stepByStep)
        {
            result.Steps.Add(new string('─', 60));
            result.Steps.Add(
                $"Done. {result.Positions.Count} occurrence(s) found at: [{string.Join(", ", result.Positions)}]");
            result.Steps.Add($"Total comparisons: {result.Comparisons}");
        }

        return result;
    }

    private static string ShowAlignment(string text, string pattern, int offset)
    {
        int start = Math.Max(0, offset - 2);
        int end = Math.Min(text.Length, offset + pattern.Length + 2);
        string prefix = start > 0 ? "..." : "";
        string suffix = end < text.Length ? "..." : "";
        string textLine = prefix + text[start..end] + suffix;
        int padLen = (start > 0 ? 3 : 0) + (offset - start);
        string patLine = new string(' ', padLen) + pattern;
        return $"\n    Text:    {textLine}\n    Pattern: {patLine}";
    }
}