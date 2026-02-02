using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Services;
using Hello100Admin.Modules.Auth.Application.Features.Auth.ReadModels;
using Hello100Admin.Modules.Auth.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Hello100Admin.Modules.Auth.Application.Common.Services;

/// <summary>
/// JWT Token Service 구현
/// </summary>
public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly IAuthRepository _authRepository;
    private readonly IAuthStore _authStore;
    private readonly ILogger<JwtTokenService> _logger;
    private readonly string? _secretKey;
    private readonly string? _issuer;
    private readonly string? _audience;
    private readonly int _accessTokenExpirationMinutes;
    private readonly int _refreshTokenExpirationDays;

    public JwtTokenService(
        IConfiguration configuration,
        IAuthRepository authRepository,
        IAuthStore authStore,
        ILogger<JwtTokenService> logger)
    {
        _configuration = configuration;
        _authRepository = authRepository;
        _authStore = authStore;
        _logger = logger;
        _secretKey = _configuration["Jwt:SecretKey"];
        _issuer = _configuration["Jwt:Issuer"];
        _audience = _configuration["Jwt:Audience"];
        _accessTokenExpirationMinutes = int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "30");
        _refreshTokenExpirationDays = int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "1");
    }

    public string GenerateAccessToken(AdminModel adminInfo, IEnumerable<string> roles)
    {
        if (_secretKey == null)
        {
            throw new InvalidOperationException("JWT SecretKey is not configured");
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, adminInfo.Aid),
            new Claim("accId", adminInfo.AccId),
            new Claim("name", adminInfo.Name),
            new Claim("grade", adminInfo.Grade),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        // 역할 추가
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
            
        if (!string.IsNullOrEmpty(adminInfo.HospNo) && !string.IsNullOrEmpty(adminInfo.HospKey))
        {
            claims.Add(new Claim("hospNo", adminInfo.HospNo));
            claims.Add(new Claim("hospKey", adminInfo.HospKey));
        }

        var expiresAt = DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token); ;
    }

    public RefreshTokenEntity GenerateRefreshToken(string userId, string? ipAddress = null)
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        
        var token = Convert.ToBase64String(randomBytes);
        var expiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);

        var refreshToken = new RefreshTokenEntity(userId, token, expiresAt, ipAddress);
        _logger.LogInformation("Refresh token generated successfully for UserId={UserId}, ExpiresAt={ExpiresAt}",
            userId, expiresAt);

        return refreshToken;
    }

    public async Task<bool> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Validating refresh token");
        var refreshToken = await _authStore.GetByTokenAsync(token, cancellationToken);
        
        if (refreshToken == null)
        {
            _logger.LogWarning("Refresh token validation failed: token not found");
            return false;
        }

        // 만료 여부 확인
        if (refreshToken.IsExpired)
        {
            _logger.LogWarning("Refresh token validation failed: token expired for UserId={UserId}", refreshToken.Aid);
            return false;
        }

        // 취소 여부 확인
        if (refreshToken.IsRevoked)
        {
            _logger.LogWarning("Refresh token validation failed: token revoked for UserId={UserId}", refreshToken.Aid);
            return false;
        }

        _logger.LogDebug("Refresh token validated successfully for UserId={UserId}", refreshToken.Aid);
        return true;
    }

    public async Task<AdminModel?> GetUserByRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _authStore.GetByTokenAsync(token, cancellationToken);
        
        if (refreshToken == null || refreshToken.IsExpired || refreshToken.IsRevoked)
        {
            return null;
        }

        return await _authStore.GetAdminByAidAsync(refreshToken.Aid, cancellationToken);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken || 
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }

    public Guid? GetUserIdFromToken(string token)
    {
        var principal = ValidateToken(token);
        if (principal == null)
        {
            return null;
        }

        var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return null;
        }

        return userId;
    }
}
