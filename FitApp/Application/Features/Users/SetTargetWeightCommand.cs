namespace FitApp.Application.Features.Users;

using MediatR;
using FitApp.Infrastructure.Interfaces;

public record SetTargetWeightCommand(Guid UserId, decimal? TargetWeight) : IRequest<bool>;

public class SetTargetWeightHandler : IRequestHandler<SetTargetWeightCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public SetTargetWeightHandler(IUserRepository userRepository) => _userRepository = userRepository;

    public async Task<bool> Handle(SetTargetWeightCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null) return false;

        user.TargetWeight = request.TargetWeight;
        await _userRepository.UpdateAsync(user);
        return true;
    }
}
