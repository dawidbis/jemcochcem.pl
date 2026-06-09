using FitApp.Domain.Entities;

namespace FitApp.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}