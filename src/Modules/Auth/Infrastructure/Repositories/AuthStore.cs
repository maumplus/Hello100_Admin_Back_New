using Dapper;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth;
using Hello100Admin.Modules.Auth.Application.Common.Errors;
using Hello100Admin.Modules.Auth.Application.Common.Extensions;
using Hello100Admin.Modules.Auth.Application.Common.Models;
using Hello100Admin.Modules.Auth.Application.Features.Auth.ReadModels;
using Hello100Admin.Modules.Auth.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Data;

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

        public async Task<AdminModel?> GetAdminByAidAsync(string aid, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting Admin by Aid: {Aid}", aid);

                var parameters = new DynamicParameters();
                parameters.Add("Aid", aid, DbType.String);

                var sql = @"
                    SELECT a.aid                                                                AS AId,
                           a.acc_id                                                             AS AccId,
                           a.acc_pwd                                                            AS AccPwd,
                           a.hosp_no                                                            AS HospNo,
                           b.hosp_key                                                           AS HospKey,
                           a.grade                                                              AS Grade,
                           a.name                                                               AS Name,
                           a.tel                                                                AS Tel,
                           a.email                                                              AS Email,
                           DATE_FORMAT(FROM_UNIXTIME(a.last_login_dt), '%Y-%m-%d %H:%i:%s')     AS LastLoginDt,
                           a.use_2fa                                                            AS Use2fa,
                           a.account_locked                                                     AS AccountLocked,
                           a.login_fail_count                                                   AS LoginFailCount,
                           a.access_token                                                       AS AccessToken,
                           a.refresh_token                                                      AS RefresgToken
                      FROM tb_admin a
                      LEFT JOIN tb_eghis_hosp_info b
                             ON a.hosp_no = b.hosp_no
                     WHERE a.aid = @Aid
                       AND a.del_yn = 'N'
                    LIMIT 1";

                using var connection = _connectionFactory.CreateConnection();

                return await connection.QueryFirstOrDefaultAsync<AdminModel>(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Admin by Aid: {Aid}", aid);
                throw;
            }
        }

        public async Task<AdminModel?> GetAdminByAccIdAsync(string accId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting Admin by AccId: {AccId}", accId);

                var parameters = new DynamicParameters();
                parameters.Add("AccId", accId, DbType.String);

                var sql = @"
                    SELECT a.aid                                                                AS AId,
                           a.acc_id                                                             AS AccId,
                           a.acc_pwd                                                            AS AccPwd,
                           a.hosp_no                                                            AS HospNo,
                           b.hosp_key                                                           AS HospKey,
                           a.grade                                                              AS Grade,
                           a.name                                                               AS Name,
                           a.tel                                                                AS Tel,
                           a.email                                                              AS Email,
                           DATE_FORMAT(FROM_UNIXTIME(a.last_login_dt), '%Y-%m-%d %H:%i:%s')     AS LastLoginDt,
                           a.use_2fa                                                            AS Use2fa,
                           a.account_locked                                                     AS AccountLocked,
                           a.login_fail_count                                                   AS LoginFailCount,
                           a.access_token                                                       AS AccessToken,
                           a.refresh_token                                                      AS RefresgToken
                      FROM tb_admin a
                      LEFT JOIN tb_eghis_hosp_info b
                             ON a.hosp_no = b.hosp_no
                     WHERE a.acc_id = @AccId
                       AND a.del_yn = 'N'
                    LIMIT 1";

                using var connection = _connectionFactory.CreateConnection();

                return await connection.QueryFirstOrDefaultAsync<AdminModel>(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Admin by AccId: {AccId}", accId);
                throw;
            }
        }

        public async Task<AppAuthNumberInfoEntity?> GetAppAuthNumberInfoAsync(int authId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting App Auth Number Info by AuthId: {AuthId}", authId);

                var parameters = new DynamicParameters();
                parameters.Add("AuthId", authId, DbType.Int32);

                var sql = @"
                    SELECT auth_id                                                 AS AuthId,
                           app_cd                                                  AS AppCd,
                           `key`                                                   AS `Key`,
                           auth_number                                             AS AuthNumber,
                           confirmYn                                               AS ConfirmYn,
                           mod_dt                                                  AS ModUnixDt,
                           DATE_FORMAT(FROM_UNIXTIME(mod_dt), '%Y-%m-%d %H:%i:%s') AS ModDt,
                           reg_dt                                                  AS RegUnixDt,
                           DATE_FORMAT(FROM_UNIXTIME(reg_dt), '%Y-%m-%d %H:%i:%s') AS RegDt
                      FROM tb_app_auth_number_info
                     WHERE auth_id = @AuthId;";

                using var connection = _connectionFactory.CreateConnection();

                return await connection.QueryFirstOrDefaultAsync<AppAuthNumberInfoEntity>(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting App Auth Number Info by AuthId: {AuthId}", authId);
                throw;
            }
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

        public async Task<CurrentHospitalInfo?> GetHospitalInfoByHospNoAsync(DbSession db, string hospNo, CancellationToken cancellationToken = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@HospNo", hospNo, DbType.String);

            var query = @"
                    SELECT z.hosp_key                                      AS HospKey,
                           z.hosp_no                                       AS HospNo,
                           z.name                                          AS Name,
                           z.hosp_cls_cd                                   AS HospClsCd,
                           z.addr                                          AS Addr,
                           z.post_cd                                       AS PostCd,
                           z.tel                                           AS Tel,
                           z.closing_yn                                    AS ClosingYn,
                           z.del_yn                                        AS DelYn,
                           z.`Desc`                                        AS Descrption,
                           z.md_cd                                         AS MdCd,
                           ( SELECT COUNT(*) 
                               FROM tb_eghis_hosp_device_settings_info 
                              WHERE hosp_no = z.hosp_no 
                                AND device_type = 1 
                                AND use_yn = 'Y' )                         AS KioskCount,
                           ( SELECT COUNT(*) 
                               FROM tb_eghis_hosp_device_settings_info 
                              WHERE hosp_no = z.hosp_no 
                                AND device_type = 2 
                                AND use_yn = 'Y' )                         AS TabletCount,
                           z.chart_type                                    AS ChartType
                      FROM VM_HOSPITAL_DETAIL z
                     WHERE hosp_no = @HospNo
                ";

            var result = await db.QueryFirstOrDefaultAsync<CurrentHospitalInfo>(query, parameters);

            if (result == null)
                throw new BizException(AuthErrorCode.NotFoundHospital.ToError());

            return result;
        }
    }
}
