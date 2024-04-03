using Dapper;
using Grpc.Core;
using ObservabilityExample.GrpcService.DataAccess;
using ObservabilityExample.GrpcService.Extensions;
using ObservabilityExample.GrpcService.Services;

namespace ObservabilityExample.GrpcService.GrpcControllers;

public class GreeterGrpcController(
    ObservationService observationService,
    IConnectionFactory connectionFactory,
    ILogger<GreeterGrpcController> logger)
    : GreeterService.GreeterServiceBase
{
    public override async Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        using var activity = observationService.ActivitySource.StartActivity($"{nameof(GreeterGrpcController)}.{nameof(SayHello)}");
        activity?.SetTag("name", request.Name);

        logger.LogMethodExecution(nameof(SayHello), nameof(GreeterGrpcController));

        await using var connection = await connectionFactory.CreateConnection(context.CancellationToken);

        await connection.ExecuteScalarAsync<int>("SELECT 1");

        return new HelloReply
        {
            Message = "Hello " + request.Name
        };
    }
}
