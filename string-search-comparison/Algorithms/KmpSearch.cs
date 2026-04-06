using System.Diagnostics;
using string_search_comparison.Interfaces;
using string_search_comparison.Models;

namespace string_search_comparison.Algorithms;

public class KmpSearch : IStringSearchStrategy
{
    public string AlgorithmName => "Knuth-Morris-Pratt (KMP)";
    public string TheoreticalComplexity => "O(n + m)";
    public string ShortComplexity => "O(n + m)";
    public string BestUseCase => "Repetitive patterns, texts with many partial matches, large inputs";

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

        if (m == 0 || m > n)
        {
            sw.Stop();
            result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            result.ElapsedNanoseconds = (long)sw.Elapsed.TotalNanoseconds;
            return result;
        }

        int[] lps = BuildLpsTable(pattern, result, stepByStep);

        if (stepByStep)
        {
            result.Steps.Add("\n=== SEARCH PHASE ===");
            result.Steps.Add($"LPS table: [{string.Join(", ", lps)}]");
            result.Steps.Add("i = text index (never goes back) | j = pattern index");
            result.Steps.Add(new string('─', 60));
        }

        int i = 0, j = 0;
        while (i < n)
        {
            result.Comparisons++;

            if (stepByStep)
                result.Steps.Add($"Compare text[{i}]='{text[i]}' with pattern[{j}]='{pattern[j]}'");

            if (text[i] == pattern[j])
            {
                i++;
                j++;
                if (stepByStep) result.Steps.Add($"  ✓ match  →  i={i}, j={j}");

                if (j == m)
                {
                    int pos = i - m;
                    result.Positions.Add(pos);
                    if (stepByStep)
                        result.Steps.Add(
                            $"  ★ PATTERN FOUND at index {pos}!  Resetting j via LPS[{j - 1}]={lps[j - 1]}");
                    j = lps[j - 1];
                }
            }
            else
            {
                if (stepByStep) result.Steps.Add($"  ✗ mismatch");

                if (j != 0)
                {
                    if (stepByStep)
                        result.Steps.Add($"  j={j} → use LPS[{j - 1}]={lps[j - 1]}  (i stays at {i})");
                    j = lps[j - 1];
                }
                else
                {
                    if (stepByStep) result.Steps.Add($"  j=0, advance i → {i + 1}");
                    i++;
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
                $"Done. {result.Positions.Count} occurrence(s) at: [{string.Join(", ", result.Positions)}]");
            result.Steps.Add($"Total comparisons: {result.Comparisons}");
        }

        return result;
    }

    private static int[] BuildLpsTable(string pattern, SearchResult result, bool stepByStep)
    {
        int m = pattern.Length;
        int[] lps = new int[m];
        lps[0] = 0;

        if (stepByStep)
        {
            result.Steps.Add("=== PHASE 1: BUILD LPS TABLE ===");
            result.Steps.Add($"Pattern: \"{pattern}\"");
            result.Steps.Add("LPS[i] = length of longest proper prefix of pattern[0..i] that is also a suffix");
            result.Steps.Add("LPS[0] = 0  (trivially)");
        }

        int len = 0, i = 1;
        while (i < m)
        {
            if (pattern[i] == pattern[len])
            {
                len++;
                lps[i] = len;
                if (stepByStep)
                    result.Steps.Add(
                        $"  pattern[{i}]='{pattern[i]}' == pattern[{len - 1}]='{pattern[len - 1]}'  → LPS[{i}] = {len}");
                i++;
            }
            else if (len != 0)
            {
                if (stepByStep)
                    result.Steps.Add(
                        $"  pattern[{i}]='{pattern[i]}' != pattern[{len}]='{pattern[len]}'  → fallback: len = LPS[{len - 1}] = {lps[len - 1]}");
                len = lps[len - 1];
            }
            else
            {
                lps[i] = 0;
                if (stepByStep)
                    result.Steps.Add(
                        $"  pattern[{i}]='{pattern[i]}' != pattern[0]='{pattern[0]}', len=0  → LPS[{i}] = 0");
                i++;
            }
        }

        if (stepByStep)
        {
            result.Steps.Add($"LPS table complete: [{string.Join(", ", lps)}]");
            result.AuxiliaryData["LPS Table"] = $"[{string.Join(", ", lps)}]";
        }

        return lps;
    }
}