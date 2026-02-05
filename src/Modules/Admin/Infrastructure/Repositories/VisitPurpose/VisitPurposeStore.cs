using Dapper;
using Hello100Admin.BuildingBlocks.Common.Errors;
using System.Data;
using Microsoft.Extensions.Logging;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.ReadModels.GetVisitPurposes;
using Mapster;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.Modules.Admin.Infrastructure.Persistence.DbModels.VisitPurpose;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.VisitPurpose;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Queries.GetVisitPurposeDetail;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.ReadModels.GetVisitPurposeDetail;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.ReadModels.GetCertificates;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Results;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.VisitPurpose
{
    public class VisitPurposeStore : IVisitPurposeStore
    {
        #region FIELD AREA ****************************************
        private readonly IDbConnectionFactory _connection;
        private readonly ILogger<VisitPurposeStore> _logger;
        #endregion

        #region CONSTRUCTOR AREA *******************************************
        public VisitPurposeStore(IDbConnectionFactory connection, ILogger<VisitPurposeStore> logger)
        {
            _connection = connection;
            _logger = logger;
        }
        #endregion

        #region IVISITPURPOSESTORE IMPLEMENTS METHOD AREA **************************************
        public async Task<GetVisitPurposesReadModel> GetVisitPurposesAsync(string hospKey, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("GetVisitPurposesAsync() Started");

                var parameters = new DynamicParameters();
                parameters.Add("HospKey", hospKey, DbType.String);
                //parameters.Add("HospKey", "12345", DbType.String);

                var query = @"
                    SELECT tehvpi.vp_cd           AS VpCd,
                           tehvpi.parent_cd       AS ParentCd,
                           tehvpi.hosp_key        AS HospKey,
                           tehvpi.name            AS Name,
                           tehvpi.show_yn         AS ShowYn,
                           tehvpi.sort_no         AS SortNo,
                           FROM_UNIXTIME(tehvpi.reg_dt, '%Y-%m-%d %H:%i') AS RegDt
                      FROM tb_eghis_hosp_visit_purpose_info tehvpi
                     INNER JOIN tb_eghis_hosp_info tehi
                             ON tehi.hosp_key = tehvpi.hosp_key
                     WHERE tehvpi.hosp_key = @HospKey
                       AND tehvpi.parent_cd = '0'
                       AND tehvpi.del_yn = 'N'
                       AND (tehi.chart_type <> 'N' OR tehvpi.vp_cd <> '01')
                     ORDER BY tehvpi.sort_no ASC, tehvpi.reg_dt DESC;

                    SELECT COUNT(tehvpi.vp_cd) AS TotalCount
                      FROM tb_eghis_hosp_visit_purpose_info tehvpi
                     INNER JOIN tb_eghis_hosp_info tehi 
                             ON tehi.hosp_key = tehvpi.hosp_key
                     WHERE tehvpi.hosp_key = @HospKey
                       AND tehvpi.parent_cd = '0'
                       AND tehvpi.del_yn = 'N'
                       AND (tehi.chart_type <> 'N' OR tehvpi.vp_cd <> '01');
                ";

                using var connection = _connection.CreateConnection();
                var multi = await connection.QueryMultipleAsync(query, parameters);

                var result = new GetVisitPurposesReadModel();

                var tempList = (await multi.ReadAsync<GetVisitPurposesRow>()).ToList();
                var tempTotalCount = await multi.ReadSingleAsync<long>();

                result.DetailList = tempList.Adapt<List<GetVisitPurposesItemReadModel>>();
                result.TotalCount = Convert.ToInt32(tempTotalCount);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetVisitPurposesAsync() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }

        public async Task<GetVisitPurposeDetailReadModel> GetVisitPurposeDetailAsync(GetVisitPurposeDetailQuery req, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("GetVisitPurposeDetailAsync() Started");

                var parameters = new DynamicParameters();
                parameters.Add("HospKey", req.HospKey, DbType.String);
                parameters.Add("HospNo", req.HospNo, DbType.String);
                parameters.Add("VpCd", req.VpCd, DbType.String);
                parameters.Add("DelYn", "N", DbType.String);

                string query = @"
                    SELECT a.vp_cd                                                      AS VpCd,
                           a.parent_cd                                                  AS ParentCd,
                           a.hosp_key                                                   AS HospKey,
                           a.inpuiry_url                                                AS InpuiryUrl,
                           a.inpuiry_idx                                                AS InpuiryIdx,
                           CASE WHEN IFNULL(a.inpuiry_skip_yn,'') = '' THEN 'N'
                                ELSE a.inpuiry_skip_yn END                              AS InpuirySkipYn,
                           a.name                                                       AS Name,
                           a.show_yn                                                    AS ShowYn,
                           a.role                                                       AS Role,
                           a.sort_no                                                    AS SortNo,
                           a.del_yn                                                     AS DelYn,
                           FROM_UNIXTIME(a.reg_dt, '%Y-%m-%d %H:%i')                    AS RegDt,
                           ( SELECT COUNT(*)
                               FROM tb_eghis_hosp_approval_info z
                              WHERE z.hosp_key = @HospKey
                                AND z.appr_type = 'HR'
                                AND z.appr_yn = 'N'
                                AND JSON_EXTRACT(z.data, '$.Purpose.VpCd')= @VpCd )     AS RequestApprYn,
                           ( SELECT COUNT(*) 
                                FROM tb_eghis_hosp_visit_purpose_info b
                               WHERE b.vp_cd != @VpCd 
                                 AND b.hosp_key = @HospKey
                                 AND b.parent_cd = '0' 
                                 AND b.del_yn='N' 
                                 AND b.show_yn = 'Y' )                                  AS CntPurpose,
                           ( SELECT chart_type 
                               FROM tb_eghis_hosp_info tehi 
                              WHERE hosp_key = a.hosp_key )                             AS ChartType
                      FROM tb_eghis_hosp_visit_purpose_info a
                     WHERE vp_cd  = @VpCd
                       AND hosp_key = @HospKey
                       AND del_yn  = @DelYn
                     ORDER BY sort_no ASC, reg_dt DESC;

                    SELECT a.vp_cd                                   AS VpCd,
                           a.parent_cd                               AS ParentCd,
                           a.hosp_key                                AS HospKey,
                           a.inpuiry_url                             AS InpuiryUrl,
                           a.inpuiry_idx                             AS InpuiryIdx,
                           a.inpuiry_skip_yn                         AS InpuirySkipYn,
                           a.name                                    AS Name,
                           a.show_yn                                 AS ShowYn,
                           a.role                                    AS Role,
                           a.sort_no                                 AS SortNo,
                           a.del_yn                                  AS DelYn,
                           FROM_UNIXTIME(a.reg_dt, '%Y-%m-%d %H:%i') AS RegDt
                      FROM tb_eghis_hosp_visit_purpose_info a
                     WHERE hosp_key = @HospKey
                       AND parent_cd  = @VpCd
                       AND del_yn  = @DelYn
                     ORDER BY sort_no ASC, reg_dt DESC;
                ";

                using var connection = _connection.CreateConnection();
                var multi = await connection.QueryMultipleAsync(query, parameters);

                var result = new GetVisitPurposeDetailReadModel();

                var parent = await multi.ReadFirstOrDefaultAsync<GetVisitPurposeDetailParentRow>();
                var child = (await multi.ReadAsync<GetVisitPurposeDetailChildRow>()).ToList();

                result.Purpose = parent.Adapt<GetVisitPurposeDetailParentItemReadModel>();
                result.Details = child.Adapt<List<GetVisitPurposeDetailChildItemReadModel>>();

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetVisitPurposeDetailAsync() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }

        public async Task<GetCertificatesReadModel> GetCertificatesAsync(string hospKey, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("GetVisitPurposesAsync() Started");

                var parameters = new DynamicParameters();
                parameters.Add("HospKey", hospKey, DbType.String);

                var query = @"
                    SELECT a.hosp_key	AS HospKey,
                           a.re_doc_cd  AS ReDocCd,
                           a.show_yn    AS ShowYn,
                           a.sort_no    AS SortNo,
                           b.cm_name    AS Name
                      FROM tb_eghis_recert_doc_info a
                     INNER JOIN tb_common b
                             ON b.cls_cd = '13'
                            AND b.cm_cd = a.re_doc_cd
                     WHERE hosp_key = @HospKey
                     ORDER BY sort_no ASC;

                    SELECT COUNT(re_doc_cd) AS TotalCount
                      FROM tb_eghis_recert_doc_info
                     WHERE hosp_key = @HospKey;
                ";

                using var connection = _connection.CreateConnection();
                var multi = await connection.QueryMultipleAsync(query, parameters);

                var result = new GetCertificatesReadModel();

                var tempList = (await multi.ReadAsync<GetCertificatesRow>()).ToList();
                var tempTotalCount = await multi.ReadSingleAsync<long>();

                result.List = tempList.Adapt<List<GetCertificatesItemReadModel>>();
                result.TotalCount = Convert.ToInt32(tempTotalCount);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetVisitPurposesAsync() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }

        public async Task<ListResult<GetQuestionnairesResult>> GetQuestionnairesAsync(DbSession db, string hospNo, CancellationToken ct)
        {

            var parameters = new DynamicParameters();
            parameters.Add("HospNo", hospNo, DbType.String);

            var query = @"
                SELECT intCd       AS IntCd,
                        intNm       AS IntNm,
                        category    AS Category
                    FROM hello100_api.eghis_paper_info
                WHERE hospNo = @HospNo
                    AND useYn = 1
                GROUP BY intCd, intNm, category;

                SELECT COUNT(DISTINCT intCd)
                    FROM hello100_api.eghis_paper_info
                    WHERE hospNo = @HospNo
                    AND useYn = 1
            ";

            var multi = await db.QueryMultipleAsync(query, parameters);

            var result = new ListResult<GetQuestionnairesResult>();

            result.Items = (await multi.ReadAsync<GetQuestionnairesResult>()).ToList();
            result.TotalCount = await multi.ReadSingleAsync<int>();

            return result;
        }
        #endregion
    }
}
