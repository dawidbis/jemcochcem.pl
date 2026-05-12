namespace FitApp.Application.Features.Diet;

using MediatR;
using FitApp.Domain.Entities;   
using FitApp.Infrastructure.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

// 1. Definicja danych wejściowych (To, co widać w Request Body w Swaggerze)
public record CreateMeasurementCommand(Guid UserId, decimal Weight, DateTime Date) : IRequest<Guid>;

// 2. Handler - Logika zapisu do bazy
public class CreateMeasurementHandler : IRequestHandler<CreateMeasurementCommand, Guid>
{
    private readonly IBodyMeasurementRepository _repository;

    public CreateMeasurementHandler(IBodyMeasurementRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateMeasurementCommand request, CancellationToken ct)
    {
        // Mapujemy komendę na encję bazodanową
        var measurement = new BodyMeasurement
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Weight = request.Weight,
            Date = request.Date
        };

        // Zapisujemy przez repozytorium
        await _repository.AddAsync(measurement);
        
        return measurement.Id;
    }
}