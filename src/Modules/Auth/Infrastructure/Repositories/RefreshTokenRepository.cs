using System.Data;
using Dapper;
using Hello100Admin.Modules.Auth.Domain.Entities;
using Hello100Admin.Modules.Auth.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Auth.Infrastructure.Repositories;

/// <summary>
/// Dapper 기반 RefreshToken 저장소 구현체
/// </summary>
public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IDbConnection _connection;
    private readonly ILogger<RefreshTokenRepository> _logger;

    public RefreshTokenRepository(IDbConnection connection, ILogger<RefreshTokenRepository> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task<RefreshToken> AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Adding RefreshToken. Aid: {Aid}, Token: {Token}", refreshToken.Aid, refreshToken.Token);
            var sql = @"INSERT INTO tb_admin_refresh_token (Id, aid, Token, ExpiresAt, IsRevoked, RevokedByIp, RevokedAt, ReplacedByToken, CreatedByIp, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, IsDeleted)
VALUES (@Id, @Aid, @Token, @ExpiresAt, @IsRevoked, @RevokedByIp, @RevokedAt, @ReplacedByToken, @CreatedByIp, @CreatedAt, @UpdatedAt, @CreatedBy, @UpdatedBy, @IsDeleted)";
            await _connection.ExecuteAsync(sql, refreshToken);
            return refreshToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding RefreshToken. Aid: {Aid}, Token: {Token}", refreshToken.Aid, refreshToken.Token);
            throw;
        }
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting RefreshToken by Token: {Token}", token);
            var sql = "SELECT * FROM tb_admin_refresh_token WHERE Token = @Token";
            var result = await _connection.QueryFirstOrDefaultAsync<RefreshToken>(sql, new { Token = token });
            if (result == null)
            {
                _logger.LogWarning("No RefreshToken found for Token: {Token}", token);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting RefreshToken by Token: {Token}", token);
            throw;
        }
    }

    // 필요시 사용자 조회 메서드 추가 가능

    public async Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating RefreshToken. Token: {Token}", refreshToken.Token);
            var sql = @"UPDATE tb_admin_refresh_token SET ExpiresAt = @ExpiresAt, CreatedByIp = @CreatedByIp WHERE Token = @Token";
            await _connection.ExecuteAsync(sql, refreshToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating RefreshToken. Token: {Token}", refreshToken.Token);
            throw;
        }
    }

    public async Task RevokeUserTokensAsync(string aid, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Revoking all RefreshTokens for Aid: {Aid}", aid);
            var sql = "DELETE FROM tb_admin_refresh_token WHERE aid = @Aid";
            await _connection.ExecuteAsync(sql, new { Aid = aid });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking RefreshTokens for Aid: {Aid}", aid);
            throw;
        }
    }
}

