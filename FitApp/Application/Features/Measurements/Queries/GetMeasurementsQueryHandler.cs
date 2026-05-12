namespace FitApp.Application.Features.Measurements;

using MediatR;
using FitApp.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Application.DTOs;

public record GetMeasurementsQuery(Guid UserId) : IRequest<List<MeasurementDto>>;

public class GetMeasurementsHandler : IRequestHandler<GetMeasurementsQuery, List<MeasurementDto>>
{
    private readonly IBodyMeasurementRepository _repository;

    public GetMeasurementsHandler(IBodyMeasurementRepository repository) => _repository = repository;

    public async Task<List<MeasurementDto>> Handle(GetMeasurementsQuery request, CancellationToken ct)
    {
        var measurements = await _repository.GetUserHistoryAsync(request.UserId);
        return measurements.OrderByDescending(m => m.Date)
        .Select(m => new MeasurementDto
        {
            Id = m.Id,
            Weight = m.Weight,
            Date = m.Date
        }).ToList();
    }
}