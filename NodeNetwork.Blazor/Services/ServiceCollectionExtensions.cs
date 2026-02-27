using Microsoft.Extensions.DependencyInjection;
using NodeNetwork.Blazor.Compatibility;
using NodeNetwork.Blazor.Validation;

namespace NodeNetwork.Blazor.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNodeNetworkBlazor(this IServiceCollection services)
    {
        services.AddScoped<INetworkConnectionValidator, DefaultNetworkConnectionValidator>();
        services.AddScoped<INodeNetworkCompatibilityAdapter, DefaultNodeNetworkCompatibilityAdapter>();
        return services;
    }
}
