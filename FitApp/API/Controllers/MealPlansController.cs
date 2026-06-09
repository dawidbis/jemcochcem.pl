using FitApp.Application.Features.GenerateMealPlan;
using FitApp.Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitApp.API.Controllers;

public class MealPlansController : ApiControllerBase
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
        var command = new GenerateMealPlanCommand(CurrentUserId, request.Prompt);
        var mealPlanId = await _mediator.Send(command);
        return Ok(new { MealPlanId = mealPlanId });
    }

    [HttpGet]
    public async Task<IActionResult> GetUserMealPlans()
    {
        var plans = await _mealPlanRepo.GetByUserIdAsync(CurrentUserId);
        return Ok(plans);
    }
}

public class GenerateMealPlanRequest
{
    public string Prompt { get; set; } = string.Empty;
}