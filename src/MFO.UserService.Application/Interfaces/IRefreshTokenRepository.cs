using MFO.UserService.Domain.Entities;

namespace MFO.UserService.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
}