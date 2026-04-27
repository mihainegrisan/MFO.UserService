using FluentResults;
using MediatR;
using MFO.UserService.Application.DTOs;
using MFO.UserService.Application.Interfaces;
using MFO.UserService.Domain.Entities;
using MFO.UserService.Domain.Errors;

namespace MFO.UserService.Application.CommandsQueries.Commands;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthenticationResponse>>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthenticationResponse>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITokenGenerator _tokenGenerator;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        ITokenGenerator tokenGenerator)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<Result<AuthenticationResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var currentRefreshTokenEntity = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (currentRefreshTokenEntity is null || !currentRefreshTokenEntity.IsActive)
            return Result.Fail(new UnauthorizedAccessError("Unauthorized"));

        var user = await _userRepository.GetByIdAsync(currentRefreshTokenEntity.UserId, cancellationToken);
        if (user is null)
            return Result.Fail(new UnauthorizedAccessError("Unauthorized"));


        var newRefreshTokenValue = _tokenGenerator.GenerateRefreshToken();

        currentRefreshTokenEntity.RevokedAtUtc = DateTime.UtcNow;
        currentRefreshTokenEntity.ReplacedByToken = newRefreshTokenValue;

        var newRefreshToken = new RefreshToken
        {
            RefreshTokenId = Guid.CreateVersion7(),
            UserId = user.UserId,
            Token = newRefreshTokenValue,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        };

        await _refreshTokenRepository.UpdateAsync(currentRefreshTokenEntity, cancellationToken);
        await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

        var newAccessToken = _tokenGenerator.GenerateAccessToken(user);

        return Result.Ok(new AuthenticationResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenValue
        });
    }
}