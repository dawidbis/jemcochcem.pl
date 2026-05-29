using MediatR;
using System;

namespace FitApp.Application.Features.Users;
public record UpdateUserStreakCommand(Guid UserId, decimal TargetCalories) : IRequest<Guid>;