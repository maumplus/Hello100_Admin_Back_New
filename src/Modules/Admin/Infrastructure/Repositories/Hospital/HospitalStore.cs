using Dapper;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Features.Hospital.ReadModels;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Infrastructure.Persistence.DbModels.Hospital;
using Mapster;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.Hospital
{
    /// <summary>
    /// Dapper 기반 HospitalRepository 구현체
    /// </summary>
    public class HospitalStore : IHospitalStore
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<HospitalStore> _logger;

        public HospitalStore(IDbConnectionFactory connectionFactory, ILogger<HospitalStore> logger)
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

        public async Task<List<GetDoctorListModel>> GetDoctorList(string hospNo, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting Doctor by HospNo: {HospNo}", hospNo);

                var parameters = new DynamicParameters();
                parameters.Add("@HospNo", hospNo, DbType.String);

                var sql = @"
                    SELECT a.hosp_no                     AS HospNo,
                           a.hosp_key                    AS HospKey,
                           a.empl_no                     AS EmplNo,
                           a.doct_no                     AS DoctNo,
                           a.doct_nm                     AS DoctNm,
                           a.dept_cd                     AS DeptCd,
                           a.dept_nm                     AS DeptNm,
                           CASE b.other_cnt
                             WHEN 0 THEN b.weeks_nm
                             ELSE CONCAT(b.weeks_nm, ',지정', b.other_cnt, '건')
                           END                           AS WeeksNm,
                           a.front_view_role             AS FrontViewRole
                      FROM hello100_api.eghis_doct_info a
                      LEFT JOIN ( SELECT t.empl_no                      AS empl_no,
                                         GROUP_CONCAT(t.week_num)       AS weeks_num,
                                         GROUP_CONCAT(
                                           CASE t.week_num
                                             WHEN 1 THEN '월'
                                             WHEN 2 THEN '화'
                                             WHEN 3 THEN '수'
                                             WHEN 4 THEN '목'
                                             WHEN 5 THEN '금'
                                             WHEN 6 THEN '토'
                                             WHEN 7 THEN '일'
                                             WHEN 8 THEN '공휴일'
                                           END
                                         )                              AS weeks_nm,
                                         SUM(IF(t.week_num = 11, 1, 0)) AS other_cnt
                                    FROM hello100_api.eghis_doct_info t
                                   WHERE t.hosp_no = @HospNo
                                     AND t.doct_no != ''
                                     AND t.use_yn = 'Y'
                                     AND (IFNULL(t.clinic_ymd, '') = '' OR t.clinic_ymd > DATE_FORMAT(NOW(), '%Y%m%d'))
                                  GROUP BY t.empl_no ) b
                        ON b.empl_no = a.empl_no
                     WHERE a.hosp_no = @HospNo
                       AND doct_no != ''
                    GROUP BY a.hosp_no,
                             a.hosp_key,
                             a.empl_no,
                             a.doct_no,
                             a.doct_nm,
                             a.dept_cd,
                             a.dept_nm,
                             a.front_view_role
                    ORDER BY CAST(a.empl_no AS SIGNED);
                ";

                using var connection = CreateConnection();
                var queryResult = (await connection.QueryAsync<GetDoctorListRow>(sql, parameters)).ToList();

                var result = queryResult.Adapt<List<GetDoctorListModel>>();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Doctor by HospNo: {HospNo}", hospNo);
                throw;
            }
        }
    }
}
