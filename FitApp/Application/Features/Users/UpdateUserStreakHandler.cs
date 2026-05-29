namespace FitApp.Application.Features.Users;

using MediatR;
using FitApp.Infrastructure.Interfaces;
public class UpdateUserStreakHandler : IRequestHandler<UpdateUserStreakCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IMealLogRepository _mealLogRepository;
    private readonly IDietStreakService _streakService;

    public UpdateUserStreakHandler(
        IUserRepository userRepository, 
        IMealLogRepository mealLogRepository, 
        IDietStreakService streakService)
    {
        _userRepository = userRepository;
        _mealLogRepository = mealLogRepository;
        _streakService = streakService;
    }

   public async Task<Guid> Handle(UpdateUserStreakCommand request, CancellationToken ct)
{
    var user = await _userRepository.GetByIdAsync(request.UserId);
    if (user == null) throw new KeyNotFoundException("User not found");

    var today = DateTime.UtcNow;
    
    var log = await _mealLogRepository.GetByDateAsync(user.Id, today.Date);

    decimal consumedCalories = log?.TotalCalories ?? 0m;

    _streakService.CalculateStreak(user, consumedCalories, request.TargetCalories, today);

    await _userRepository.UpdateAsync(user);

    return user.Id;
}
}