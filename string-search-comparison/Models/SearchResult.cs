namespace string_search_comparison.Models;

public class SearchResult
{
    public string AlgorithmName { get; set; } = string.Empty;
    public string TheoreticalComplexity { get; set; } = string.Empty;
    public string ShortComplexity { get; set; } = string.Empty;
    public List<int> Positions { get; set; } = new();
    public int Comparisons { get; set; }
    public int SpuriousHits { get; set; }   // Rabin-Karp hash collisions
    public long ElapsedMilliseconds { get; set; }
    public long ElapsedNanoseconds { get; set; }
    public List<string> Steps { get; set; } = new();
    public int TextLength { get; set; }
    public int PatternLength { get; set; }
    public Dictionary<string, string> AuxiliaryData { get; set; } = new();
}
