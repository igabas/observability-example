using Grpc.Core;
using ObservabilityExample.GrpcService.Services;

namespace ObservabilityExample.GrpcService.GrpcControllers;

public class GreeterGrpcController(
    ObservationService observationService,
    ILogger<GreeterGrpcController> logger)
    : GreeterService.GreeterServiceBase
{
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        using var activity =
            observationService.ActivitySource.StartActivity($"{nameof(GreeterGrpcController)}.{nameof(SayHello)}");
        activity?.SetTag(nameof(request.Name), request.Name);

        logger.LogInformation(
            "Starting execute method '{Method}' of '{Service}' service ...",
            nameof(SayHello),
            nameof(GreeterGrpcController));

        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }
}
