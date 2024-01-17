using Microsoft.AspNetCore.Server.Kestrel.Core;
using ObservabilityExample.GrpcService.GrpcControllers;
using ObservabilityExample.GrpcService.Services;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

var tracingBackendHost = Environment.GetEnvironmentVariable("OTEL_COLLECTOR_ENDPOINT") ?? "http://jaeger:4317";
var applicationName = Environment.GetEnvironmentVariable("APPLICATION_NAME") ?? "grpc-service";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.WebHost.ConfigureKestrel((_, options) =>
{
    if (!int.TryParse(Environment.GetEnvironmentVariable("ASPNETCORE_GRPC_PORT"), out var grpcPort))
    {
        grpcPort = 8084;
    }

    if (!int.TryParse(Environment.GetEnvironmentVariable("ASPNETCORE_HTTP_PORT"), out var httpPort))
    {
        grpcPort = 8080;
    }

    //grpc port for http2 connections
    options.ListenAnyIP(grpcPort, listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
    //add health check on a separate port instead of Http1AndHttp2 on same port
    options.ListenAnyIP(httpPort, listenOptions => { listenOptions.Protocols = HttpProtocols.Http1; });
});
// add grpc
builder.Services.AddGrpc(opt => { opt.EnableDetailedErrors = true; });
builder.Services.AddGrpcReflection();

builder.Services.AddSingleton(new ObservationService(applicationName));

// add tracing
builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(o =>
        o.AddService(applicationName, serviceVersion :ObservationService.ApplicationVersion)
    )
    .WithTracing(b => b
        .AddSource(applicationName)
        .SetSampler(new AlwaysOnSampler())
        .AddAspNetCoreInstrumentation(opt =>
        {
            // skip tracing metrics path
            opt.Filter = context => !(context.Request.Path == "/metrics" && context.Request.Method == "GET");
        })
        //.AddGrpcCoreInstrumentation()
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

        options.SetResourceBuilder(rb);

        options.IncludeFormattedMessage = true;
        options.IncludeScopes = true;
        options.ParseStateValues = true;

        options.AddOtlpExporter(o => o.Endpoint = new Uri(tracingBackendHost));
        options.AttachLogsToActivityEvent();
    })
    .AddConsole();

var app = builder.Build();

app.MapGrpcService<GreeterGrpcController>();
// add grpc reflection
app.MapGrpcReflectionService();
app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.Run();
