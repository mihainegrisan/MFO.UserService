namespace MFO.UserService.Application.Options;

public sealed class AuthenticationOptions
{
    public string SecretForKey { get; set; }
    public string Issuer { get; set; } // The entity that creates the token (which is this API)
    public string Audience { get; set; }
}