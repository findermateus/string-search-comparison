using System.Diagnostics;
using string_search_comparison.Interfaces;

namespace string_search_comparison.Algorithms;

public class KmpSearch : IStringSearchStrategy
{
    public void Search(string text, string pattern)
    {
        var stopwatch = Stopwatch.StartNew();

        Console.WriteLine("KMP STRING SEARCH START");
        Console.WriteLine($"Text: \"{text}\"");
        Console.WriteLine($"Pattern: \"{pattern}\"");

        var textSize = text.Length;
        var patternSize = pattern.Length;

        Console.WriteLine($"Text size: {textSize}");
        Console.WriteLine($"Pattern size: {patternSize}\n");

        var m = 0;
        var i = 0;

        int[] t = new int[pattern.Length];

        Console.WriteLine("[INIT] Building T table with -1");

        for (var j = 0; j < t.Length; j++)
        {
            t[j] = -1;
            Console.WriteLine($"  T[{j}] = -1");
        }

        Console.WriteLine();

        while (m + i < text.Length)
        {
            Console.WriteLine($"[WHILE] m = {m}, i = {i}, comparing text[{m + i}] ('{text[m + i]}') with pattern[{i}] ('{pattern[i]}')");

            if (pattern[i] == text[m + i])
            {
                Console.WriteLine("  [MATCH] Characters match");

                if (i == pattern.Length - 1)
                {
                    stopwatch.Stop();
                    Console.WriteLine($"[MATCH FOUND] Pattern found at index {m}");
                    Console.WriteLine($"[TIME] Execution time: {stopwatch.ElapsedMilliseconds} ms\n");
                    return;
                }

                i++;
                Console.WriteLine($"  [ADVANCE] i -> {i}\n");

                continue;
            }

            Console.WriteLine("  [MISMATCH] Characters do not match");

            if (t[i] > -1)
            {
                Console.WriteLine($"  [T TABLE] t[{i}] = {t[i]}");

                var oldM = m;
                var oldI = i;

                m = m + i - t[i];
                i = t[i];

                Console.WriteLine($"  [SHIFT] m: {oldM} -> {m}, i: {oldI} -> {i}\n");

                continue;
            }

            Console.WriteLine("  [RESET] t[i] == -1, moving to next position");

            m += 1;
            i = 0;

            Console.WriteLine($"  [MOVE] m -> {m}, i -> {i}\n");
        }

        stopwatch.Stop();
        Console.WriteLine("[END] Pattern not found");
        Console.WriteLine($"[TIME] Execution time: {stopwatch.ElapsedMilliseconds} ms\n");
    }
}