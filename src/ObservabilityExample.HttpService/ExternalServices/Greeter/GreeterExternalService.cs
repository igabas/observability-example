using Grpc.Net.ClientFactory;
using ObservabilityExample.GrpcService;
using GrpcGreeterServiceClient = ObservabilityExample.GrpcService.GreeterService.GreeterServiceClient;

namespace ObservabilityExample.HttpService.ExternalServices.Greeter;

public class GreeterExternalService : IGreeterExternalService
{
    private readonly GrpcGreeterServiceClient _client;

    public GreeterExternalService(GrpcClientFactory grpcClientFactory)
    {
        _client = grpcClientFactory.CreateClient<GrpcGreeterServiceClient>(ExternalServiceNames.Greeter);
    }

    public async Task<string> GetGreetingAsync(string name, CancellationToken cancelToken)
    {
        var request = new HelloRequest
        {
            Name = name
        };

        var response = await _client.SayHelloAsync(request, cancellationToken :cancelToken);

        return response.Message;
    }
}
