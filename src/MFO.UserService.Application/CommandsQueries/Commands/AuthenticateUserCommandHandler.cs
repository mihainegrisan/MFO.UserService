using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentResults;
using MediatR;
using MFO.UserService.Application.DTOs;
using MFO.UserService.Application.Interfaces;
using MFO.UserService.Application.Options;
using MFO.UserService.Domain.Errors;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MFO.UserService.Application.CommandsQueries.Commands;

public record AuthenticateUserCommand(string Email, string Password) : IRequest<Result<AuthenticationResponse>>;

public class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, Result<AuthenticationResponse>>
{
    private IUserRepository _userRepository;
    private IPasswordHasherService _passwordHasherService;
    private AuthenticationOptions _authenticationOptions;

    public AuthenticateUserCommandHandler(IUserRepository userRepository, IPasswordHasherService passwordHasherService, IOptions<AuthenticationOptions> options)
    {
        _userRepository = userRepository;
        _passwordHasherService = passwordHasherService;
        _authenticationOptions = options.Value;
    }

    public async Task<Result<AuthenticationResponse>> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            return Result.Fail(new NotFoundError($"User with Email '{request.Email}' not found."));
        }

        if (!_passwordHasherService.VerifyPassword(user.PasswordHash, request.Password))
        {
            return Result.Fail(new UnauthorizedAccessError("Unauthorized access for the provided email and password."));
        }

        // Step 2: Create a token
        var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(_authenticationOptions.SecretForKey));

        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claimsForToken = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new (JwtRegisteredClaimNames.GivenName, user.FirstName),
            new (JwtRegisteredClaimNames.FamilyName, user.LastName),
            new (JwtRegisteredClaimNames.Email, user.Email)
        };

        var jwtSecurityToken = new JwtSecurityToken(
            _authenticationOptions.Issuer,
            _authenticationOptions.Audience,
            claimsForToken,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1),
            signingCredentials);

        var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        return Result.Ok(new AuthenticationResponse
        {
            AccessToken = tokenToReturn
        });
    }
}