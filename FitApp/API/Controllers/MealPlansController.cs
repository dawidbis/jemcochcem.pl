using FitApp.Application.Features.GenerateMealPlan;
using FitApp.Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FitApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MealPlansController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMealPlanRepository _mealPlanRepo;

    public MealPlansController(IMediator mediator, IMealPlanRepository mealPlanRepo)
    {
        _mediator = mediator;
        _mealPlanRepo = mealPlanRepo;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateMealPlan([FromBody] GenerateMealPlanRequest request)
    {
        var command = new GenerateMealPlanCommand(request.UserId, request.Prompt);
        var mealPlanId = await _mediator.Send(command);
        return Ok(new { MealPlanId = mealPlanId });
    }

    [HttpGet]
    public async Task<IActionResult> GetUserMealPlans([FromQuery] Guid userId)
    {
        try 
        {
            // Sprawdź logi dockera – jeśli tu jest błąd, to wina repozytorium
            var plans = await _mealPlanRepo.GetByUserIdAsync(userId);
            return Ok(plans);
        }
        catch (Exception ex)
        {
            // Zaloguj błąd do konsoli, żebyś wiedział co się dzieje
            Console.WriteLine($"Błąd pobierania planów: {ex.Message}");
            return StatusCode(500, "Błąd serwera przy pobieraniu planów.");
        }
    }
}

public class GenerateMealPlanRequest
{
    public Guid UserId { get; set; }
    public string Prompt { get; set; } = string.Empty;
}