using string_search_comparison.Algorithms;
using string_search_comparison.Context;
using string_search_comparison.Interfaces;
using string_search_comparison.Models;
using string_search_comparison.Observability;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

Instrumentation.Init();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<IEnumerable<IStringSearchStrategy>>(new List<IStringSearchStrategy>
{
    new NaiveSearch(),
    new RabinKarpSearch(),
    new KmpSearch(),
    new BoyerMooreSearch()
});

builder.Services.AddScoped<SearchContext>(sp => 
{
    var strategies = sp.GetRequiredService<IEnumerable<IStringSearchStrategy>>();
    return new SearchContext(strategies.First());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.MapPost("/api/search", ([FromBody] SearchRequest request, [FromServices] SearchContext context, [FromServices] IEnumerable<IStringSearchStrategy> strategies) =>
{
    var strategy = strategies.FirstOrDefault(s => s.AlgorithmName.Contains(request.Algorithm, StringComparison.OrdinalIgnoreCase)) 
                   ?? strategies.First();
    
    context.SetStrategy(strategy);
    var result = context.ExecuteSearch(request.Text, request.Pattern, request.StepByStep);
    
    return Results.Ok(result);
})
.WithName("ExecuteSearch");

app.MapPost("/api/compare", ([FromBody] CompareRequest request, [FromServices] SearchContext context, [FromServices] IEnumerable<IStringSearchStrategy> strategies) =>
{
    var results = new List<SearchResult>();
    
    foreach (var strategy in strategies)
    {
        context.SetStrategy(strategy);
        var r = context.ExecuteSearch(request.Text, request.Pattern, stepByStep: false);
        results.Add(r);
    }
    
    return Results.Ok(results);
})
.WithName("CompareAlgorithms");

app.MapGet("/api/algorithms", ([FromServices] IEnumerable<IStringSearchStrategy> strategies) =>
{
    var info = strategies.Select(s => new {
        s.AlgorithmName,
        s.TheoreticalComplexity,
        s.ShortComplexity,
        s.BestUseCase
    });
    return Results.Ok(info);
})
.WithName("GetAlgorithms");

app.Run();

Instrumentation.Shutdown();

public record SearchRequest(string Text, string Pattern, string Algorithm, bool StepByStep = false);
public record CompareRequest(string Text, string Pattern);
