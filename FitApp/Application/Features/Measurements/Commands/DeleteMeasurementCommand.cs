namespace FitApp.Application.Features.Measurements;

using MediatR;
using FitApp.Infrastructure.Interfaces;

public record DeleteMeasurementCommand(Guid Id) : IRequest<bool>;

public class DeleteMeasurementHandler : IRequestHandler<DeleteMeasurementCommand, bool>
{
    private readonly IBodyMeasurementRepository _repository;

    public DeleteMeasurementHandler(IBodyMeasurementRepository repository) => _repository = repository;

    public async Task<bool> Handle(DeleteMeasurementCommand request, CancellationToken ct)
    {
        var measurement = await _repository.GetByIdAsync(request.Id);
        if (measurement == null) return false;

        await _repository.RemoveAsync(measurement);
        return true;
    }
}
