using string_search_comparison.Models;

namespace string_search_comparison.Interfaces;

public interface IStringSearchStrategy
{
    string AlgorithmName { get; }
    string TheoreticalComplexity { get; }
    string ShortComplexity { get; }
    string BestUseCase { get; }
    SearchResult Search(string text, string pattern, bool stepByStep = false);
}