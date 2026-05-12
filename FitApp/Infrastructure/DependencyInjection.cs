namespace FitApp.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;       
using FitApp.Infrastructure.Interfaces;
using FitApp.Infrastructure.Data;
using FitApp.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
// Dodajemy using do naszych interfejsów i serwisów
using FitApp.Domain.Interfaces;
using FitApp.Domain.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMealLogRepository, MealLogRepository>();
        services.AddScoped<IFoodRepository, FoodRepository>();
        services.AddScoped<IBodyMeasurementRepository, BodyMeasurementRepository>(); 

        // --- REJESTRACJA WARSTWY BLL (PUNKT 2) ---
        services.AddScoped<INutritionCalculationService, NutritionCalculationService>();
        services.AddScoped<IMealLogDomainService, MealLogDomainService>();

        return services;
    }
}