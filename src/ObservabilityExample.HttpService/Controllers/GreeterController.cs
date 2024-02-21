using Microsoft.AspNetCore.Mvc;
using ObservabilityExample.HttpService.ExternalServices.Greeter;
using ObservabilityExample.HttpService.Services;

namespace ObservabilityExample.HttpService.Controllers;

[ApiController]
[Route("[controller]")]
public class GreeterController(
    ObservationService observationService,
    ILogger<GreeterController> logger,
    IGreeterExternalService greeterExternalService) : ControllerBase
{
    [HttpGet("me")]
    public async Task<string> GreetMeAsync(
        [FromQuery] string name,
        CancellationToken cancelToken)
    {
        using var activity =
            observationService.ActivitySource.StartActivity($"{nameof(GreeterController)}.{nameof(GreetMeAsync)}");

        activity?.SetTag("name", name);

        logger.LogInformation(
            "Starting execute method '{Method}' of '{Service}' controller ...",
            nameof(GreetMeAsync),
            nameof(GreeterController));

        var greetMessage = await greeterExternalService.GetGreetingAsync(name, cancelToken);

        return greetMessage;
    }
}
