using System.Diagnostics;
using string_search_comparison.Interfaces;
using string_search_comparison.Models;

namespace string_search_comparison.Algorithms;

public class RabinKarpSearch : IStringSearchStrategy
{
    public string AlgorithmName => "Rabin-Karp";
    public string TheoreticalComplexity => "O(n + m) average, O(n × m) worst case";
    public string ShortComplexity => "O(n+m) avg";
    public string BestUseCase => "Multiple-pattern search, plagiarism detection, DNA analysis";

    private const int Base = 65536;
    private const int Modulus = 1000003;

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

        if (stepByStep)
        {
            result.Steps.Add("=== RABIN-KARP SEARCH ===");
            result.Steps.Add($"Base (d) = {Base}  |  Modulus (q, prime) = {Modulus}");
            result.Steps.Add($"Hash(s) = (s[0]×d^(m-1) + s[1]×d^(m-2) + … + s[m-1]) mod q");
            result.Steps.Add(new string('─', 60));
        }

        long h = 1;
        for (int i = 0; i < m - 1; i++)
            h = (h * Base) % Modulus;

        if (stepByStep)
            result.Steps.Add($"h = {Base}^({m}-1) mod {Modulus} = {h}  (used to remove leading char)");

        long patHash = 0, txtHash = 0;
        for (int i = 0; i < m; i++)
        {
            patHash = (Base * patHash + pattern[i]) % Modulus;
            txtHash = (Base * txtHash + text[i]) % Modulus;
        }

        if (stepByStep)
        {
            result.Steps.Add($"\nPattern hash(\"{pattern}\") = {patHash}");
            result.Steps.Add($"Initial window hash(text[0..{m - 1}] = \"{text[..m]}\") = {txtHash}");
            result.AuxiliaryData["Pattern Hash (mod q)"] = patHash.ToString();
        }

        for (int i = 0; i <= n - m; i++)
        {
            result.Comparisons++;

            if (stepByStep)
            {
                string window = text.Length <= i + m ? text[i..] : text[i..(i + m)];
                result.Steps.Add($"\n[Window i={i}] \"{window}\"  hash={txtHash}  pattern_hash={patHash}");
            }

            if (txtHash == patHash)
            {
                if (stepByStep)
                    result.Steps.Add("  Hash match! Verifying characters one by one…");

                bool match = true;
                for (int j = 0; j < m; j++)
                {
                    result.Comparisons++;
                    if (text[i + j] != pattern[j])
                    {
                        match = false;
                        result.SpuriousHits++;
                        if (stepByStep)
                            result.Steps.Add(
                                $"  Spurious hit! text[{i + j}]='{text[i + j]}' != pattern[{j}]='{pattern[j]}'");
                        break;
                    }
                }

                if (match)
                {
                    result.Positions.Add(i);
                    if (stepByStep) result.Steps.Add($"  ★ PATTERN FOUND at index {i}!");
                }
            }
            else
            {
                if (stepByStep) result.Steps.Add($"  Hashes differ ({txtHash} ≠ {patHash}), skip");
            }

            if (i < n - m)
            {
                txtHash = (Base * (txtHash - text[i] * h) + text[i + m]) % Modulus;
                if (txtHash < 0) txtHash += Modulus;

                if (stepByStep)
                    result.Steps.Add($"  Roll: remove text[{i}]='{text[i]}' (val={(int)text[i]}), " +
                                     $"add text[{i + m}]='{text[i + m]}' (val={(int)text[i + m]})  → new hash={txtHash}");
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
            result.Steps.Add($"Spurious hits (hash collisions): {result.SpuriousHits}");
            result.Steps.Add($"Total comparisons (hash + char): {result.Comparisons}");
        }

        return result;
    }
}