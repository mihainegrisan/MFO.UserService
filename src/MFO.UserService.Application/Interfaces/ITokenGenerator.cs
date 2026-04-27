using MFO.UserService.Domain.Entities;

namespace MFO.UserService.Application.Interfaces;

public interface ITokenGenerator
{
    string GenerateAccessToken(User user);

    string GenerateRefreshToken();
}