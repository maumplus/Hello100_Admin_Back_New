using Dapper;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Microsoft.Extensions.Logging;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.AdminUser;
using System.Data;
using System.Text;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.AdminUser;

public class AdminUserRepository : IAdminUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<AdminUserRepository> _logger;

    public AdminUserRepository(IDbConnectionFactory connectionFactory, ILogger<AdminUserRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task SaveAsync(AdminUserEntity adminUser, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Saving AdminUser. Aid: {Aid}", adminUser.Aid);
            using var connection = _connectionFactory.CreateConnection();
            var sql = "UPDATE tb_admin SET is_deleted = 1 WHERE account_id = @AccountId";
            await connection.ExecuteAsync(sql, new { AccountId = adminUser.Aid });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving AdminUser. Aid: {Aid}", adminUser.Aid);
            throw;
        }
    }

    public async Task<int> UpdatePassword(string aId, string encPwd)
    {
        try
        {
            _logger.LogInformation("UpdatePassword() {Id}", aId);

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("AId", aId, DbType.String);
            parameters.Add("AccPwd", encPwd, DbType.String);

            var sql = @"
                UPDATE tb_admin
                   SET acc_pwd = @AccPwd
                 WHERE aid = @AId
            ";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.ExecuteAsync(sql, parameters);

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError("UpdatePassword 비밀번호 변경 실패 {Exception}", e);
            return -1;
        }
    }

    public async Task<int> DeleteHospitalAdminMappingAsync(DbSession db, string aId, string? hospNo, string? hospKey, CancellationToken ct)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("AId", aId, DbType.String);
        parameters.Add("HospKey", hospNo, DbType.String);
        parameters.Add("HospNo", hospKey, DbType.String);

        #region == Query ==
        StringBuilder sb = new StringBuilder();

        if (!string.IsNullOrEmpty(hospKey))
        {
            sb.AppendLine("DELETE                     ");
            sb.AppendLine("  FROM tb_eghis_hosp_info  ");
            sb.AppendLine(" WHERE hosp_key = @HospKey;");

            sb.AppendLine("DELETE                               ");
            sb.AppendLine("  FROM tb_eghis_hosp_medical_time_new");
            sb.AppendLine(" WHERE hosp_key = @HospKey;          ");

            sb.AppendLine("DELETE                            ");
            sb.AppendLine("  FROM tb_eghis_hosp_settings_info");
            sb.AppendLine(" WHERE hosp_key = @HospKey;       ");

            sb.AppendLine("DELETE                                 ");
            sb.AppendLine("  FROM tb_eghis_hosp_visit_purpose_info");
            sb.AppendLine(" WHERE hosp_key = @HospKey;            ");

            sb.AppendLine("DELETE                         ");
            sb.AppendLine("  FROM tb_eghis_recert_doc_info");
            sb.AppendLine(" WHERE hosp_key = @HospKey;    ");

            sb.AppendLine("DELETE                             ");
            sb.AppendLine("  FROM hello100_api.eghis_doct_info");
            sb.AppendLine(" WHERE hosp_key = @HospKey;        ");
        }

        if (!string.IsNullOrEmpty(hospNo))
        {
            sb.AppendLine("DELETE                   ");
            sb.AppendLine("  FROM tb_eghis_hosp_info");
            sb.AppendLine(" WHERE hosp_no = @HospNo;");
        }

        sb.AppendLine("UPDATE tb_admin      ");
        sb.AppendLine("   SET hosp_no = NULL");
        sb.AppendLine(" WHERE aid = @AId;   ");

        #endregion

        var result = await db.ExecuteAsync(sb.ToString(), parameters);

        return result;
    }
}
