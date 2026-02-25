using Dapper;
using Hello100Admin.BuildingBlocks.Common.Errors;
using System.Data;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Common;
using Hello100Admin.Modules.Admin.Infrastructure.Persistence.DbModels.Common;
using Microsoft.Extensions.Logging;
using Mapster;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.Modules.Admin.Application.Common.ReadModels;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.Common
{
    public class HospitalInfoProvider : IHospitalInfoProvider
    {
        #region FIELD AREA ***************************************************
        private readonly ILogger<HospitalInfoProvider> _logger;
        private readonly IDbConnectionFactory _connection;
        #endregion

        #region CONSTRUCTOR AREA *********************************************
        public HospitalInfoProvider(ILogger<HospitalInfoProvider> logger,
                                    IDbConnectionFactory connection)
        {
            _logger = logger;
            _connection = connection;
        }
        #endregion

        #region GENERAL METHOD AREA **************************************
        public async Task<GetHospitalInfoReadModel?> GetHospitalInfoByHospNoAsync(string hospNo, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("GetHospitalInfoByHospNoAsync() Started");

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
                           z.descrption                                    AS Descrption,
                           z.md_cd                                         AS MdCd,
                           z.kiosk_cnt                                     AS KioskCnt,
                           z.tablet_cnt                                    AS TabletCnt,
                           z.chart_type                                    AS ChartType
                      FROM ( SELECT b.hosp_key              AS hosp_key,
                                    a.hosp_no               AS hosp_no,
                                    a.business_no           AS business_no,
                                    a.business_level        AS business_level,
                                    b.name                  AS name,
                                    b.hosp_cls_cd           AS hosp_cls_cd,
                                    b.addr                  AS addr,
                                    b.post_cd               AS post_cd,
                                    b.tel                   AS tel,
                                    b.site                  AS site,
                                    b.lat                   AS lat,
                                    b.lng                   AS lng,
                                    a.closing_yn            AS closing_yn,
                                    a.del_yn                AS del_yn,
                                    a.`desc`                AS descrption,
                                    FROM_UNIXTIME(a.reg_dt) AS reg_dt,
                                    a.chart_type            AS chart_type,
                                    b.is_test               AS is_test,
                                    ( SELECT GROUP_CONCAT(z.md_cd SEPARATOR ',')
                                        FROM tb_hospital_medical_info z 
                                       WHERE a.hosp_key = z.hosp_key ) AS md_cd,
                                    ( SELECT MAX(z.md_cd)
                                        FROM tb_hospital_medical_info z 
                                       WHERE main_yn = 'Y'
                                         AND a.hosp_key = z.hosp_key ) AS main_md_cd,
                                    ( SELECT COUNT(1)
                                        FROM tb_eghis_hosp_device_settings_info z
                                       WHERE a.hosp_no = z.hosp_no
                                         AND z.device_type = 1
                                         AND z.use_yn = 'Y' ) AS kiosk_cnt,
                                    ( SELECT COUNT(1)
                                        FROM tb_eghis_hosp_device_settings_info z
                                       WHERE a.hosp_no = z.hosp_no
                                         AND z.device_type = 2
                                         AND z.use_yn = 'Y' ) AS tablet_cnt,
                                    ( SELECT COUNT(1)
                                        FROM tb_eghis_hosp_approval_info z
                                       WHERE z.appr_type = 'HI'
                                         AND z.appr_yn = 'N'
                                         AND a.hosp_key = z.hosp_key) AS request_appr_yn
                               FROM tb_eghis_hosp_info a
                              INNER JOIN tb_hospital_info b
                                 ON a.hosp_key = b.hosp_key
                              WHERE a.hosp_no = @HospNo
                                AND a.del_yn = 'N' ) z
                ";

                using var connection = _connection.CreateConnection();
                var queryResult = await connection.QueryFirstOrDefaultAsync<CurrentHospitalInfo>(query, parameters);

                var result = queryResult.Adapt<GetHospitalInfoReadModel>();

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetHospitalInfoByHospNoAsync() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }

        // 위 쿼리와 동일하니 추후 병합
        public async Task<GetHospitalInfoReadModel?> GetHospitalInfoByHospKeyAsync(string hospKey, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("GetHospitalInfoByHospKeyAsync() Started");

                var parameters = new DynamicParameters();
                parameters.Add("@HospKey", hospKey, DbType.String);

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
                           z.descrption                                    AS Descrption,
                           z.md_cd                                         AS MdCd,
                           z.kiosk_cnt                                     AS KioskCnt,
                           z.tablet_cnt                                    AS TabletCnt,
                           z.chart_type                                    AS ChartType
                      FROM ( SELECT b.hosp_key              AS hosp_key,
                                    a.hosp_no               AS hosp_no,
                                    a.business_no           AS business_no,
                                    a.business_level        AS business_level,
                                    b.name                  AS name,
                                    b.hosp_cls_cd           AS hosp_cls_cd,
                                    b.addr                  AS addr,
                                    b.post_cd               AS post_cd,
                                    b.tel                   AS tel,
                                    b.site                  AS site,
                                    b.lat                   AS lat,
                                    b.lng                   AS lng,
                                    a.closing_yn            AS closing_yn,
                                    a.del_yn                AS del_yn,
                                    a.`desc`                AS descrption,
                                    FROM_UNIXTIME(a.reg_dt) AS reg_dt,
                                    a.chart_type            AS chart_type,
                                    b.is_test               AS is_test,
                                    ( SELECT GROUP_CONCAT(z.md_cd SEPARATOR ',')
                                        FROM tb_hospital_medical_info z 
                                       WHERE a.hosp_key = z.hosp_key ) AS md_cd,
                                    ( SELECT MAX(z.md_cd)
                                        FROM tb_hospital_medical_info z 
                                       WHERE main_yn = 'Y'
                                         AND a.hosp_key = z.hosp_key ) AS main_md_cd,
                                    ( SELECT COUNT(1)
                                        FROM tb_eghis_hosp_device_settings_info z
                                       WHERE a.hosp_no = z.hosp_no
                                         AND z.device_type = 1
                                         AND z.use_yn = 'Y' ) AS kiosk_cnt,
                                    ( SELECT COUNT(1)
                                        FROM tb_eghis_hosp_device_settings_info z
                                       WHERE a.hosp_no = z.hosp_no
                                         AND z.device_type = 2
                                         AND z.use_yn = 'Y' ) AS tablet_cnt,
                                    ( SELECT COUNT(1)
                                        FROM tb_eghis_hosp_approval_info z
                                       WHERE z.appr_type = 'HI'
                                         AND z.appr_yn = 'N'
                                         AND a.hosp_key = z.hosp_key) AS request_appr_yn
                               FROM tb_eghis_hosp_info a
                              INNER JOIN tb_hospital_info b
                                 ON a.hosp_key = b.hosp_key
                              WHERE a.hosp_key = @HospKey
                                AND a.del_yn = 'N' ) z
                ";

                using var connection = _connection.CreateConnection();
                var queryResult = await connection.QueryFirstOrDefaultAsync<CurrentHospitalInfo>(query, parameters);

                var result = queryResult.Adapt<GetHospitalInfoReadModel>();

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetHospitalInfoByHospKeyAsync() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }
        #endregion
    }
}
