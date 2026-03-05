namespace NodeNetwork.Blazor.Services;

using Microsoft.Extensions.DependencyInjection;
using NodeNetwork.Blazor.Compatibility;
using NodeNetwork.Blazor.Validation;

public static class ServiceCollectionExtensions {
  public static IServiceCollection AddNodeNetworkBlazor(this IServiceCollection services) {
    services.AddScoped<INetworkConnectionValidator, DefaultNetworkConnectionValidator>();
    services.AddScoped<INodeNetworkCompatibilityAdapter, DefaultNodeNetworkCompatibilityAdapter>();
    return services;
  }
}
