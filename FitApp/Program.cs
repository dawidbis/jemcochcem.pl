AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
using FitApp.Application;
using FitApp.Infrastructure;
using FitApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

var app = builder.Build();
app.UseRouting();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("VitePolicy");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();