namespace FitApp.Application;

using Microsoft.Extensions.DependencyInjection;
using FitApp.Domain.Services;
using FitApp.Application.Interfaces;
using FitApp.Infrastructure.ExternalServices;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddScoped<NutritionCalculationService>();
        services.AddScoped<MealLogDomainService>();
        services.AddScoped<TdeeCalculationService>();
        services.AddHttpClient<IOffApiClient, OpenFoodFactsClient>();
        return services;
    }
}