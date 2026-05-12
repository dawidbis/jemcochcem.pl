using FitApp.Infrastructure.Interfaces;
using MediatR;
using System.Text.Json.Serialization;
public record UpdateFoodCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string Name { get; init; } = string.Empty;
    public int CaloriesPer100g { get; init; }
    public decimal ProteinPer100g { get; init; }
    public decimal CarbsPer100g { get; init; }
    public decimal FatsPer100g { get; init; }
}

public class UpdateFoodHandler : IRequestHandler<UpdateFoodCommand, bool>
{
    private readonly IFoodRepository _repository;
    public UpdateFoodHandler(IFoodRepository repository) => _repository = repository;

    public async Task<bool> Handle(UpdateFoodCommand request, CancellationToken ct)
    {
        var food = await _repository.GetByIdAsync(request.Id);
        if (food == null) return false;

        // Aktualizacja pól
        food.Name = request.Name;
        food.CaloriesPer100g = request.CaloriesPer100g;
        food.ProteinPer100g = request.ProteinPer100g;
        food.CarbsPer100g = request.CarbsPer100g;
        food.FatsPer100g = request.FatsPer100g;

        await _repository.UpdateAsync(food);
        return true;
    }
}