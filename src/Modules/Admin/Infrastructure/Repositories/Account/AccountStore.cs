using Dapper;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Account;
using Hello100Admin.Modules.Admin.Application.Features.Account.ReadModels;
using Mapster;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.Account
{
    public class AccountStore : IAccountStore
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<AccountStore> _logger;

        public AccountStore(IDbConnectionFactory connectionFactory, ILogger<AccountStore> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        private IDbConnection CreateConnection()
        {
            var conn = _connectionFactory.CreateConnection();
            if (conn == null)
                throw new InvalidOperationException("IDbConnectionFactory returned null connection.");
            return conn;
        }

        public async Task<(List<GetHospitalModel>, int)> GetHospitalList(AccountHospitalListSearchType searchType, string keyword, int pageNo, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting Hospital List by SearchType: {SearchType}, Keyword: {Keyword}, PageNo: {PageNo}, PageSize: {PageSize}", searchType, keyword, pageNo, pageSize);

                var parameters = new DynamicParameters();
                parameters.Add("@Keyword", keyword, DbType.String);
                parameters.Add("@PageNo", pageNo, DbType.String);
                parameters.Add("@PageSize", pageSize, DbType.String);

                var sql = @$"
                    SELECT COUNT(*) OVER()         AS TotalCount,
                           a.hosp_key              AS HospKey,
                           a.hosp_no               AS HospNo,
                           a.business_no           AS BusinessNo,
                           a.name                  AS Name,
                           a.hosp_cls_cd           AS HospClsCd,
                           a.addr                  AS Addr,
                           a.post_cd               AS PostCd,
                           a.tel                   AS Tel,
                           a.site                  AS Site,
                           a.lat                   AS Lat,
                           a.lng                   AS Lng,
                           a.reg_dt                AS RegDt
                      FROM ( SELECT b.hosp_key              AS hosp_key,
                                    IFNULL(a.hosp_no, '')   AS hosp_no,
                                    a.business_no           AS business_no,
                                    b.name                  AS name,
                                    b.hosp_cls_cd           AS hosp_cls_cd,
                                    b.addr                  AS addr,
                                    b.post_cd               AS post_cd,
                                    b.tel                   AS tel,
                                    b.site                  AS site,
                                    b.lat                   AS lat,
                                    b.lng                   AS lng,
                                    DATE_FORMAT(FROM_UNIXTIME(b.reg_dt), '%Y-%m-%d %H:%i:%s') AS reg_dt
                               FROM tb_hospital_info b
                               LEFT JOIN tb_eghis_hosp_info a
                                 ON a.hosp_key = b.hosp_key
                              WHERE IFNULL(a.hosp_no, '') = '' ) a
                     WHERE a.{searchType.ToString()} LIKE CONCAT('%', @Keyword, '%')
                    ORDER BY a.reg_dt DESC
                    LIMIT @PageNo, @PageSize;
                ";

                using var connection = CreateConnection();

                var hospitalList = await connection.QueryAsync(sql, parameters);

                var result = hospitalList.Adapt<List<GetHospitalModel>>();
                int totalCount = hospitalList.Count() > 0 ? (int)hospitalList.First().TotalCount : 0;

                return (result, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Getting Hospital List by SearchType: {SearchType}, Keyword: {Keyword}, Offset: {PageNo}, Limit: {PageSize}", searchType, keyword, pageNo, pageSize);
                throw;
            }
        }
    }
}
