using Dapper;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper;
using System.Data;
using System.Text;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Hello100Admin.Modules.Admin.Application.Features.Hospitals.Results;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.Hospitals
{
    public class HospitalsStore : IHospitalsStore
    {
        #region FIELD AREA ****************************************
        private readonly ILogger<HospitalsStore> _logger;
        #endregion

        #region CONSTRUCTOR AREA *******************************************
        public HospitalsStore(ILogger<HospitalsStore> logger, IConfiguration config)
        {
            _logger = logger;
        }
        #endregion

        #region IHOSPITALSSTORE IMPLEMENTS AREA **********************************
        public async Task<ListResult<SearchHospitalsResult>> SearchHospitalsAsync(DbSession db, int pageNo, int pageSize, int searchType, string? searchKeyword, CancellationToken ct = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Keyword", searchKeyword, DbType.String);
            parameters.Add("@Limit", pageSize, DbType.String);
            parameters.Add("@OffSet", (pageNo - 1) * pageSize, DbType.String);

            string condi = string.Empty;
            if (string.IsNullOrWhiteSpace(searchKeyword) == false)
            {
                if (searchType == 0)
                    condi = " AND name LIKE CONCAT('%',@Keyword,'%')";
                else if (searchType == 1)
                    condi = " AND tel LIKE CONCAT('%',@Keyword,'%')";
            }

            #region == Query ==
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"SET @total_cnt:= (SELECT COUNT(*) FROM tb_hospital_info WHERE hosp_cls_cd <> '81' {condi} );");
            sb.AppendLine($"SET @rownum:= (@total_cnt +1) - @OffSet;");
            sb.AppendLine("  SELECT @rownum:= @rownum - 1 AS RowNum  ");
            sb.AppendLine("	     , 	hosp_key    AS HospKey   ");
            sb.AppendLine("	     , 	name        AS Name     ");
            sb.AppendLine("	     , 	addr        AS Addr     ");
            sb.AppendLine("	     , 	tel         AS Tel      ");
            sb.AppendLine("	   ,    from_unixtime(reg_dt, '%Y-%m-%d %T') AS RegDt");
            sb.AppendLine("	   FROM tb_hospital_info           ");
            sb.AppendLine($"WHERE hosp_cls_cd <> '81' ");
            sb.AppendLine(condi);
            sb.AppendLine("   ORDER BY reg_dt DESC   ");
            sb.AppendLine("   LIMIT @OffSet, @Limit;");

            sb.AppendLine($"SELECT @total_cnt;");
            #endregion

            var multi = await db.QueryMultipleAsync(sb.ToString(), parameters, ct, _logger);

            var result = new ListResult<SearchHospitalsResult>();

            result.Items = (await multi.ReadAsync<SearchHospitalsResult>()).ToList();
            result.TotalCount = await multi.ReadSingleAsync<int>();

            return result;
        }

        public async Task<GetHospitalDetailResult> GetHospitalDetailAsync(DbSession db, string hospKey, CancellationToken ct = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@HospKey", hospKey, DbType.String);

            #region == Query ==
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("  SELECT a.hosp_key         AS HospKey  ");
            //sb.AppendLine("     ,   a.hosp_no          AS HospNo   ");
            sb.AppendLine("     ,   a.hosp_cls_cd      AS HospClsCd");
            sb.AppendLine("     ,   a.name             AS Name     ");
            sb.AppendLine("     ,   a.addr             AS Addr     ");
            sb.AppendLine("     ,   a.post_cd          AS PostCd   ");
            sb.AppendLine("     ,   a.tel              AS Tel      ");
            sb.AppendLine("     ,   a.site             AS Site     ");
            sb.AppendLine("     ,   a.lat              AS Lat      ");
            sb.AppendLine("     ,   a.lng              AS Lng      ");
            sb.AppendLine("     ,   a.is_test          AS IsTest    ");
            sb.AppendLine("     ,   from_unixtime(a.reg_dt, '%Y-%m-%d %T') AS RegDt");
            sb.AppendLine("     ,   c.dept_cd         AS DeptCd");
            sb.AppendLine("     ,   c.dept_name       AS DeptName");
            sb.AppendLine("     ,   (select chart_type from tb_eghis_hosp_info where hosp_key = a.hosp_key) AS ChartType");
            sb.AppendLine("   FROM  tb_hospital_info a");
            sb.AppendLine("    LEFT JOIN (SELECT z.hosp_key  ");
            sb.AppendLine("             ,   IFNULL(GROUP_CONCAT(z.md_cd SEPARATOR ','), '') AS dept_cd ");
            sb.AppendLine("             ,   IFNULL(GROUP_CONCAT(x.cm_name separator ','), '') AS dept_name ");
            sb.AppendLine("            FROM tb_hospital_medical_info z ");
            sb.AppendLine("           INNER JOIN tb_common x ");
            sb.AppendLine("             ON (x.cls_cd = '03' AND x.del_yn = 'N' AND x.cm_cd = z.md_cd) ");
            sb.AppendLine("           WHERE z.hosp_key = @HospKey) c ");
            sb.AppendLine("     ON (c.hosp_key = a.hosp_key) ");
            sb.AppendLine("   WHERE a.hosp_key = @HospKey ");
            #endregion

            var result = await db.QueryFirstOrDefaultAsync<GetHospitalDetailResult>(sb.ToString(), parameters, ct, _logger);

            if (result == null)
                throw new BizException(AdminErrorCode.NotFoundHospital.ToError());

            return result;
        }

        public async Task<List<ExportHospitalsExcelResult>> ExportHospitalsExcelAsync(DbSession db, int searchType, string? searchKeyword, CancellationToken ct = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Keyword", searchKeyword, DbType.String);
            parameters.Add("@SearchType", searchType, DbType.Int32);

            string condi = string.Empty;
            if (string.IsNullOrWhiteSpace(searchKeyword) == false)
            {
                if (searchType == 0)
                    condi = " AND a.name LIKE CONCAT('%',@Keyword,'%')";
                else if (searchType == 1)
                    condi = " AND a.tel LIKE CONCAT('%',@Keyword,'%')";
            }

            #region == Query ==
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("  SELECT a.hosp_key                         AS HospKey      ");
            sb.AppendLine("     ,	a.name                             AS Name ");
            sb.AppendLine("     ,   a.hosp_cls_cd                      AS HospClsCd   ");
            sb.AppendLine("     ,   b.cm_name                          AS CmName ");
            sb.AppendLine("     ,	a.post_cd                          AS PostCd   ");
            sb.AppendLine("     ,   a.addr                             AS Addr       ");
            sb.AppendLine("     ,	a.tel                              AS Tel   ");
            sb.AppendLine("     ,	a.site                             AS Site    ");
            sb.AppendLine("     ,	FROM_UNIXTIME(a.reg_dt, '%Y%m%d')  AS RegDt   ");
            sb.AppendLine("     ,	a.lat                              AS Lat      ");
            sb.AppendLine("     ,	a.lng                              AS Lng      ");
            sb.AppendLine("	   FROM tb_hospital_info a                                 ");
            sb.AppendLine("    LEFT JOIN tb_common b                                   ");
            sb.AppendLine("    ON (b.cls_cd = '05' AND a.hosp_cls_cd = b.cm_cd)        ");
            sb.AppendLine($"WHERE 1 = 1 ");
            sb.AppendLine(condi);
            sb.AppendLine("   ORDER BY a.reg_dt DESC;  ");
            #endregion

            var result = (await db.QueryAsync<ExportHospitalsExcelResult>(sb.ToString(), parameters, ct, _logger)).ToList();

            return result;
        }
        #endregion
    }
}
