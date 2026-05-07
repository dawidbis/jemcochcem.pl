namespace FitApp.Application;

using Microsoft.Extensions.DependencyInjection;
using FitApp.Domain.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddScoped<NutritionCalculationService>();
        services.AddScoped<MealLogDomainService>();
        services.AddScoped<TdeeCalculationService>();
        return services;
    }
}