using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.Modules.Auth.Application.Common.Views;
using Hello100Admin.Modules.Auth.Domain.Entities;

namespace Hello100Admin.Modules.Auth.Application.Common.Abstractions.Services;

/// <summary>
/// JWT 토큰 서비스 인터페이스
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Access Token 생성
    /// </summary>
    string GenerateAccessToken(TbAdminView adminInfo, IEnumerable<string> roles, IEnumerable<ChartType> chartTypes);

    /// <summary>
    /// Refresh Token 생성
    /// </summary>
    RefreshTokenEntity GenerateRefreshToken(string userId, string? ipAddress = null);

    /// <summary>
    /// Refresh Token 검증
    /// </summary>
    Task<bool> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
}
