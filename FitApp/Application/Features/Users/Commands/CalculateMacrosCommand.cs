namespace FitApp.Application.Features.Users;

using MediatR;
using FitApp.Domain.Services;
using FitApp.Infrastructure.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

// Komenda przyjmuje ID usera i mnożnik aktywności
public record CalculateMacrosCommand(Guid UserId, decimal ActivityMultiplier) : IRequest<MacroResultDto>;

// DTO żeby nie zwracać modelu domenowego bezpośrednio przez API
public record MacroResultDto(int Tdee, int Protein, int Fat, int Carbs);

public class CalculateMacrosHandler : IRequestHandler<CalculateMacrosCommand, MacroResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly TdeeCalculationService _tdeeService;
    private readonly NutritionCalculationService _nutritionService;

    public CalculateMacrosHandler(
        IUserRepository userRepository, 
        TdeeCalculationService tdeeService, 
        NutritionCalculationService nutritionService)
    {
        _userRepository = userRepository;
        _tdeeService = tdeeService;
        _nutritionService = nutritionService;
    }

    public async Task<MacroResultDto> Handle(CalculateMacrosCommand request, CancellationToken ct)
{
    var user = await _userRepository.GetByIdAsync(request.UserId);
    if (user == null) throw new ArgumentException("Nie znaleziono użytkownika.");

    // 1. Najpierw liczymy zapotrzebowanie na kalorie (TDEE)
    var tdee = _tdeeService.CalculateTDEE(user.Weight, user.Height, user.Age, user.Gender, request.ActivityMultiplier);

    // 2. Używamy NOWEJ metody do policzenia podziału na makroskładniki
    var dailyMacros = _nutritionService.CalculateDailyMacroGoals(tdee, user.Weight);

    // 3. Zwracamy zmapowane na DTO (rzutując na int, żeby API zwracało ładne pełne liczby)
    // UWAGA: Upewnij się, jak nazywają się właściwości w Twojej klasie MacroNutrients! 
    // Zapewne są to .Protein, .Fats, .Carbs (lub podobnie).
    return new MacroResultDto(
        tdee, 
        (int)dailyMacros.Protein, 
        (int)dailyMacros.Fats, 
        (int)dailyMacros.Carbs
    );
}
}