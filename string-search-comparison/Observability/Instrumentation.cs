using System.Diagnostics;
using System.Diagnostics.Metrics;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Logs;
using Microsoft.Extensions.Logging;

namespace string_search_comparison.Observability;

public static class Instrumentation
{
    public const string ServiceName = "StringSearchComparison";
    public const string ServiceVersion = "1.0.0";

    public static readonly ActivitySource ActivitySource = new(ServiceName);
    public static readonly Meter Meter = new(MeterName);
    public const string MeterName = "StringSearch.Metrics";

    public static readonly Counter<long> SearchExecutionsCounter = Meter.CreateCounter<long>("string_search_executions_total", description: "Total number of string search executions");
    public static readonly Histogram<double> SearchDurationHistogram = Meter.CreateHistogram<double>("string_search_duration_seconds", unit: "s", description: "Duration of string search executions");
    public static readonly Counter<long> ComparisonsCounter = Meter.CreateCounter<long>("string_search_comparisons_total", description: "Total number of character comparisons performed");

    private static TracerProvider? _tracerProvider;
    private static MeterProvider? _meterProvider;
    private static ILoggerFactory? _loggerFactory;

    public static void Init()
    {
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(ServiceName, serviceVersion: ServiceVersion);

        _tracerProvider = Sdk.CreateTracerProviderBuilder()
            .SetResourceBuilder(resourceBuilder)
            .AddSource(ServiceName)
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter(opt => {
                opt.Endpoint = new Uri("http://localhost:4317");
            })
            .Build();

        _meterProvider = Sdk.CreateMeterProviderBuilder()
            .SetResourceBuilder(resourceBuilder)
            .AddMeter(MeterName)
            .AddRuntimeInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter(opt => {
                opt.Endpoint = new Uri("http://localhost:4317");
            })
            .Build();

        _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddOpenTelemetry(options =>
            {
                options.SetResourceBuilder(resourceBuilder);
                options.AddConsoleExporter();
                options.AddOtlpExporter(opt => {
                    opt.Endpoint = new Uri("http://localhost:4317");
                });
            });
        });
    }

    public static ILogger<T> CreateLogger<T>() => _loggerFactory!.CreateLogger<T>();

    public static void Shutdown()
    {
        _tracerProvider?.Dispose();
        _meterProvider?.Dispose();
        _loggerFactory?.Dispose();
    }
}
