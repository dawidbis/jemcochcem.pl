namespace FitApp.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration; 
using Microsoft.EntityFrameworkCore;      
using FitApp.Infrastructure.Interfaces;
using FitApp.Infrastructure.Data;
using FitApp.Infrastructure.Data.Repositories;

public static class DependencyInjection
{
    // Dodaj parametr IConfiguration, aby odczytać appsettings.json
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Rejestracja bazy przeniesiona z Program.cs
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMealLogRepository, MealLogRepository>();
        services.AddScoped<IFoodRepository, FoodRepository>();
        return services;
    }
}