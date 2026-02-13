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
                           z.`Desc`                                        AS Descrption,
                           z.md_cd                                         AS MdCd,
                           ( SELECT COUNT(*) 
                               FROM tb_eghis_hosp_device_settings_info 
                              WHERE hosp_no = z.hosp_no 
                                AND device_type = 1 
                                AND use_yn = 'Y' )                         AS KioskCnt,
                           ( SELECT COUNT(*) 
                               FROM tb_eghis_hosp_device_settings_info 
                              WHERE hosp_no = z.hosp_no 
                                AND device_type = 2 
                                AND use_yn = 'Y' )                         AS TabletCnt,
                           z.chart_type                                    AS ChartType
                      FROM VM_HOSPITAL_DETAIL z
                     WHERE hosp_no = @HospNo
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
                           z.`Desc`                                        AS Descrption,
                           z.md_cd                                         AS MdCd,
                           ( SELECT COUNT(*) 
                               FROM tb_eghis_hosp_device_settings_info 
                              WHERE hosp_no = z.hosp_no 
                                AND device_type = 1 
                                AND use_yn = 'Y' )                         AS KioskCnt,
                           ( SELECT COUNT(*) 
                               FROM tb_eghis_hosp_device_settings_info 
                              WHERE hosp_no = z.hosp_no 
                                AND device_type = 2 
                                AND use_yn = 'Y' )                         AS TabletCnt,
                           z.chart_type                                    AS ChartType
                      FROM VM_HOSPITAL_DETAIL z
                     WHERE hosp_key = @HospKey
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
