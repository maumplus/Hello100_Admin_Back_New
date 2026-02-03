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

        public async Task UpdateLoginFailureAsync(AdminEntity admin, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Updating login failure for User. Aid: {Aid}", admin.Aid);

                var sql = @"
                        UPDATE tb_admin
                           SET login_fail_count = @LoginFailCount,
                               account_locked = @AccountLocked
                         WHERE aid = @Aid";

                using var connection = _connectionFactory.CreateConnection();
                await connection.ExecuteAsync(sql, new
                {
                    admin.Aid,
                    admin.LoginFailCount,
                    admin.AccountLocked
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating login failure for User. Aid: {Aid}", admin.Aid);
                throw;
            }
        }

        public async Task UpdateLoginSuccessAsync(AdminEntity admin, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Updating login success for User. Aid: {Aid}", admin.Aid);
                var sql = @"
                        UPDATE tb_admin
                           SET last_login_dt = UNIX_TIMESTAMP(NOW()),
                               login_fail_count = 0,
                               account_locked = 'N'
                         WHERE aid = @Aid";

                using var connection = _connectionFactory.CreateConnection();
                await connection.ExecuteAsync(sql, new
                {
                    Aid = admin.Aid,
                    AccessToken = admin.AccessToken,
                    RefreshToken = admin.RefreshToken,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating login success for User. Aid: {Aid}", admin.Aid);
                throw;
            }
        }

        public async Task UpdateTokensAsync(AdminEntity admin, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Updating tokens for User. Aid: {Aid}", admin.Aid);

                var sql = @"
                        UPDATE tb_admin
                           SET access_token = @AccessToken,
                               refresh_token = @RefreshToken
                         WHERE aid = @Aid";

                using var connection = _connectionFactory.CreateConnection();
                await connection.ExecuteAsync(sql, new
                {
                    admin.Aid,
                    admin.AccessToken,
                    admin.RefreshToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tokens for User. Aid: {Aid}", admin.Aid);
                throw;
            }
        }

        public async Task UpdateAccessTokenAsync(AdminEntity admin, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Updating Access Token for User. Aid: {Aid}", admin.Aid);

                var sql = @"
                        UPDATE tb_admin
                           SET access_token = @AccessToken
                         WHERE aid = @Aid";

                using var connection = _connectionFactory.CreateConnection();
                await connection.ExecuteAsync(sql, new
                {
                    admin.Aid,
                    admin.AccessToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Access Token for User. Aid: {Aid}", admin.Aid);
                throw;
            }
        }

        public async Task InsertAdminLogAsync(AdminLogEntity adminLog, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Inserting Admin Log. Aid: {Aid}", adminLog.Aid);

                var sql = @"
                        INSERT INTO tb_admin_log (aid, user_agent, ip, reg_dt)
                        VALUES (@Aid, @UserAgent, @Ip, UNIX_TIMESTAMP(NOW()));";

                using var connection = _connectionFactory.CreateConnection();
                await connection.ExecuteAsync(sql, new
                {
                    adminLog.Aid,
                    adminLog.UserAgent,
                    adminLog.IP
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Inserting Admin Log. Aid: {Aid}", adminLog.Aid);
                throw;
            }
        }

        public async Task<int> InsertAsync(AppAuthNumberInfoEntity appAuthNumberInfo, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Inserting App Auth Number Info.");

                var sql = @"
                        INSERT INTO tb_app_auth_number_info
                          (app_cd, `key`, auth_number, confirmYn, reg_dt)
                        VALUES
                          (@AppCd, @Key, @AuthNumber, 'N', UNIX_TIMESTAMP(NOW()));

                        SELECT LAST_INSERT_ID();";

                using var connection = _connectionFactory.CreateConnection();
                return await connection.ExecuteScalarAsync<int>(sql, new
                {
                    appAuthNumberInfo.AppCd,
                    appAuthNumberInfo.Key,
                    appAuthNumberInfo.AuthNumber
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Inserting App Auth Number Info");
                throw;
            }
        }

        public async Task UpdateConfirmAsync(AppAuthNumberInfoEntity appAuthNumberInfo, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Updating App Auth Number Info for AuthId. AuthId: {AuthId}", appAuthNumberInfo.AuthId);

                var sql = @"
                        UPDATE tb_app_auth_number_info
                           SET confirmYn = 'Y',
                               mod_dt    = UNIX_TIMESTAMP(NOW())
                         WHERE auth_id = @AuthId;";

                using var connection = _connectionFactory.CreateConnection();
                await connection.ExecuteAsync(sql, new
                {
                    appAuthNumberInfo.AuthId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Updating App Auth Number Info for AuthId. AuthId: {AuthId}", appAuthNumberInfo.AuthId);
                throw;
            }
        }
    }
}
