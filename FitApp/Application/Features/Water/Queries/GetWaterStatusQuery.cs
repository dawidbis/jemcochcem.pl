using MediatR;

public record GetWaterStatusQuery(Guid UserId, DateTime Date) : IRequest<WaterStatusDto>;

public record WaterStatusDto(int CurrentAmountMl, int TargetAmountMl, bool IsExtraHydrationRequired, string AlertMessage);