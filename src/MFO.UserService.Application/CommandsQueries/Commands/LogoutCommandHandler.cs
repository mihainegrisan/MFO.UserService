using FluentResults;
using MediatR;
using MFO.UserService.Application.Interfaces;

namespace MFO.UserService.Application.CommandsQueries.Commands;

public sealed record LogoutCommand(string RefreshToken) : IRequest<Result>;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        
        if (storedToken is null || !storedToken.IsActive)
            Result.Ok();

        storedToken.RevokedAtUtc = DateTime.UtcNow;
        await _refreshTokenRepository.UpdateAsync(storedToken, cancellationToken);

        return Result.Ok();
    }
}