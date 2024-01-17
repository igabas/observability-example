using ObservabilityExample.HttpService.Extensions;
using ObservabilityExample.HttpService.Services;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var tracingBackendHost = Environment.GetEnvironmentVariable("OTEL_COLLECTOR_ENDPOINT") ?? "http://jaeger:4317";
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
        .AddAspNetCoreInstrumentation(opt =>
        {
            // skip tracing metrics path
            opt.Filter = context => !(context.Request.Method.Equals("GET") && context.Request.Path == "/metrics");
        })
        //.AddGrpcClientInstrumentation()
        .AddOtlpExporter(o => { o.Endpoint = new Uri(tracingBackendHost); })
    )
    // add metrics
    .WithMetrics(b => b
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddProcessInstrumentation()
        .AddRuntimeInstrumentation()
        .AddPrometheusExporter()
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

        options.IncludeFormattedMessage = true;
        options.IncludeScopes = true;
        options.ParseStateValues = true;

        options.SetResourceBuilder(rb);
        options.AddOtlpExporter(o => o.Endpoint = new Uri(tracingBackendHost));
        options.AttachLogsToActivityEvent();
    })
    .AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.Run();
