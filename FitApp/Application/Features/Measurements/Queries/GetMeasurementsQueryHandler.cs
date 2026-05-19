namespace FitApp.Application.Features.Measurements;

using MediatR;
using FitApp.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Application.DTOs;

public record GetMeasurementsQuery(Guid UserId) : IRequest<List<MeasurementDto>>;

public class GetMeasurementsHandler : IRequestHandler<GetMeasurementsQuery, List<MeasurementDto>>
{
    private readonly IBodyMeasurementRepository _measurementRepository;
    private readonly IUserRepository _userRepository;

    public GetMeasurementsHandler(IBodyMeasurementRepository measurementRepository, IUserRepository userRepository)
    {
        _measurementRepository = measurementRepository;
        _userRepository = userRepository;
    }

    public async Task<List<MeasurementDto>> Handle(GetMeasurementsQuery request, CancellationToken ct)
    {
        var measurements = await _measurementRepository.GetUserHistoryAsync(request.UserId);
        var user = await _userRepository.GetByIdAsync(request.UserId);
        var heightM = user?.Height > 0 ? (double?)((double)user.Height / 100.0) : null;

        return measurements
            .OrderByDescending(m => m.Date)
            .Select(m =>
            {
                decimal? bmi = null;
                if (heightM.HasValue)
                    bmi = Math.Round((decimal)((double)m.Weight / (heightM.Value * heightM.Value)), 1);

                return new MeasurementDto
                {
                    Id = m.Id,
                    Date = m.Date,
                    Weight = m.Weight,
                    BodyFatPercentage = m.BodyFatPercentage,
                    Waist = m.Waist,
                    Hips = m.Hips,
                    Notes = m.Notes,
                    Bmi = bmi
                };
            })
            .ToList();
    }
}
