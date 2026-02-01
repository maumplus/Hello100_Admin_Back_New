using Dapper;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth;
using Hello100Admin.Modules.Auth.Domain.Entities;
using Hello100Admin.Modules.Auth.Infrastructure.Persistence.DbModels.Auth;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Auth.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<AuthRepository> _logger;

        public AuthRepository(IDbConnectionFactory connectionFactory, ILogger<AuthRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<RefreshTokenEntity> AddAsync(RefreshTokenEntity refreshToken, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Adding RefreshToken. Aid: {Aid}, Token: {Token}", refreshToken.Aid, refreshToken.Token);
                var sql = @"INSERT INTO tb_admin_refresh_token (Id, aid, Token, ExpiresAt, IsRevoked, RevokedByIp, RevokedAt, ReplacedByToken, CreatedByIp, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, IsDeleted)
VALUES (@Id, @Aid, @Token, @ExpiresAt, @IsRevoked, @RevokedByIp, @RevokedAt, @ReplacedByToken, @CreatedByIp, @CreatedAt, @UpdatedAt, @CreatedBy, @UpdatedBy, @IsDeleted)";

                using var connection = _connectionFactory.CreateConnection();
                await connection.ExecuteAsync(sql, refreshToken);
                return refreshToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding RefreshToken. Aid: {Aid}, Token: {Token}", refreshToken.Aid, refreshToken.Token);
                throw;
            }
        }

        // 필요시 사용자 조회 메서드 추가 가능
        public async Task UpdateAsync(RefreshTokenEntity refreshToken, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Updating RefreshToken. Token: {Token}", refreshToken.Token);
                var sql = @"UPDATE tb_admin_refresh_token SET ExpiresAt = @ExpiresAt, CreatedByIp = @CreatedByIp WHERE Token = @Token";
                using var connection = _connectionFactory.CreateConnection();
                await connection.ExecuteAsync(sql, refreshToken);
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
                using var connection = _connectionFactory.CreateConnection();
                await connection.ExecuteAsync(sql, new { Aid = aid });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking RefreshTokens for Aid: {Aid}", aid);
                throw;
            }
        }

        public async Task UpdateLoginSuccessAsync(UserEntity user, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Updating login success for User. Aid: {Aid}", user.Aid);
                var sql = @"
                        UPDATE tb_admin
                           SET last_login_dt = @LastLoginDt,
                               login_fail_count = 0,
                               account_locked = @AccountLocked,
                               access_token = @AccessToken,
                               refresh_token = @RefreshToken
                         WHERE aid = @Aid";

                using var connection = _connectionFactory.CreateConnection();
                await connection.ExecuteAsync(sql, new
                {
                    Aid = user.Aid,
                    LastLoginDt = user.LastLoginDt,
                    AccountLocked = user.AccountLocked,
                    AccessToken = user.AccessToken,
                    RefreshToken = user.RefreshToken,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating login success for User. Aid: {Aid}", user.Aid);
                throw;
            }
        }

        public async Task UpdateLoginFailureAsync(UserEntity user, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Updating login failure for User. Aid: {Aid}", user.Aid);
                var sql = @"
                        UPDATE tb_admin
                           SET login_fail_count = @LoginFailCount,
                               account_locked = @AccountLocked
                         WHERE aid = @Aid";

                using var connection = _connectionFactory.CreateConnection();
                await connection.ExecuteAsync(sql, new
                {
                    user.LoginFailCount,
                    user.AccountLocked,
                    user.Aid
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating login failure for User. Aid: {Aid}", user.Aid);
                throw;
            }
        }

        public async Task UpdateTokensAsync(UserEntity user, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Updating tokens for User. Aid: {Aid}", user.Aid);
                var sql = @"UPDATE tb_admin SET refresh_token = @RefreshToken WHERE aid = @Aid";
                using var connection = _connectionFactory.CreateConnection();
                await connection.ExecuteAsync(sql, new
                {
                    user.RefreshToken,
                    user.Aid
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tokens for User. Aid: {Aid}", user.Aid);
                throw;
            }
        }
    }
}
