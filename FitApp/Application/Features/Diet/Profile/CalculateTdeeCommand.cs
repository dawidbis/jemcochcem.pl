namespace FitApp.Application.Features.Profile;

using MediatR;
using FitApp.Domain.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using FitApp.Domain.Interfaces;

public class CalculateTdeeCommand : IRequest<int>
{
    public Guid UserId { get; set; }
    public decimal ActivityLevel { get; set; }
}

public class CalculateTdeeHandler : IRequestHandler<CalculateTdeeCommand, int>
{
    private readonly IUserRepository _userRepository;
    private readonly TdeeCalculationService _tdeeService;

    public CalculateTdeeHandler(IUserRepository userRepository, TdeeCalculationService tdeeService)
    {
        _userRepository = userRepository;
        _tdeeService = tdeeService;
    }

    public async Task<int> Handle(CalculateTdeeCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        
        if (user == null)
        {
            throw new ArgumentException($"User {request.UserId} not found.");
        }

        var tdee = _tdeeService.CalculateTDEE(
            user.Weight, 
            user.Height, 
            user.Age, 
            user.Gender, 
            request.ActivityLevel);

        return tdee;
    }
}