using FluentResults;
using MediatR;
using MFO.UserService.Application.DTOs;
using MFO.UserService.Application.Interfaces;
using MFO.UserService.Domain.Entities;
using MFO.UserService.Domain.Errors;

namespace MFO.UserService.Application.CommandsQueries.Commands;

public record AuthenticateUserCommand(string Email, string Password) : IRequest<Result<AuthenticationResponse>>;

public class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, Result<AuthenticationResponse>>
{
    private IUserRepository _userRepository;
    private IPasswordHasherService _passwordHasherService;
    private ITokenGenerator _tokenGenerator;
    private IRefreshTokenRepository _refreshTokenRepository;

    public AuthenticateUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasherService passwordHasherService,
        ITokenGenerator tokenGenerator,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _passwordHasherService = passwordHasherService;
        _tokenGenerator = tokenGenerator;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<Result<AuthenticationResponse>> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            return Result.Fail(new NotFoundError($"User with Email '{request.Email}' not found."));
        }

        // At this point, we found a user which must have a password hash
        if (!_passwordHasherService.VerifyPassword(user.PasswordHash, request.Password))
        {
            return Result.Fail(new UnauthorizedAccessError("Unauthorized access for the provided email and password."));
        }

        var accessToken = _tokenGenerator.GenerateAccessToken(user);
        var refreshToken = _tokenGenerator.GenerateRefreshToken();

        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            RefreshTokenId = Guid.CreateVersion7(),
            UserId = user.UserId,
            Token = refreshToken,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        }, cancellationToken);

        return Result.Ok(new AuthenticationResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }
}