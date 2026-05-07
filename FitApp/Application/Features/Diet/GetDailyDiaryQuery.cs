namespace FitApp.Application.Features.Diet;

using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FitApp.Application.DTOs;
using FitApp.Infrastructure.Interfaces;

public class GetDailyDiaryQuery : IRequest<DiaryDto>
{
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
}

public class GetDailyDiaryHandler : IRequestHandler<GetDailyDiaryQuery, DiaryDto>
{
    private readonly IMealLogRepository _mealLogRepository;

    public GetDailyDiaryHandler(IMealLogRepository mealLogRepository)
    {
        _mealLogRepository = mealLogRepository;
    }

    public async Task<DiaryDto> Handle(GetDailyDiaryQuery request, CancellationToken ct)
    {
        var log = await _mealLogRepository.GetByDateAsync(request.UserId, request.Date);

        if (log == null) 
        {
            return new DiaryDto { Date = request.Date };
        }

        return new DiaryDto
        {
            Date = log.Date,
            TotalCalories = log.TotalCalories,
            Items = log.Items.Select(item => 
           {
            ArgumentNullException.ThrowIfNull(item.FoodProduct); 
            return new MealLogItemDto
            {
                FoodName = item.FoodProduct.Name,
                Grams = item.Grams,
                Calories = (int)(item.Grams * item.FoodProduct.CaloriesPer100g) / 100
            };
        }).ToList()
    };
    }
}