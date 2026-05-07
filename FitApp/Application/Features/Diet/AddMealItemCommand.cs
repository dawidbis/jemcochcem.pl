namespace FitApp.Application.Features.Diet;

using MediatR;
using FitApp.Domain.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using FitApp.Infrastructure.Interfaces;
using FitApp.Domain.Entities;
public class AddMealItemCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public Guid FoodProductId { get; set; }
    public decimal Grams { get; set; }
}

public class AddMealItemHandler : IRequestHandler<AddMealItemCommand, Unit>
{
    private readonly IMealLogRepository _mealLogRepository;
    private readonly IFoodRepository _foodRepository;
    private readonly MealLogDomainService _mealLogService;

    public AddMealItemHandler(IMealLogRepository mealLogRepository, IFoodRepository foodRepository, MealLogDomainService mealLogService)
    {
        _mealLogRepository = mealLogRepository;
        _foodRepository = foodRepository;
        _mealLogService = mealLogService;
    }

    public async Task<Unit> Handle(AddMealItemCommand request, CancellationToken ct)
    {
        var log = await _mealLogRepository.GetByDateAsync(request.UserId, request.Date) 
                  ?? new MealLog { Id = Guid.NewGuid(), UserId = request.UserId, Date = request.Date };

        var food = await _foodRepository.GetByIdAsync(request.FoodProductId);
        if (food == null) throw new ArgumentException("Food product not found.");

        var item = new MealLogItem 
        { 
            Id = Guid.NewGuid(), 
            MealLogId = log.Id, 
            FoodProductId = food.Id, 
            Grams = request.Grams, 
            FoodProduct = food 
        };
        
        _mealLogService.AddItemToLog(log, item);

        if (log.Items.Count == 1) await _mealLogRepository.AddAsync(log);
        else await _mealLogRepository.UpdateAsync(log);

        return Unit.Value;
    }
}