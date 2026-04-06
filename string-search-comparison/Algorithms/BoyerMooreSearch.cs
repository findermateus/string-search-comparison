using System.Diagnostics;
using string_search_comparison.Interfaces;
using string_search_comparison.Models;

namespace string_search_comparison.Algorithms;

public class BoyerMooreSearch : IStringSearchStrategy
{
    public string AlgorithmName => "Boyer-Moore";
    public string TheoreticalComplexity => "O(n/m) best, O(n + m) preprocessing, O(n × m) worst";
    public string ShortComplexity => "O(n/m) best";
    public string BestUseCase => "Natural language text, large alphabets, long patterns";

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

        int[] badChar = BuildBadCharTable(pattern, result, stepByStep);
        int[] goodSuffix = BuildGoodSuffixTable(pattern, result, stepByStep);

        if (stepByStep)
        {
            result.Steps.Add("\n=== SEARCH PHASE (right-to-left comparison) ===");
            result.Steps.Add(new string('─', 60));
        }

        int s = 0;
        while (s <= n - m)
        {
            int j = m - 1;

            if (stepByStep)
                result.Steps.Add($"\n[Shift s={s}] Aligning pattern at position {s}, comparing right→left");

            while (j >= 0)
            {
                result.Comparisons++;
                if (stepByStep)
                    result.Steps.Add($"  pattern[{j}]='{pattern[j]}' vs text[{s + j}]='{text[s + j]}'");

                if (pattern[j] == text[s + j])
                {
                    if (stepByStep) result.Steps.Add("  ✓ match");
                    j--;
                }
                else
                {
                    if (stepByStep) result.Steps.Add($"  ✗ mismatch at pattern[{j}]");
                    break;
                }
            }

            if (j < 0)
            {
                result.Positions.Add(s);

                int gsShift = goodSuffix[0];
                if (stepByStep)
                    result.Steps.Add($"  ★ PATTERN FOUND at index {s}!  Shift by goodSuffix[0]={gsShift}");

                s += gsShift;
            }
            else
            {
                int bcShift = Math.Max(1, j - badChar[text[s + j]]);
                int gsShift = goodSuffix[j + 1];
                int shift = Math.Max(bcShift, gsShift);

                if (stepByStep)
                {
                    result.Steps.Add(
                        $"  Bad-char  shift: max(1, j({j}) − badChar['{text[s + j]}']({badChar[text[s + j]]})) = {bcShift}");
                    result.Steps.Add($"  Good-suff shift: goodSuffix[{j + 1}] = {gsShift}");
                    result.Steps.Add($"  Applying  shift: max({bcShift}, {gsShift}) = {shift}");
                }

                s += shift;
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

    private static int[] BuildBadCharTable(string pattern, SearchResult result, bool stepByStep)
    {
        const int alpha = 256;
        int[] bc = new int[alpha];
        for (int i = 0; i < alpha; i++) bc[i] = -1;
        for (int i = 0; i < pattern.Length; i++) bc[pattern[i]] = i;

        if (stepByStep)
        {
            result.Steps.Add("=== BAD-CHARACTER TABLE ===");
            result.Steps.Add("Maps each char to its last position in pattern (-1 = not present)");
            var entries = Enumerable.Range(0, alpha)
                .Where(c => bc[c] >= 0)
                .Select(c => $"'{(char)c}'→{bc[c]}");
            string summary = string.Join(", ", entries);
            result.Steps.Add("Entries: " + summary);
            result.AuxiliaryData["Bad-Char Table"] = summary;
        }

        return bc;
    }

    private static int[] BuildGoodSuffixTable(string pattern, SearchResult result, bool stepByStep)
    {
        int m = pattern.Length;
        int[] shift = new int[m + 1];
        int[] borderPos = new int[m + 1];

        int i = m, j = m + 1;
        borderPos[i] = j;

        while (i > 0)
        {
            while (j <= m && pattern[i - 1] != pattern[j - 1])
            {
                if (shift[j] == 0) shift[j] = j - i;
                j = borderPos[j];
            }

            i--;
            j--;
            borderPos[i] = j;
        }

        j = borderPos[0];
        for (i = 0; i <= m; i++)
        {
            if (shift[i] == 0) shift[i] = j;
            if (i == j) j = borderPos[j];
        }

        if (stepByStep)
        {
            result.Steps.Add("\n=== GOOD-SUFFIX TABLE ===");
            result.Steps.Add("shift[j+1] = how far to shift when mismatch occurs at pattern position j");
            result.Steps.Add($"Shift table: [{string.Join(", ", shift)}]");
            result.AuxiliaryData["Good-Suffix Table"] = $"[{string.Join(", ", shift)}]";
        }

        return shift;
    }
}