using string_search_comparison.Interfaces;
using string_search_comparison.Models;

namespace string_search_comparison.Context;

public class SearchContext
{
    private IStringSearchStrategy _strategy;

    public SearchContext(IStringSearchStrategy strategy)
    {
        _strategy = strategy;
    }

    public void SetStrategy(IStringSearchStrategy strategy) => _strategy = strategy;

    public IStringSearchStrategy CurrentStrategy => _strategy;

    public SearchResult ExecuteSearch(string text, string pattern, bool stepByStep = false)
        => _strategy.Search(text, pattern, stepByStep);
}

