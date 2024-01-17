namespace ObservabilityExample.HttpService.ExternalServices.Greeter;

public interface IGreeterExternalService
{
    Task<string> GetGreetingAsync(string name, CancellationToken cancelToken);
}
