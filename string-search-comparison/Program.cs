using System.Text;
using string_search_comparison.Algorithms;
using string_search_comparison.Context;
using string_search_comparison.Interfaces;
using string_search_comparison.Models;

Console.OutputEncoding = Encoding.UTF8;

var algorithms = new List<IStringSearchStrategy>
{
    new NaiveSearch(),
    new RabinKarpSearch(),
    new KmpSearch(),
    new BoyerMooreSearch()
};

var context = new SearchContext(algorithms[0]);
var loadedFiles = new List<string>();
string text = string.Empty;
string pattern = string.Empty;
int algoIdx = 0;

bool running = true;
while (running)
{
    Console.Clear();
    PrintHeader();
    PrintStatus();
    PrintMenu();

    string? choice = Console.ReadLine()?.Trim();
    Console.Clear();

    switch (choice)
    {
        case "1": LoadFiles(); break;
        case "2": EnterText(); break;
        case "3": SetPattern(); break;
        case "4": ChooseAlgorithm(); break;
        case "5": RunNormal(); break;
        case "6": RunStepByStep(); break;
        case "7": CompareAll(); break;
        case "8": ClearText(); break;
        case "0": running = false; break;
        default:
            Console.WriteLine("Invalid option.");
            Pause();
            break;
    }
}

Console.WriteLine("\nGoodbye!\n");

void PrintHeader()
{
    Console.WriteLine("=== String Search Algorithm Comparison ===\n");
}

void PrintStatus()
{
    Console.Write("Text:      ");
    if (text.Length == 0)
        Console.WriteLine("[not loaded]");
    else
        Console.WriteLine($"{text.Length} char(s)" +
                          (loadedFiles.Count > 0 ? $" from {loadedFiles.Count} file(s)" : " (manual input)"));

    Console.Write("Pattern:   ");
    Console.WriteLine(pattern.Length == 0 ? "[not set]" : $"\"{pattern}\"");

    Console.WriteLine($"Algorithm: {algorithms[algoIdx].AlgorithmName}");
    Console.WriteLine();
}

void PrintMenu()
{
    Console.WriteLine("-- Input --");
    Console.WriteLine("1. Load text from .txt file(s)");
    Console.WriteLine("2. Enter text manually");
    Console.WriteLine("3. Set search pattern");
    Console.WriteLine();
    Console.WriteLine("-- Algorithm --");
    Console.WriteLine("4. Select algorithm");
    Console.WriteLine();
    Console.WriteLine("-- Execute --");
    Console.WriteLine("5. Run search (normal)");
    Console.WriteLine("6. Run search (step-by-step)");
    Console.WriteLine("7. Compare all algorithms");
    Console.WriteLine();
    Console.WriteLine("8. Clear loaded text");
    Console.WriteLine("0. Exit");
    Console.WriteLine();
    Console.Write("Option: ");
}

void Pause()
{
    Console.Write("\nPress any key to continue...");
    Console.ReadKey(intercept: true);
}

void LoadFiles()
{
    Console.WriteLine("=== Load Text From File(s) ===");
    Console.WriteLine("Enter file path(s), one per line. Empty line to finish.\n");

    var sb = new StringBuilder();
    while (true)
    {
        Console.Write("Path: ");
        string? p = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(p)) break;

        if (!File.Exists(p))
        {
            Console.WriteLine($"File not found: {p}");
            continue;
        }

        string content = File.ReadAllText(p, Encoding.UTF8);
        sb.Append(content);
        if (!loadedFiles.Contains(p)) loadedFiles.Add(p);
        Console.WriteLine($"Loaded: {p} ({content.Length} chars)");
    }

    if (sb.Length > 0)
    {
        text = sb.ToString();
        Console.WriteLine($"\nTotal: {text.Length} character(s) loaded.");
    }
    else
    {
        Console.WriteLine("No files loaded.");
    }

    Pause();
}

void EnterText()
{
    Console.WriteLine("=== Enter Text Manually ===\n");
    Console.Write("Text: ");
    text = Console.ReadLine() ?? string.Empty;
    loadedFiles.Clear();
    Console.WriteLine($"\nText set ({text.Length} chars).");
    Pause();
}

void SetPattern()
{
    Console.WriteLine("=== Set Search Pattern ===\n");
    Console.Write("Pattern: ");
    pattern = Console.ReadLine() ?? string.Empty;
    Console.WriteLine($"\nPattern set: \"{pattern}\"");
    Pause();
}

