using MFO.UserService.Application.Interfaces;
using MFO.UserService.Application.Options;
using MFO.UserService.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace MFO.UserService.Application.Services;

public class TokenGenerator : ITokenGenerator
{
    private readonly AuthenticationOptions _authenticationOptions;

    public TokenGenerator(IOptions<AuthenticationOptions> authenticationOptions)
    {
        _authenticationOptions = authenticationOptions.Value;
    }

    public string GenerateAccessToken(User user)
    {
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

        return tokenToReturn;
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }

}