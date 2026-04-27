namespace MFO.UserService.Domain.Entities;

public class RefreshToken
{
    public Guid RefreshTokenId { get; set; }

    public Guid UserId { get; set; }

    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAtUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? RevokedAtUtc { get; set; }

    public string? ReplacedByToken { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;

    public bool IsRevoked => RevokedAtUtc != null;

    public bool IsActive => !IsExpired && !IsRevoked;
}