void ChooseAlgorithm()
{
    Console.WriteLine("=== Select Algorithm ===\n");
    for (int i = 0; i < algorithms.Count; i++)
    {
        string mark = i == algoIdx ? "> " : "  ";
        Console.WriteLine($"{mark}{i + 1}. {algorithms[i].AlgorithmName}");
        Console.WriteLine($"     Complexity : {algorithms[i].TheoreticalComplexity}");
        Console.WriteLine($"     Best for   : {algorithms[i].BestUseCase}");
        Console.WriteLine();
    }

    Console.Write("Select (1-4): ");
    if (int.TryParse(Console.ReadLine(), out int c) && c >= 1 && c <= algorithms.Count)
    {
        algoIdx = c - 1;
        context.SetStrategy(algorithms[algoIdx]);
        Console.WriteLine($"\nSelected: {algorithms[algoIdx].AlgorithmName}");
    }
    else
    {
        Console.WriteLine("Invalid selection.");
    }

    Pause();
}

void RunNormal()
{
    if (!ValidateInputs()) return;

    Console.WriteLine($"=== {algorithms[algoIdx].AlgorithmName} ===");
    Console.WriteLine($"Pattern   : \"{pattern}\"");
    Console.WriteLine($"Text size : {text.Length} chars\n");

    var result = context.ExecuteSearch(text, pattern, stepByStep: false);
    PrintResult(result);
    Pause();
}

void RunStepByStep()
{
    if (!ValidateInputs()) return;

    if (text.Length > 600)
    {
        Console.WriteLine($"\nWarning: text is {text.Length} chars, this may generate many steps.");
        Console.Write("Continue? (y/N): ");
        if (!string.Equals(Console.ReadLine()?.Trim(), "y", StringComparison.OrdinalIgnoreCase))
            return;
    }

    Console.WriteLine($"=== Step-by-Step: {algorithms[algoIdx].AlgorithmName} ===");
    Console.WriteLine("Generating steps...");
    var result = context.ExecuteSearch(text, pattern, stepByStep: true);

    Console.Clear();
    Console.WriteLine($"=== Step-by-Step: {algorithms[algoIdx].AlgorithmName} ===");
    Console.WriteLine($"Pattern : \"{pattern}\"");
    Console.WriteLine($"{result.Steps.Count} steps generated.");
    Console.WriteLine("[Enter] next  |  [A] show all  |  [Q] quit to results\n");
    Console.Write("Press any key to start...");
    Console.ReadKey(intercept: true);
    Console.WriteLine("\n");

    bool showAll = false;
    int stepNum = 0;

    foreach (string step in result.Steps)
    {
        stepNum++;
        Console.WriteLine(step);

        if (!showAll)
        {
            Console.Write($"[{stepNum}/{result.Steps.Count}] Enter=next  A=all  Q=quit > ");
            var key = Console.ReadKey(intercept: true);
            Console.WriteLine();
            if (key.Key == ConsoleKey.Q) break;
            if (key.Key == ConsoleKey.A)
            {
                showAll = true;
                Console.WriteLine();
            }
        }
    }

    Console.WriteLine("\n--- Final Result ---");
    PrintResult(result);
    Pause();
}

void CompareAll()
{
    if (!ValidateInputs()) return;

    Console.WriteLine("=== Comparing All Algorithms ===");
    Console.WriteLine($"Pattern: \"{pattern}\"  |  Text: {text.Length} chars\n");

    var results = new List<SearchResult>();
    foreach (var algo in algorithms)
    {
        var r = algo.Search(text, pattern, stepByStep: false);
        results.Add(r);
        Console.WriteLine(
            $"{algo.AlgorithmName,-28}  {r.Comparisons,8} comparisons   {r.ElapsedNanoseconds / 1000.0,10:F1} us");
    }

    Console.WriteLine();
    PrintComparisonTable(results);

    int baseCount = results[0].Positions.Count;
    bool allAgree = results.All(r => r.Positions.Count == baseCount);

    Console.WriteLine();
    if (allAgree)
        Console.WriteLine(
            $"All algorithms agree: {baseCount} occurrence(s) at [{string.Join(", ", results[0].Positions)}]");
    else
        Console.WriteLine("Warning: algorithms disagree on occurrence count! Check implementation.");

    Console.WriteLine();
    PrintTheoreticalAnalysis(results, text.Length, pattern.Length);
    Pause();
}

