namespace FitApp.Application.Features.Diet;

using MediatR;
using FitApp.Domain.Entities;
using FitApp.Infrastructure.Interfaces;

public record CreateMeasurementCommand(
    Guid UserId,
    decimal Weight,
    DateTime Date,
    decimal? BodyFatPercentage = null,
    decimal? Waist = null,
    decimal? Hips = null,
    string? Notes = null
) : IRequest<Guid>;

public class CreateMeasurementHandler : IRequestHandler<CreateMeasurementCommand, Guid>
{
    private readonly IBodyMeasurementRepository _repository;

    public CreateMeasurementHandler(IBodyMeasurementRepository repository) => _repository = repository;

    public async Task<Guid> Handle(CreateMeasurementCommand request, CancellationToken ct)
    {
        var measurement = new BodyMeasurement
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Weight = request.Weight,
            Date = request.Date,
            BodyFatPercentage = request.BodyFatPercentage,
            Waist = request.Waist,
            Hips = request.Hips,
            Notes = request.Notes
        };

        await _repository.AddAsync(measurement);
        return measurement.Id;
    }
}
