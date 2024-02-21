using Microsoft.AspNetCore.Server.Kestrel.Core;
using Npgsql;
using ObservabilityExample.GrpcService.DataAccess;
using ObservabilityExample.GrpcService.GrpcControllers;
using ObservabilityExample.GrpcService.Services;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

var otelExporterEndpoint = new Uri(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT") !);
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

builder.Services.AddTransient<IConnectionFactory, PostgresConnectionFactory>(sp =>
    new PostgresConnectionFactory(builder.Configuration.GetConnectionString("PgDb")!)
);

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
        .AddNpgsql()
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

app.MapGrpcService<GreeterGrpcController>();
// add grpc reflection
app.MapGrpcReflectionService();

app.Run();
