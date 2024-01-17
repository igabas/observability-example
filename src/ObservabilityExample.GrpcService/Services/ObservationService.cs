using System.Diagnostics;

namespace ObservabilityExample.GrpcService.Services;

public class ObservationService : IDisposable
{
    public string ApplicationName { get; init; }

    public const string ApplicationVersion = "0.1.1-alpha";

    public ActivitySource ActivitySource { get; init; }

    public ObservationService(string applicationName)
    {
        if (string.IsNullOrEmpty(applicationName))
            throw new ArgumentNullException(nameof(applicationName));

        ApplicationName = applicationName;
        ActivitySource = new ActivitySource(applicationName, ApplicationVersion);
    }

    public void Dispose() { ActivitySource?.Dispose(); }
}
