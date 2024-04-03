using System.Diagnostics;
using EvolveDb;
using EvolveDb.Configuration;
using ObservabilityExample.GrpcService.DataAccess;
using ObservabilityExample.GrpcService.Services;

namespace ObservabilityExample.GrpcService.HostedServices;

public class MigrationHostedServices(
    ILogger<MigrationHostedServices> logger,
    IServiceProvider serviceProvider)
    : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        return Migrate(cancellationToken);
    }

    // todo move to libs
    private async Task Migrate(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var tracer = scope.ServiceProvider.GetRequiredService<ObservationService>();
        using var activity = tracer.ActivitySource.StartActivity("Database migration");

        try
        {
            logger.LogInformation("Starting migrating database");

            var connectionFactory = scope.ServiceProvider.GetRequiredService<IConnectionFactory>();
            var connection = await connectionFactory.CreateConnection(cancellationToken);

            var migrator = new Evolve(connection, msg => logger.LogInformation(msg))
            {
                Command = CommandOptions.DoNothing,
                IsEraseDisabled = true,
                Locations = new[] { "migrations" },
                MetadataTableName = "migrations",
                CommandTimeout = (int)TimeSpan.FromMinutes(10).TotalSeconds,
            };

            migrator.Migrate();
            logger.LogInformation("Database successfully migrated");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Error occur while migrating database");
            if (activity != null)
            {
                activity.AddTag("exception.message", ex.Message);
                activity.AddTag("exception.stacktrace", ex.ToString());
                activity.AddTag("exception.type", ex.GetType().FullName);
                activity.SetStatus(ActivityStatusCode.Error);
            }

            throw;
        }
    }
}
