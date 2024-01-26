using ObservabilityExample.HttpService.ExternalServices;
using ObservabilityExample.HttpService.ExternalServices.Greeter;

namespace ObservabilityExample.HttpService.Extensions;

public static class ExternalServiceCollectionExtensions
{
    public static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        var grpcServiceAddress = configuration.GetSection("GrpcServiceOptions:Address").Get<string>();

        return services
                // Greeter
                .AddGrpcExternalService(ExternalServiceNames.Greeter, new Uri(grpcServiceAddress!))
                .AddTransient<IGreeterExternalService, GreeterExternalService>()
            ;
    }

    private static IServiceCollection AddGrpcExternalService(
        this IServiceCollection services,
        string name,
        Uri? address)
    {
        services
            .AddGrpcClient<ObservabilityExample.GrpcService.GreeterService.GreeterServiceClient>(name,
                opt => { opt.Address = address; })
            .EnableCallContextPropagation(o => o.SuppressContextNotFoundErrors = true);

        return services;
    }
}
