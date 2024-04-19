using Npgsql;
using ObservabilityExample.GrpcService.DataAccess;
using ObservabilityExample.GrpcService.GrpcControllers;
using ObservabilityExample.GrpcService.HostedServices;
using ObservabilityExample.GrpcService.Services;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

var applicationName = Environment.GetEnvironmentVariable("APPLICATION_NAME") ?? "grpc-service";

var builder = WebApplication.CreateBuilder(args);

// add grpc
builder.Services.AddGrpc(opt => { opt.EnableDetailedErrors = true; });
builder.Services.AddGrpcReflection();

builder.Services.AddNpgsqlDataSource(builder.Configuration.GetConnectionString("Example")!, op =>
{
    op.Name = "ExampleDb";
    // todo register mappings types/enum
});
builder.Services.AddHostedService<MigrationHostedServices>();
builder.Services.AddTransient<IConnectionFactory, PostgresConnectionFactory>();

builder.Services.AddSingleton(new ObservationService(applicationName));

// add tracing
builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(o =>
        o.AddService(applicationName, serviceVersion: ObservationService.ApplicationVersion)
    )
    .WithTracing(b => b
        .AddSource(applicationName)
        .SetSampler(new AlwaysOnSampler())
        .AddNpgsql(o => { })
        .AddAspNetCoreInstrumentation()
        //.AddConsoleExporter()
        .AddOtlpExporter()
    )
    // add metrics
    .WithMetrics(b => b
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddProcessInstrumentation()
        .AddRuntimeInstrumentation()
        .AddMeter("Npgsql")
        //.AddConsoleExporter()
        .AddOtlpExporter()
    );

// logging
builder.Logging
    .ClearProviders()
    .AddOpenTelemetry(options =>
    {
        var rb = ResourceBuilder.CreateDefault()
            .AddService(
                applicationName,
                serviceVersion: ObservationService.ApplicationVersion);

        options.SetResourceBuilder(rb);

        options.IncludeFormattedMessage = true;
        options.IncludeScopes = true;
        options.ParseStateValues = true;

        options.AttachLogsToActivityEvent();

        options.AddOtlpExporter();
    })
    .AddConsole();


var app = builder.Build();


app.MapGrpcService<GreeterGrpcController>();
// add grpc reflection
app.MapGrpcReflectionService();


app.Run();
