namespace FitApp.Application.Features.Users;

using MediatR;
using FitApp.Domain.Entities;
using FitApp.Infrastructure.Interfaces;

public class AddMeasurementCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public decimal Weight { get; set; }
    public DateTime Date { get; set; }
}

public class AddMeasurementHandler : IRequestHandler<AddMeasurementCommand, Guid>
{
    private readonly IBodyMeasurementRepository _repository;

    public AddMeasurementHandler(IBodyMeasurementRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(AddMeasurementCommand request, CancellationToken ct)
    {
        var measurement = new BodyMeasurement
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Weight = request.Weight,
            Date = request.Date
        };

        await _repository.AddAsync(measurement);
        return measurement.Id;
    }
}