using MediatR;
using System;

namespace FitApp.Application.Features.GenerateMealPlan;

public record GenerateMealPlanCommand(Guid UserId, string Prompt) : IRequest<Guid>;