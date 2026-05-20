using MediatR;

namespace FitApp.Application.Features.Diary.AddAiMealToDiary;

public record AddAiMealToDiaryCommand(
    Guid UserId, 
    Guid MealPlanItemId, 
    DateTime Date
) : IRequest<Guid>;