using Hello100Admin.Modules.Auth.Domain.Entities;

namespace Hello100Admin.Modules.Auth.Application.Interfaces;

/// <summary>
/// JWT 토큰 서비스 인터페이스
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Access Token 생성
    /// </summary>
    string GenerateAccessToken(User user, IEnumerable<string> roles);

    /// <summary>
    /// Refresh Token 생성
    /// </summary>
    RefreshToken GenerateRefreshToken(string userId, string? ipAddress = null);

    /// <summary>
    /// Refresh Token 검증
    /// </summary>
    Task<bool> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refresh Token으로 사용자 조회
    /// </summary>
    Task<User?> GetUserByRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
}
