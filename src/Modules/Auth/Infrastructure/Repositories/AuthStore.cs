using Dapper;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth;
using Hello100Admin.Modules.Auth.Domain.Entities;
using Hello100Admin.Modules.Auth.Infrastructure.Persistence.DbModels.Auth;
using Mapster;
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

        public async Task<UserEntity?> GetAdminInfoByAIdAsync(string aid, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting User by AId: {Aid}", aid);

                var sql = @"
                    SELECT a.aid                  AS AId,
                           a.acc_id               AS AccId,
                           a.acc_pwd              AS AccPwd,
                           a.hosp_no              AS HospNo,
                           b.hosp_key             AS HospKey,
                           a.grade                AS Grade,
                           a.name                 AS Name,
                           a.tel                  AS Tel,
                           a.email                AS Email,
                           a.del_yn               AS DelYn,
                           a.reg_dt               AS RegDt,
                           a.last_login_dt        AS LastLoginDt,
                           a.agree_dt             AS AgreeDt,
                           a.role_id              AS RoleId,
                           a.use_2fa              AS Use2Fa,
                           a.account_locked       AS AccountLocked,
                           a.login_fail_count     AS LoginFailCount,
                           a.last_pwd_change_dt   AS LastPwdChangeDt,
                           a.access_token         AS AccessToken,
                           a.refresh_token        AS RefresgToken,
                           a.2fa_key              AS 2FaKey
                      FROM tb_admin a
                      LEFT JOIN tb_eghis_hosp_info b
                             ON a.hosp_no = b.hosp_no
                     WHERE a.aid = @AId
                       AND a.del_yn = 'N'
                     LIMIT 1";

                using var connection = _connectionFactory.CreateConnection();
                var queryResult = await connection.QueryFirstOrDefaultAsync<UserDbRow>(sql, new { AId = aid });

                if (queryResult == null)
                {
                    _logger.LogWarning("No User found for Aid: {Aid}", aid);
                    return null;
                }

                // 추후 필요 시 Global setting으로 뺄 예정
                var config = new TypeAdapterConfig();

                config.NewConfig<UserDbRow, UserEntity>()
                    .Ignore(dest => dest.LastLoginDt!);

                var result = queryResult.Adapt<UserEntity>(config);
                result.LastLoginDt = DateTimeOffset.FromUnixTimeSeconds(queryResult.LastLoginDt ?? 0).DateTime;

                return result;
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

                var sql = @"
                    SELECT a.aid                  AS AId,
                           a.acc_id               AS AccId,
                           a.acc_pwd              AS AccPwd,
                           a.hosp_no              AS HospNo,
                           b.hosp_key             AS HospKey,
                           a.grade                AS Grade,
                           a.name                 AS Name,
                           a.tel                  AS Tel,
                           a.email                AS Email,
                           a.del_yn               AS DelYn,
                           a.reg_dt               AS RegDt,
                           a.last_login_dt        AS LastLoginDt,
                           a.agree_dt             AS AgreeDt,
                           a.role_id              AS RoleId,
                           a.use_2fa              AS Use2Fa,
                           a.account_locked       AS AccountLocked,
                           a.login_fail_count     AS LoginFailCount,
                           a.last_pwd_change_dt   AS LastPwdChangeDt,
                           a.access_token         AS AccessToken,
                           a.refresh_token        AS RefresgToken,
                           a.2fa_key              AS 2FaKey
                      FROM tb_admin a
                      LEFT JOIN tb_eghis_hosp_info b
                             ON a.hosp_no = b.hosp_no
                     WHERE a.acc_id = @AccId
                       AND a.del_yn = 'N'
                     LIMIT 1";

                using var connection = _connectionFactory.CreateConnection();
                var queryResult = await connection.QueryFirstOrDefaultAsync<UserDbRow>(sql, new { AccId = accId });

                if (queryResult == null)
                {
                    _logger.LogWarning("No User found for AccId: {AccId}", accId);
                    return null;
                }

                // 추후 필요 시 Global setting으로 뺄 예정
                var config = new TypeAdapterConfig();

                config.NewConfig<UserDbRow, UserEntity>()
                    .Ignore(dest => dest.LastLoginDt!);

                var result = queryResult.Adapt<UserEntity>(config);
                result.LastLoginDt = DateTimeOffset.FromUnixTimeSeconds(queryResult.LastLoginDt ?? 0).DateTime;

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting User by AccId: {AccId}", accId);
                throw;
            }
        }
    }
}