void ClearText()
{
    text = string.Empty;
    loadedFiles.Clear();
    Console.WriteLine("\nText cleared.");
    Pause();
}

void PrintResult(SearchResult r)
{
    Console.WriteLine($"Algorithm   : {r.AlgorithmName}");
    Console.WriteLine($"Complexity  : {r.TheoreticalComplexity}");
    Console.WriteLine($"Text length : {r.TextLength} chars");
    Console.WriteLine($"Pattern len : {r.PatternLength} chars");
    Console.WriteLine();

    if (r.Positions.Count > 0)
        Console.WriteLine($"{r.Positions.Count} occurrence(s) at index(es): [{string.Join(", ", r.Positions)}]");
    else
        Console.WriteLine("Pattern not found.");

    Console.WriteLine();
    Console.WriteLine($"Comparisons : {r.Comparisons}");
    if (r.SpuriousHits > 0)
        Console.WriteLine($"Spurious hits (hash collisions): {r.SpuriousHits}");
    Console.WriteLine($"Time (ms)   : {r.ElapsedMilliseconds}");
    Console.WriteLine($"Time (ns)   : {r.ElapsedNanoseconds}");

    if (r.AuxiliaryData.Count > 0)
    {
        Console.WriteLine("\nAuxiliary structures:");
        foreach (var (k, v) in r.AuxiliaryData)
            Console.WriteLine($"  {k}: {v}");
    }
}

void PrintComparisonTable(List<SearchResult> res)
{
    int[] w = { 28, 12, 14, 12, 14 };
    string[] h = { "Algorithm", "Time (us)", "Comparisons", "Occurrences", "Complexity" };

    string Sep() => "+" + string.Join("+", w.Select(x => new string('-', x + 2))) + "+";

    string Row(string[] cells)
    {
        var sb = new StringBuilder("|");
        for (int i = 0; i < cells.Length; i++)
        {
            string cell = cells[i].Length > w[i] ? cells[i][..(w[i] - 1)] + "." : cells[i];
            sb.Append($" {cell.PadRight(w[i])} |");
        }

        return sb.ToString();
    }

    long minNs = res.Min(r => r.ElapsedNanoseconds);
    int minComp = res.Min(r => r.Comparisons);

    Console.WriteLine(Sep());
    Console.WriteLine(Row(h));
    Console.WriteLine(Sep());

    foreach (var r in res)
    {
        bool leanest = r.Comparisons == minComp;
        string[] cells =
        {
            r.AlgorithmName,
            $"{r.ElapsedNanoseconds / 1000.0:F1}",
            r.Comparisons + (leanest ? " *" : ""),
            r.Positions.Count.ToString(),
            r.ShortComplexity
        };
        Console.WriteLine(Row(cells));
    }

    Console.WriteLine(Sep());
    Console.WriteLine("(* = fewest comparisons)");
}

void PrintTheoreticalAnalysis(List<SearchResult> res, int n, int m)
{
    Console.WriteLine("\n--- Theoretical vs Actual ---");
    Console.WriteLine($"n = {n}  |  m = {m}\n");
    Console.WriteLine($"{"Algorithm",-28}  {"Complexity",-14}  {"Actual Comp.",-14}  Comp./n");
    Console.WriteLine(new string('-', 72));

    foreach (var r in res)
    {
        double ratio = n > 0 ? (double)r.Comparisons / n : 0;
        Console.WriteLine($"{r.AlgorithmName,-28}  {r.ShortComplexity,-14}  {r.Comparisons,-14}  {ratio:F3}");
    }

    Console.WriteLine();
    Console.WriteLine("Notes:");
    Console.WriteLine("  Naive       - Comp./n approaches m in worst case");
    Console.WriteLine("  Rabin-Karp  - Comp./n ~1, rises with hash collisions");
    Console.WriteLine("  KMP         - Comp./n <= 2, good for repetitive patterns");
    Console.WriteLine("  Boyer-Moore - Comp./n can go below 1 by skipping characters");
}

bool ValidateInputs()
{
    if (text.Length == 0)
    {
        Console.WriteLine("No text loaded. Use option 1 or 2 first.");
        Pause();
        return false;
    }

    if (pattern.Length == 0)
    {
        Console.WriteLine("No pattern set. Use option 3 first.");
        Pause();
        return false;
    }

    return true;
}