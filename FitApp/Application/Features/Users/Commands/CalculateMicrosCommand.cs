namespace FitApp.Application.Features.Users;

using MediatR;
using FitApp.Domain.Services;
using FitApp.Infrastructure.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

// Komenda przyjmuje ID usera – cele mikroskładników zależą m.in. od płci (żelazo)
public record CalculateMicrosCommand(Guid UserId) : IRequest<MicroResultDto>;

// DTO z dziennymi celami mikroskładników (g dla błonnika/cukrów/tł. nasyconych, mg dla sodu/wapnia/żelaza)
public record MicroResultDto(
    decimal Fiber,
    decimal Sugars,
    decimal SaturatedFat,
    decimal Sodium,
    decimal Calcium,
    decimal Iron);

public class CalculateMicrosHandler : IRequestHandler<CalculateMicrosCommand, MicroResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly NutritionCalculationService _nutritionService;

    public CalculateMicrosHandler(
        IUserRepository userRepository,
        NutritionCalculationService nutritionService)
    {
        _userRepository = userRepository;
        _nutritionService = nutritionService;
    }

    public async Task<MicroResultDto> Handle(CalculateMicrosCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null) throw new ArgumentException("Nie znaleziono użytkownika.");

        var goals = _nutritionService.CalculateDailyMicroGoals(user.Gender);

        return new MicroResultDto(
            goals.Fiber,
            goals.Sugars,
            goals.SaturatedFat,
            goals.Sodium,
            goals.Calcium,
            goals.Iron);
    }
}
