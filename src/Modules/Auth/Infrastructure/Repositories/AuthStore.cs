using Dapper;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth;
using Hello100Admin.Modules.Auth.Domain.Entities;
using Hello100Admin.Modules.Auth.Infrastructure.Persistence.DbModels.Auth;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Auth.Infrastructure.Repositories
{
    public class AuthStore : IAuthStore
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<AuthStore> _logger;

        public AuthStore(IDbConnectionFactory connectionFactory, ILogger<AuthStore> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<RefreshTokenEntity?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting RefreshToken by Token: {Token}", token);
                var sql = "SELECT * FROM tb_admin_refresh_token WHERE Token = @Token";

                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.QueryFirstOrDefaultAsync<RefreshTokenEntity>(sql, new { Token = token });
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

        public async Task<UserEntity?> GetByAidAsync(string aid, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting User by Aid: {Aid}", aid);
                var sql = "SELECT * FROM tb_admin WHERE aid = @Aid LIMIT 1";
                using var connection = _connectionFactory.CreateConnection();
                var dbUser = await connection.QueryFirstOrDefaultAsync<UserDbRow>(sql, new { Aid = aid });
                if (dbUser == null)
                {
                    _logger.LogWarning("No User found for Aid: {Aid}", aid);
                    return null;
                }
                return MapToDomain(dbUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting User by Aid: {Aid}", aid);
                throw;
            }
        }

        public async Task<UserEntity?> GetByUsernameAsync(string accId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting User by AccId: {AccId}", accId);
                var sql = "SELECT * FROM tb_admin WHERE acc_id = @AccId LIMIT 1";
                using var connection = _connectionFactory.CreateConnection();
                var dbUser = await connection.QueryFirstOrDefaultAsync<UserDbRow>(sql, new { AccId = accId });
                if (dbUser == null)
                {
                    _logger.LogWarning("No User found for AccId: {AccId}", accId);
                    return null;
                }
                return MapToDomain(dbUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting User by AccId: {AccId}", accId);
                throw;
            }
        }

        // DB 모델 → 도메인 모델 매핑
        private UserEntity MapToDomain(UserDbRow db)
        {
            return new UserEntity
            {
                Aid = db.Aid,
                AccId = db.AccId,
                AccPwd = db.AccPwd ?? string.Empty,
                HospNo = db.HospNo,
                Grade = db.Grade,
                Name = db.Name,
                DelYn = db.DelYn,
                LastLoginDt = DateTimeOffset.FromUnixTimeSeconds(db.LastLoginDt ?? 0).DateTime,
                AccountLocked = db.AccountLocked,
                LoginFailCount = db.LoginFailCount,
                RefreshToken = db.RefreshToken,
                AccessToken = db.AccessToken,
                Approved = db.Approved,
                Enabled = db.Enabled
            };
        }
    }
}
