using MediatR;

public record LogWaterIntakeCommand(Guid UserId, DateTime Date, int AmountMl) : IRequest<Unit>;