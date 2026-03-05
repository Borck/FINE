namespace FINE.Blazor.Services;

using Microsoft.Extensions.DependencyInjection;
using FINE.Blazor.Compatibility;
using FINE.Blazor.Validation;

public static class ServiceCollectionExtensions {
  public static IServiceCollection AddFINEBlazor(this IServiceCollection services) {
    services.AddScoped<INetworkConnectionValidator, DefaultNetworkConnectionValidator>();
    services.AddScoped<IFINECompatibilityAdapter, DefaultFINECompatibilityAdapter>();
    return services;
  }
}
