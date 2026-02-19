using System.Data;
using System.Text;
using Dapper;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.AdminUser;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.AdminUser.Results;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Infrastructure.Persistence.DbModels.AdminUser;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.AdminUser
{
    public class AdminUserStore : IAdminUserStore
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<AdminUserStore> _logger;

        public AdminUserStore(IDbConnectionFactory connectionFactory, ILogger<AdminUserStore> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<ListResult<GetAdminUsersResult>> GetAdminUsersAsync(DbSession db, int pageNo, int pageSize, CancellationToken ct)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Limit", pageSize, DbType.Int32);
            parameters.Add("OffSet", (pageNo - 1) * pageSize, DbType.Int32);

            #region == Query ==
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" SET @total_count:= (SELECT COUNT(*) FROM tb_admin WHERE grade IN ('S', 'A') AND del_yn = 'N');");
            sb.AppendLine(" SET @row_num:=@total_count + 1 - @OffSet;      ");
            sb.AppendLine(" SELECT 	@row_num := @row_num - 1 AS RowNum     ");
            sb.AppendLine("    ,    a.aid                      AS AId      ");
            sb.AppendLine("    , 	a.acc_id                   AS AccId    ");
            sb.AppendLine("    ,    a.acc_pwd                  AS AccPwd   ");
            sb.AppendLine("    , 	a.hosp_no                  AS HospNo   ");
            sb.AppendLine("    , 	a.grade                    AS Grade    ");
            sb.AppendLine("    , 	b.cm_name                  AS GradeName");
            sb.AppendLine("    , 	a.name                     AS Name     ");
            sb.AppendLine("    , 	a.tel                      AS Tel      ");
            sb.AppendLine("    , 	a.del_yn                   AS DelYn    ");
            sb.AppendLine("    , 	CASE WHEN last_login_dt IS NULL THEN '' ELSE FROM_UNIXTIME(a.last_login_dt, '%Y-%m-%d %H:%i')  END AS LastLoginDt ");
            sb.AppendLine("  FROM tb_admin a                               ");
            sb.AppendLine("  LEFT JOIN tb_common b                         ");
            sb.AppendLine("    ON b.cls_cd = '07' AND b.del_yn = 'N' AND a.grade = b.cm_cd");
            sb.AppendLine(" WHERE a.grade IN ('S', 'A')                    ");
            sb.AppendLine("   AND a.del_yn = 'N'                           ");
            sb.AppendLine(" ORDER BY a.aid DESC	                           ");
            sb.AppendLine(" LIMIT @OffSet, @Limit;                         ");
            sb.AppendLine(" SELECT @total_count;                           ");
            #endregion

            var multi = await db.QueryMultipleAsync(sb.ToString(), parameters, ct, _logger);

            var result = new ListResult<GetAdminUsersResult>
            {
                Items = (await multi.ReadAsync<GetAdminUsersResult>()).ToList(),
                TotalCount = Convert.ToInt32(await multi.ReadFirstAsync<long>())
            };

            return result;
        }

        public async Task<AdminUserEntity?> GetByIdAsync(string accountId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting AdminUser by AccountId: {AccountId}", accountId);
                using var connection = _connectionFactory.CreateConnection();
                var sql = "SELECT * FROM tb_admin WHERE account_id = @AccountId AND is_deleted = 0 LIMIT 1";
                var dbUser = await connection.QueryFirstOrDefaultAsync<AdminUserRow>(sql, new { AccountId = accountId });
                if (dbUser == null)
                {
                    _logger.LogWarning("No AdminUser found for AccountId: {AccountId}", accountId);
                    return null;
                }
                return MapToDomain(dbUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting AdminUser by AccountId: {AccountId}", accountId);
                throw;
            }
        }

        public async Task<AdminUserEntity?> GetByIdWithAdminUserAsync(string accountId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting AdminUser (with details) by AccountId: {AccountId}", accountId);
                using var connection = _connectionFactory.CreateConnection();
                var sql = "SELECT * FROM tb_admin WHERE account_id = @AccountId AND is_deleted = 0 LIMIT 1";
                var dbUser = await connection.QueryFirstOrDefaultAsync<AdminUserRow>(sql, new { AccountId = accountId });
                if (dbUser == null)
                {
                    _logger.LogWarning("No AdminUser (with details) found for AccountId: {AccountId}", accountId);
                    return null;
                }
                return MapToDomain(dbUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting AdminUser (with details) by AccountId: {AccountId}", accountId);
                throw;
            }
        }

        public async Task<AdminUserEntity?> GetByIdIncludeDeletedAsync(string accountId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting AdminUser (include deleted) by AccountId: {AccountId}", accountId);
                using var connection = _connectionFactory.CreateConnection();
                var sql = "SELECT * FROM tb_admin WHERE account_id = @AccountId LIMIT 1";
                var dbUser = await connection.QueryFirstOrDefaultAsync<AdminUserRow>(sql, new { AccountId = accountId });
                if (dbUser == null)
                {
                    _logger.LogWarning("No AdminUser (include deleted) found for AccountId: {AccountId}", accountId);
                    return null;
                }
                return MapToDomain(dbUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting AdminUser (include deleted) by AccountId: {AccountId}", accountId);
                throw;
            }
        }

        public async Task<(List<AdminUserEntity> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, bool includeDeleted = false, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting paged AdminUsers. Page: {Page}, PageSize: {PageSize}, IncludeDeleted: {IncludeDeleted}", page, pageSize, includeDeleted);
                using var connection = _connectionFactory.CreateConnection();
                var offset = (page - 1) * pageSize;
                var sql = includeDeleted
                    ? "SELECT * FROM tb_admin LIMIT @PageSize OFFSET @Offset"
                    : "SELECT * FROM tb_admin WHERE del_yn = 'N' LIMIT @PageSize OFFSET @Offset";
                var dbItems = (await connection.QueryAsync<AdminUserRow>(sql, new { PageSize = pageSize, Offset = offset })).ToList();
                var items = dbItems.Select(MapToDomain).ToList();
                var countSql = includeDeleted ? "SELECT COUNT(*) FROM tb_admin" : "SELECT COUNT(*) FROM tb_admin WHERE del_yn = 'N'";
                var totalCount = await connection.ExecuteScalarAsync<int>(countSql);
                return (items, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged AdminUsers. Page: {Page}, PageSize: {PageSize}, IncludeDeleted: {IncludeDeleted}", page, pageSize, includeDeleted);
                throw;
            }
        }

        // DB 모델 → 도메인 모델 매핑
        private AdminUserEntity MapToDomain(AdminUserRow db)
        {
            return new AdminUserEntity
            {
                Aid = db.Aid,
                AccId = db.AccountId,
                AccPwd = db.Password ?? string.Empty,
                Grade = db.Role ?? string.Empty,
                Name = db.Name ?? string.Empty,
                DelYn = db.DelYn ?? "N",
                AccountLocked = db.AccountLocked ?? "0",
                Approved = db.Approved ?? "1",
                Enabled = db.Enabled ?? "1",
            };
        }
    }
}
