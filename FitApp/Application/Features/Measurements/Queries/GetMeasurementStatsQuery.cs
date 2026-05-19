namespace FitApp.Application.Features.Measurements;

using MediatR;
using FitApp.Infrastructure.Interfaces;
using Application.DTOs;

public record GetMeasurementStatsQuery(Guid UserId) : IRequest<MeasurementStatsDto>;

public class GetMeasurementStatsHandler : IRequestHandler<GetMeasurementStatsQuery, MeasurementStatsDto>
{
    private readonly IBodyMeasurementRepository _measurementRepository;
    private readonly IUserRepository _userRepository;

    public GetMeasurementStatsHandler(IBodyMeasurementRepository measurementRepository, IUserRepository userRepository)
    {
        _measurementRepository = measurementRepository;
        _userRepository = userRepository;
    }

    public async Task<MeasurementStatsDto> Handle(GetMeasurementStatsQuery request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        var measurements = (await _measurementRepository.GetUserHistoryAsync(request.UserId))
            .OrderByDescending(m => m.Date)
            .ToList();

        var stats = new MeasurementStatsDto
        {
            TotalMeasurements = measurements.Count,
            TargetWeight = user?.TargetWeight
        };

        if (measurements.Count == 0)
            return stats;

        var latest = measurements[0];
        stats.LatestWeight = latest.Weight;

        if (measurements.Count >= 2)
        {
            stats.PreviousWeight = measurements[1].Weight;
            stats.WeightChange = Math.Round(latest.Weight - measurements[1].Weight, 2);
        }

        if (user?.Height > 0)
        {
            double heightM = (double)user.Height / 100.0;
            double bmi = (double)latest.Weight / (heightM * heightM);
            stats.CurrentBmi = Math.Round((decimal)bmi, 1);
            stats.BmiCategory = GetBmiCategory(bmi);
        }

        if (user?.TargetWeight.HasValue == true && measurements.Count >= 2)
        {
            var startWeight = measurements[^1].Weight;
            var target = user.TargetWeight.Value;
            var current = latest.Weight;
            var totalChange = Math.Abs(startWeight - target);

            if (totalChange > 0)
            {
                var achieved = Math.Abs(startWeight - current);
                stats.ProgressPercent = Math.Min(100, Math.Round(achieved / totalChange * 100, 1));
            }
        }

        return stats;
    }

    private static string GetBmiCategory(double bmi) => bmi switch
    {
        < 18.5 => "Niedowaga",
        < 25.0 => "Prawidłowa",
        < 30.0 => "Nadwaga",
        _ => "Otyłość"
    };
}
