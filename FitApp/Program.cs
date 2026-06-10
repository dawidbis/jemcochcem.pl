
using FitApp.Application;
using FitApp.Infrastructure;
using FitApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FitApp.Domain.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
DotNetEnv.Env.Load();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Dodaj to tam, gdzie rejestrujesz serwisy
builder.Services.AddHttpClient<IGeminiService, GeminiService>();
builder.Services.AddScoped<FitApp.Domain.Services.IAiMealService, FitApp.Domain.Services.AiMealService>();
builder.Services.AddCors(options => {
    options.AddPolicy("VitePolicy", policy => {
        if (builder.Environment.IsDevelopment())
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            var corsOrigins = builder.Configuration["CORS_ORIGINS"];
            if (!string.IsNullOrEmpty(corsOrigins))
                policy.WithOrigins(corsOrigins.Split(','));
            else
                policy.AllowAnyOrigin();

            policy.AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization();
var app = builder.Build();

app.UseRouting();
app.UseCors("VitePolicy");
app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.MapFallbackToFile("index.html");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    await DataSeeder.SeedAsync(db);
}
app.Run();