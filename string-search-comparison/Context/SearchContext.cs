using Microsoft.Extensions.Logging;
using string_search_comparison.Interfaces;
using string_search_comparison.Models;
using string_search_comparison.Observability;
using System.Diagnostics;

namespace string_search_comparison.Context;

public class SearchContext
{
    private IStringSearchStrategy _strategy;
    private readonly ILogger<SearchContext> _logger;

    public SearchContext(IStringSearchStrategy strategy)
    {
        _strategy = strategy;
        _logger = Instrumentation.CreateLogger<SearchContext>();
    }

    public void SetStrategy(IStringSearchStrategy strategy)
    {
        _strategy = strategy;
        _logger.LogInformation("Algorithm changed to: {AlgorithmName}", strategy.AlgorithmName);
    }

    public IStringSearchStrategy CurrentStrategy => _strategy;

    public SearchResult ExecuteSearch(string text, string pattern, bool stepByStep = false)
    {
        using var activity = Instrumentation.ActivitySource.StartActivity("SearchOperation");
        activity?.SetTag("algorithm", _strategy.AlgorithmName);
        activity?.SetTag("text_length", text.Length);
        activity?.SetTag("pattern_length", pattern.Length);
        activity?.SetTag("step_by_step", stepByStep);

        _logger.LogInformation("Starting search with {AlgorithmName}. Pattern: '{Pattern}', Text Length: {TextLength}", 
            _strategy.AlgorithmName, pattern, text.Length);

        var stopwatch = Stopwatch.StartNew();
        var result = _strategy.Search(text, pattern, stepByStep);
        stopwatch.Stop();

        double durationSeconds = stopwatch.Elapsed.TotalSeconds;

        var tags = new TagList { { "algorithm", _strategy.AlgorithmName } };
        Instrumentation.SearchExecutionsCounter.Add(1, tags);
        Instrumentation.SearchDurationHistogram.Record(durationSeconds, tags);
        Instrumentation.ComparisonsCounter.Add(result.Comparisons, tags);

        _logger.LogInformation("Search completed. Found {Count} occurrences. Comparisons: {Comparisons}. Time: {Time}ms", 
            result.Positions.Count, result.Comparisons, result.ElapsedMilliseconds);

        activity?.SetTag("occurrences_found", result.Positions.Count);
        activity?.SetTag("comparisons", result.Comparisons);

        return result;
    }
}
