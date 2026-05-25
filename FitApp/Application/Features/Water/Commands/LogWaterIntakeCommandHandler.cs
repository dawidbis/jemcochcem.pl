using FitApp.Domain.Entities;
using MediatR;

public class LogWaterIntakeCommandHandler : IRequestHandler<LogWaterIntakeCommand, Unit>
{
    private readonly IWaterLogRepository _waterRepo;

    public LogWaterIntakeCommandHandler(IWaterLogRepository waterRepo) => _waterRepo = waterRepo;

    public async Task<Unit> Handle(LogWaterIntakeCommand request, CancellationToken cancellationToken)
    {
        var waterLog = await _waterRepo.GetByDateAsync(request.UserId, request.Date);

        if (waterLog == null)
        {
            waterLog = new WaterLog(request.UserId, request.Date, request.AmountMl);
            await _waterRepo.AddAsync(waterLog);
        }
        else
        {
            waterLog.AmountMl += request.AmountMl; // Inkrementacja (np. dodanie kolejnych 250ml)
            await _waterRepo.UpdateAsync(waterLog);
        }

        await _waterRepo.SaveChangesAsync();
        return Unit.Value;
    }
}