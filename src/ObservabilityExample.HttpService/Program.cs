using ObservabilityExample.HttpService.Extensions;
using ObservabilityExample.HttpService.Services;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var otelExporterEndpoint = new Uri(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT") !);
var applicationName = Environment.GetEnvironmentVariable("APPLICATION_NAME") ?? "http-service";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddExternalServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

builder.Services.AddSingleton(new ObservationService(applicationName));

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(o =>
        o.AddService(applicationName, serviceVersion :ObservationService.ApplicationVersion)
    )
    // add tracing
    .WithTracing(b => b
        .AddSource(applicationName)
        .SetSampler(new AlwaysOnSampler())
        .AddAspNetCoreInstrumentation()
        //.AddConsoleExporter()
        .AddOtlpExporter(o => o.Endpoint = otelExporterEndpoint)
    )
    // add metrics
    .WithMetrics(b => b
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddProcessInstrumentation()
        .AddRuntimeInstrumentation()
        //.AddConsoleExporter()
        .AddOtlpExporter(o => o.Endpoint = otelExporterEndpoint)
    );

// logging
builder.Logging
    .ClearProviders()
    .AddOpenTelemetry(options =>
    {
        var rb = ResourceBuilder.CreateDefault()
            .AddService(
                applicationName,
                serviceVersion :ObservationService.ApplicationVersion);

        options.SetResourceBuilder(rb);

        options.IncludeFormattedMessage = true;
        options.IncludeScopes = true;
        options.ParseStateValues = true;

        options.AttachLogsToActivityEvent();

        options.AddOtlpExporter(o => o.Endpoint = otelExporterEndpoint);
    })
    .AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
