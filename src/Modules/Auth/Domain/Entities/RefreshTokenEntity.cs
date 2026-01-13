using Hello100Admin.BuildingBlocks.Common.Domain;

namespace Hello100Admin.Modules.Auth.Domain.Entities;

/// <summary>
/// Refresh Token 엔티티
/// </summary>
public class RefreshTokenEntity : BaseEntity
{
    public string Aid { get; private set; } = string.Empty;  // tb_admin.aid와 매핑
    public UserEntity User { get; private set; } = null!;

    public string Token { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public string? RevokedByIp { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByToken { get; private set; }
    public string? CreatedByIp { get; private set; }

    private RefreshTokenEntity()
    {
        Token = string.Empty;
    }

    public RefreshTokenEntity(string aid, string token, DateTime expiresAt, string? createdByIp = null)
    {
        Aid = aid ?? throw new ArgumentNullException(nameof(aid));
        Token = token ?? throw new ArgumentNullException(nameof(token));
        ExpiresAt = expiresAt;
        CreatedByIp = createdByIp;
        IsRevoked = false;
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;

    public void Revoke(string? revokedByIp = null, string? replacedByToken = null)
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        RevokedByIp = revokedByIp;
        ReplacedByToken = replacedByToken;
    }
}
