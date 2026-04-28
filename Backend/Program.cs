using Backend.dtos;
using Backend.services;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

// Endpoint zwracający FoodProductDTO
app.MapGet("/food", () =>
{
    var product = new FoodProductDTO 
    { 
        Name = "Twaróg chudy", 
        Kcal = 90 
    };
    return Results.Ok(product);
})
.WithName("GetFood")
.WithOpenApi();

app.Run();


public partial class Program { }
