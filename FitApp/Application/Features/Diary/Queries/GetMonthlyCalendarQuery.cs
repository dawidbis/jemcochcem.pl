namespace FitApp.Application.Features.Diet;

using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FitApp.Infrastructure.Interfaces;

public record CalendarDaySummaryDto(
    int TotalCalories,
    decimal TotalProtein,
    decimal TotalCarbs,
    decimal TotalFats,
    int WaterMl);

public record GetMonthlyCalendarQuery(Guid UserId, int Year, int Month)
    : IRequest<Dictionary<string, CalendarDaySummaryDto>>;

public class GetMonthlyCalendarHandler
    : IRequestHandler<GetMonthlyCalendarQuery, Dictionary<string, CalendarDaySummaryDto>>
{
    private readonly IMealLogRepository _mealLogRepo;
    private readonly IWaterLogRepository _waterLogRepo;

    public GetMonthlyCalendarHandler(IMealLogRepository mealLogRepo, IWaterLogRepository waterLogRepo)
    {
        _mealLogRepo = mealLogRepo;
        _waterLogRepo = waterLogRepo;
    }

    public async Task<Dictionary<string, CalendarDaySummaryDto>> Handle(
        GetMonthlyCalendarQuery request, CancellationToken ct)
    {
        var mealLogs  = await _mealLogRepo.GetMonthAsync(request.UserId, request.Year, request.Month);
        var waterLogs = await _waterLogRepo.GetMonthAsync(request.UserId, request.Year, request.Month);

        var waterByDate = new Dictionary<string, int>();
        foreach (var w in waterLogs)
            waterByDate[w.Date.ToString("yyyy-MM-dd")] = w.AmountMl;

        var result = new Dictionary<string, CalendarDaySummaryDto>();
        foreach (var log in mealLogs)
        {
            var key = log.Date.ToString("yyyy-MM-dd");
            var waterMl = waterByDate.TryGetValue(key, out var ml) ? ml : 0;
            if (log.TotalCalories > 0 || waterMl > 0)
                result[key] = new CalendarDaySummaryDto(
                    log.TotalCalories,
                    log.TotalProtein,
                    log.TotalCarbs,
                    log.TotalFats,
                    waterMl);
        }

        foreach (var (key, ml) in waterByDate)
        {
            if (!result.ContainsKey(key) && ml > 0)
                result[key] = new CalendarDaySummaryDto(0, 0, 0, 0, ml);
        }

        return result;
    }
}
