using System.Diagnostics;
using string_search_comparison.Interfaces;

namespace string_search_comparison.Algorithms;

public class NaiveSearch : IStringSearchStrategy
{
    public void Search(string text, string pattern)
    {
        var stopwatch = Stopwatch.StartNew();

        Console.WriteLine("NAIVE STRING SEARCH START");
        Console.WriteLine($"Text: \"{text}\"");
        Console.WriteLine($"Pattern: \"{pattern}\"");

        var textSize = text.Length;
        var patternSize = pattern.Length;

        Console.WriteLine($"Text size: {textSize}");
        Console.WriteLine($"Pattern size: {patternSize}\n");

        int i;

        for (i = 0; i <= textSize - patternSize; i++)
        {
            Console.WriteLine($"[FOR] Trying position i = {i}");

            var j = 0;

            while (j < patternSize && text[i + j] == pattern[j])
            {
                Console.WriteLine(
                    $"  [WHILE] Match at text[{i + j}] ('{text[i + j]}') == pattern[{j}] ('{pattern[j]}')");
                j++;
            }

            if (j < patternSize)
            {
                Console.WriteLine(
                    $"  [WHILE BREAK] Mismatch at text[{i + j}] ('{text[i + j]}') != pattern[{j}] ('{pattern[j]}')");
            }

            if (j == patternSize)
            {
                stopwatch.Stop();
                Console.WriteLine($"[MATCH FOUND] Pattern found at index {i}");
                Console.WriteLine($"[TIME] Execution time: {stopwatch.ElapsedMilliseconds} ms\n");
                return;
            }

            Console.WriteLine($"[CONTINUE] Moving to next position\n");
        }

        stopwatch.Stop();
        Console.WriteLine("[END] Pattern not found");
        Console.WriteLine($"[TIME] Execution time: {stopwatch.ElapsedMilliseconds} ms\n");
    }
}