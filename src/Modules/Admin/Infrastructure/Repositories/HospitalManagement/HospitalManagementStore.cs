using Dapper;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper;
using System.Data;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results;
using Microsoft.Extensions.Logging;
using Mapster;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using System.Text;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Application.Features.Keywords.Results;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.HospitalManagement
{
    public class HospitalManagementStore : IHospitalManagementStore
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<HospitalManagementStore> _logger;

        public HospitalManagementStore(IDbConnectionFactory connectionFactory, ILogger<HospitalManagementStore> logger)
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

        public async Task<(List<GetHospitalResult>, int)> GetHospitalListAsync(string chartType, HospitalListSearchType searchType, string keyword, int pageNo, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting Hospital List by ChartType: {ChartType}, SearchType: {SearchType}, Keyword: {Keyword}, PageNo: {PageNo}, PageSize: {PageSize}", chartType, searchType, keyword, pageNo, pageSize);

                var parameters = new DynamicParameters();
                parameters.Add("@ChartType", chartType, DbType.String);
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
                           a.closing_yn            AS ClosingYn,
                           a.del_yn                AS DelYn,
                           a.descrption            AS Descrption,
                           a.reg_dt                AS RegDt,
                           a.chart_type            AS ChartType,
                           a.is_test               AS IsTest,
                           a.md_cd                 AS MdCd,
                           a.main_md_cd            AS MainMdCd,
                           a.kiosk_cnt             AS KioskCnt,
                           a.tablet_cnt            AS TabletCnt,
                           a.request_appr_yn       AS RequestApprYn
                      FROM ( SELECT b.hosp_key              AS hosp_key,
                                    a.hosp_no               AS hosp_no,
                                    a.business_no           AS business_no,
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
                              WHERE a.del_yn = 'N' ) a
                     WHERE a.{searchType.ToString()} LIKE CONCAT('%', @Keyword, '%')
                       AND a.chart_type LIKE @ChartType
                    ORDER BY a.reg_dt DESC
                    LIMIT @PageNo, @PageSize;
                ";

                using var connection = CreateConnection();

                var hospitalList = await connection.QueryAsync(sql, parameters);

                var result = hospitalList.Adapt<List<GetHospitalResult>>();
                int totalCount = hospitalList.Count() > 0 ? (int)hospitalList.First().TotalCount : 0;

                return (result, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Getting Hospital List by ChartType: {ChartType}, SearchType: {SearchType}, Keyword: {Keyword}, Offset: {PageNo}, Limit: {PageSize}", chartType, searchType, keyword, pageNo, pageSize);
                throw;
            }
        }

        public async Task<GetHospitalResult?> GetHospitalAsync(string hospNo, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting Hospital by HospNo: {HospNo}", hospNo);

                var parameters = new DynamicParameters();
                parameters.Add("@HospNo", hospNo, DbType.String);

                var sql = @"
                    SELECT a.hosp_key              AS HospKey,
                           a.hosp_no               AS HospNo,
                           a.business_no           AS BusinessNo,
                           a.business_level        AS BusinessLevel,
                           a.name                  AS Name,
                           a.hosp_cls_cd           AS HospClsCd,
                           a.addr                  AS Addr,
                           a.post_cd               AS PostCd,
                           a.tel                   AS Tel,
                           a.site                  AS Site,
                           a.lat                   AS Lat,
                           a.lng                   AS Lng,
                           a.closing_yn            AS ClosingYn,
                           a.del_yn                AS DelYn,
                           a.descrption            AS Descrption,
                           a.reg_dt                AS RegDt,
                           a.chart_type            AS ChartType,
                           a.is_test               AS IsTest,
                           a.md_cd                 AS MdCd,
                           a.main_md_cd            AS MainMdCd,
                           a.kiosk_cnt             AS KioskCnt,
                           a.tablet_cnt            AS TabletCnt,
                           a.request_appr_yn       AS RequestApprYn
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
                                AND a.del_yn = 'N' ) a
                ";

                using var connection = CreateConnection();

                return await connection.QueryFirstOrDefaultAsync<GetHospitalResult>(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Hospital by HospNo: {HospNo}", hospNo);
                throw;
            }
        }

        public async Task<List<MedicalTimeResultItem>> GetHospMedicalTimeListAsync(string hospKey, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting Hosp Medical Time by HospKey: {HospKey}", hospKey);

                var parameters = new DynamicParameters();
                parameters.Add("@HospKey", hospKey, DbType.String);

                var sql = @"
                    SELECT mt_id                                        AS MtId,
                           hosp_key                                     AS HospKey,
                           mt_wk                                        AS MtWk,
                           mt_nm                                        AS MtNm,
                           del_yn                                       AS DelYn,
                           DATE_FORMAT(FROM_UNIXTIME(reg_dt), '%Y%m%d') AS RegDt
                      FROM tb_eghis_hosp_medical_time
                     WHERE hosp_key = @HospKey
                       AND del_yn = 'N'
                    ORDER BY mt_id ASC;
                ";

                using var connection = CreateConnection();

                return (await connection.QueryAsync<MedicalTimeResultItem>(sql, parameters)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Hosp Medical Time by HospKey: {HospKey}", hospKey);
                throw;
            }
        }

        public async Task<List<HashTagInfoResultItem>> GetHospKeywordListAsync(string hospKey, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting Hosp Keyword by HospKey: {HospKey}", hospKey);

                var parameters = new DynamicParameters();
                parameters.Add("@HospKey", hospKey, DbType.String);

                var sql = @"
                    SELECT CONCAT(a.master_seq, '_', a.detail_seq) AS Kid,
                           a.master_seq                            AS MasterSeq,	
                           a.detail_seq                            AS DetailSeq,	
                           a.tag_nm                                AS TagNm,	
                           CASE a.detail_seq
	                         WHEN 0 THEN
	                           ( SELECT MAX(z.master_name)
	                               FROM hello100.tb_keyword_master z
	                              WHERE a.master_seq = z.master_seq ) 	
                             ELSE 	
                               ( SELECT MAX(z.detail_name)
                                   FROM hello100.tb_keyword_detail z
                                  WHERE a.detail_seq = z.detail_seq )	
                           END                                     AS Keyword,
                           a.del_yn                                AS DelYn
                      FROM tb_eghis_hosp_keyword_info a
                     WHERE a.hosp_key = @HospKey       
                       AND a.del_yn = 'N'              
                    ORDER BY a.tag_nm ASC;
                ";

                using var connection = CreateConnection();

                return (await connection.QueryAsync<HashTagInfoResultItem>(sql, parameters)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Hosp Keyword by HospKey: {HospKey}", hospKey);
                throw;
            }
        }

        public async Task<List<MedicalInfoResultItem>> GetHospitalMedicalListAsync(string hospKey, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting Hospital Medical by HospKey: {HospKey}", hospKey);

                var parameters = new DynamicParameters();
                parameters.Add("@HospKey", hospKey, DbType.String);

                var sql = @"
                    SELECT a.md_cd                                        AS MdCd,
                           a.hosp_key                                     AS HospKey,
                           b.cm_name                                      AS MdNm,
                           DATE_FORMAT(FROM_UNIXTIME(a.reg_dt), '%Y%m%d') AS RegDt
                      FROM tb_hospital_medical_info a
                     INNER JOIN tb_common b
                        ON (b.cls_cd = '03' AND b.cm_cd = a.md_cd)
                     WHERE a.hosp_key = @HospKey
                    ORDER BY a.reg_dt ASC, a.md_cd ASC;
                ";

                using var connection = CreateConnection();

                return (await connection.QueryAsync<MedicalInfoResultItem>(sql, parameters)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Hospital Medical by HospKey: {HospKey}", hospKey);
                throw;
            }
        }

        public async Task<List<ImageInfoResultItem>> GetImageListAsync(string hospKey, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting Image by HospKey: {HospKey}", hospKey);

                var parameters = new DynamicParameters();
                parameters.Add("@EncKey", "d3fa7fa7873c38097b31feb7bcd1c017ff222aee", DbType.String);
                parameters.Add("@HospKey", hospKey, DbType.String);

                var sql = @"
                    SELECT img_id                                        AS ImgId,
                           img_key                                       AS ImgKey,
                           url                                           AS Url,
                           del_yn                                        AS DelYn,
                           DATE_FORMAT(FROM_UNIXTIME(reg_dt), '%Y%m%d')  AS RegDt
                      FROM tb_image_info
                     WHERE img_key = func_HMACSHA256(@EncKey, CONCAT('hospital', @HospKey))
                       AND del_yn = 'N'
                    ORDER BY img_id ASC;
                ";

                using var connection = CreateConnection();

                return (await connection.QueryAsync<ImageInfoResultItem>(sql, parameters)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Image by HospKey: {HospKey}", hospKey);
                throw;
            }
        }

        public async Task<List<MedicalTimeNewResultItem>> GetHospMedicalTimeNewListAsync(string hospKey, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting Hosp Medical Time New by HospKey: {HospKey}", hospKey);

                var parameters = new DynamicParameters();
                parameters.Add("@HospKey", hospKey, DbType.String);

                var sql = @"
                    SELECT hosp_key          AS HospKey,
                           hosp_no           AS HospNo,
                           week_num          AS WeekNum,
                           CASE week_num
	                         WHEN 1 THEN '월요일'
                             WHEN 2 THEN '화요일'
                             WHEN 3 THEN '수요일'
                             WHEN 4 THEN '목요일'
                             WHEN 5 THEN '금요일'
                             WHEN 6 THEN '토요일'
                             WHEN 7 THEN '일요일'
                             WHEN 8 THEN '공휴일'
                             ELSE ''
                           END                AS WeekNumNm,
                           start_hour         AS StartHour,
                           start_minute       AS StartMinute,
                           end_hour           AS EndHour,
                           end_minute         AS EndMinute,
                           break_start_hour   AS BreakStartHour,
                           break_start_minute AS BreakStartMinute,
                           break_end_hour     AS BreakEndHour,
                           break_end_minute   AS BreakEndMinute,
                           use_yn             AS UseYn
                      FROM hello100.tb_eghis_hosp_medical_time_new
                     WHERE hosp_key = @HospKey;
                ";

                using var connection = CreateConnection();

                return (await connection.QueryAsync<MedicalTimeNewResultItem>(sql, parameters)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Hosp Medical Time New by HospKey: {HospKey}", hospKey);
                throw;
            }
        }

        public async Task<List<KeywordMasterResultItem>> GetKeywordMasterListAsync(string hospKey, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting Keyword Master by HospKey: {HospKey}", hospKey);

                var parameters = new DynamicParameters();
                parameters.Add("@EncKey", "d3fa7fa7873c38097b31feb7bcd1c017ff222aee", DbType.String);
                parameters.Add("@HospKey", hospKey, DbType.String);

                var sql = @"
                    SELECT master_name as MasterName,
                           master_seq as MasterSeq
                      FROM tb_keyword_master tkm
                     WHERE show_yn = 'Y'
                ";

                using var connection = CreateConnection();

                return (await connection.QueryAsync<KeywordMasterResultItem>(sql, parameters)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Keyword Master by HospKey: {HospKey}", hospKey);
                throw;
            }
        }

        public async Task<GetHello100SettingResult?> GetHello100SettingAsync(DbSession db, string hospKey, CancellationToken ct = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@HospKey", hospKey, DbType.String);

            var sql = @"
                SELECT a.st_id                                       AS StId,
                       a.hosp_key                                    AS HospKey,
                       a.wait_tm                                     AS WaitTm,
                       a.role                                        AS Role,
                       a.recept_end_time                             AS ReceptEndTime,
                       a.await_role                                  AS AwaitRole,
                       a.exampush_set                                AS ExamPushSet,
                       b.noti_id                                     AS NoticeId,
                       b.content                                     AS Content,
                       FROM_UNIXTIME(a.reg_dt, '%Y-%m-%d %H:%mi:%s') AS RegDt
                  FROM tb_eghis_hosp_settings_info a
                  LEFT JOIN tb_notice b
                         ON b.grade = '01' 
                        AND b.del_yn = 'N' 
                        AND b.hosp_key = a.hosp_key
                 WHERE a.hosp_key = @HospKey
                 ORDER BY a.reg_dt DESC, b.reg_dt DESC
                 LIMIT 1;
            ";

            var result = await db.QueryFirstOrDefaultAsync<GetHello100SettingResult>(sql, parameters, ct, _logger);

            return result;
        }

        public async Task<GetDeviceSettingResult<TabletRo>> GetHelloDeskSettingAsync(
            DbSession db, string hospNo, string hospKey, string? emplNo, int deviceType, CancellationToken ct = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@HospNo", hospNo, DbType.String);
            parameters.Add("@HospKey", hospKey, DbType.String);
            parameters.Add("@EmplNo", emplNo ?? string.Empty, DbType.String);
            parameters.Add("@DeviceType", deviceType, DbType.Int32); // SetDeviceType.Tablet: 2

            #region == Query ==
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" SELECT a.device_nm              AS DeviceNm          ");
            sb.AppendLine("	   ,   a.empl_no                AS EmplNo            ");
            sb.AppendLine("  FROM hello100.tb_eghis_hosp_device_settings_info a  ");
            sb.AppendLine(" WHERE a.hosp_no = @HospNo                            ");
            sb.AppendLine("   AND a.hosp_key = @HospKey                          ");
            sb.AppendLine("   AND a.device_type = @DeviceType                    ");
            sb.AppendLine("   AND a.use_yn = 'Y'                                 ");
            sb.AppendLine("  ORDER BY a.empl_no ASC   ;                          ");

            sb.AppendLine(" SELECT a.hosp_no              AS HospNo              ");
            sb.AppendLine("	   ,   a.empl_no              AS EmplNo              ");
            sb.AppendLine("	   ,   a.device_nm            AS DeviceNm            ");
            sb.AppendLine("	   ,   a.hosp_nm              AS HospNm              ");
            sb.AppendLine("	   ,   a.device_type          AS DeviceType          ");
            sb.AppendLine("	   ,   a.hosp_key             AS HospKey             ");
            sb.AppendLine("	   ,   a.info_txt             AS InfoTxt             ");
            sb.AppendLine("	   ,   a.set_json             AS SetJsonStr          ");
            sb.AppendLine("	   ,   a.use_yn               AS UseYn               ");
            sb.AppendLine("  FROM hello100.tb_eghis_hosp_device_settings_info a  ");
            sb.AppendLine(" WHERE a.hosp_no = @HospNo                            ");
            sb.AppendLine("   AND a.hosp_key = @HospKey                          ");
            sb.AppendLine("   AND a.device_type = @DeviceType                    ");
            sb.AppendLine("   AND a.use_yn = 'Y'                                 ");

            if (string.IsNullOrWhiteSpace(emplNo) == false)
            {
                sb.AppendLine("   AND a.empl_no = @EmplNo                        ");
            }
            sb.AppendLine("  ORDER BY a.empl_no ASC                              ");
            sb.AppendLine(" LIMIT 1;                                             ");
            #endregion

            var multi = await db.QueryMultipleAsync(sb.ToString(), parameters, ct, _logger);

            var result = new GetDeviceSettingResult<TabletRo>();

            result.EmplList = (await multi.ReadAsync<DeviceInfo>()).ToList();
            result.DeviceData = await multi.ReadSingleAsync<DeviceRo<TabletRo>>();

            return result;
        }

        public async Task<GetDeviceSettingResult<KioskRo>> GetKioskSettingAsync(
            DbSession db, string hospNo, string hospKey, string? emplNo, int deviceType, CancellationToken ct = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@HospNo", hospNo, DbType.String);
            parameters.Add("@HospKey", hospKey, DbType.String);
            parameters.Add("@EmplNo", emplNo ?? string.Empty, DbType.String);
            parameters.Add("@DeviceType", deviceType, DbType.Int32); // SetDeviceType.Tablet: 2

            #region == Query ==
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" SELECT a.device_nm              AS DeviceNm          ");
            sb.AppendLine("	   ,   a.empl_no                AS EmplNo            ");
            sb.AppendLine("  FROM hello100.tb_eghis_hosp_device_settings_info a  ");
            sb.AppendLine(" WHERE a.hosp_no = @HospNo                            ");
            sb.AppendLine("   AND a.hosp_key = @HospKey                          ");
            sb.AppendLine("   AND a.device_type = @DeviceType                    ");
            sb.AppendLine("   AND a.use_yn = 'Y'                                 ");
            sb.AppendLine("  ORDER BY a.empl_no ASC   ;                          ");

            sb.AppendLine(" SELECT a.hosp_no              AS HospNo              ");
            sb.AppendLine("	   ,   a.empl_no              AS EmplNo              ");
            sb.AppendLine("	   ,   a.device_nm            AS DeviceNm            ");
            sb.AppendLine("	   ,   a.hosp_nm              AS HospNm              ");
            sb.AppendLine("	   ,   a.device_type          AS DeviceType          ");
            sb.AppendLine("	   ,   a.hosp_key             AS HospKey             ");
            sb.AppendLine("	   ,   a.info_txt             AS InfoTxt             ");
            sb.AppendLine("	   ,   a.set_json             AS SetJsonStr          ");
            sb.AppendLine("	   ,   a.use_yn               AS UseYn               ");
            sb.AppendLine("  FROM hello100.tb_eghis_hosp_device_settings_info a  ");
            sb.AppendLine(" WHERE a.hosp_no = @HospNo                            ");
            sb.AppendLine("   AND a.hosp_key = @HospKey                          ");
            sb.AppendLine("   AND a.device_type = @DeviceType                    ");
            sb.AppendLine("   AND a.use_yn = 'Y'                                 ");

            if (string.IsNullOrWhiteSpace(emplNo) == false)
            {
                sb.AppendLine("   AND a.empl_no = @EmplNo                        ");
            }
            sb.AppendLine("  ORDER BY a.empl_no ASC                              ");
            sb.AppendLine(" LIMIT 1;                                             ");
            #endregion

            var multi = await db.QueryMultipleAsync(sb.ToString(), parameters, ct, _logger);

            var result = new GetDeviceSettingResult<KioskRo>();

            result.EmplList = (await multi.ReadAsync<DeviceInfo>()).ToList();
            result.DeviceData = await multi.ReadSingleAsync<DeviceRo<KioskRo>>();

            return result;
        }

        public async Task<ListResult<DoctorBaseRo>> GetDoctorsAsync(
            DbSession db, string hospNo, int pageNo, int pageSize, CancellationToken ct = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Limit", pageSize, DbType.Int32);
            parameters.Add("@OffSet", (pageNo - 1) * pageSize, DbType.Int32);
            parameters.Add("@HospNo", hospNo, DbType.String);

            #region == Query ==
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" SET @total_count:= (SELECT COUNT(DISTINCT empl_no)            ");
            sb.AppendLine("                       FROM hello100_api.eghis_doct_info       ");
            sb.AppendLine("                      WHERE hosp_no = @HospNo  and doct_no!=''                  ");
            sb.AppendLine("                        AND doct_nm LIKE CONCAT('%', '', '%'));");
            sb.AppendLine(" SET @rownum:= @total_count + 1 - @OffSet;                     ");

            sb.AppendLine(" SELECT @rownum:= @rownum - 1         AS rownum                ");
            sb.AppendLine("      , t.*                                                    ");
            sb.AppendLine("    FROM(                                                      ");
            sb.AppendLine(" SELECT  a.hosp_no                     AS HospNo               ");
            sb.AppendLine("   ,     a.hosp_key                    AS HospKey              ");
            sb.AppendLine("   ,     a.empl_no                     AS EmplNo               ");
            sb.AppendLine("   ,     a.doct_no                     AS DoctNo               ");
            sb.AppendLine("   ,     a.doct_nm                     AS DoctNm               ");
            sb.AppendLine("   ,     a.dept_cd                     AS DeptCd               ");
            sb.AppendLine("   ,     a.dept_nm                     AS DeptNm               ");
            sb.AppendLine("   ,     b.weeks_num                                           ");
            sb.AppendLine("   ,     case when b.otherCnt = 0 then b.weeks_nm              ");
            sb.AppendLine("         ELSE CONCAT(b.weeks_nm, ',지정', b.otherCnt, '건') END AS WeeksNm ");
            sb.AppendLine("   ,     a.front_view_role             AS FrontViewRole        ");
            sb.AppendLine("  FROM hello100_api.eghis_doct_info a                                       ");
            sb.AppendLine("  LEFT JOIN(SELECT empl_no, GROUP_CONCAT(z.week_num) AS weeks_num,  ");
            sb.AppendLine("        GROUP_CONCAT(CASE WHEN z.week_num = 1 THEN '월'        ");
            sb.AppendLine("                          WHEN z.week_num = 2 THEN '화'        ");
            sb.AppendLine("                          WHEN z.week_num = 3 THEN '수'        ");
            sb.AppendLine("                          WHEN z.week_num = 4 THEN '목'        ");
            sb.AppendLine("                          WHEN z.week_num = 5 THEN '금'        ");
            sb.AppendLine("                          WHEN z.week_num = 6 THEN '토'        ");
            sb.AppendLine("                          WHEN z.week_num = 7 THEN '일'        ");
            sb.AppendLine("                          WHEN z.week_num = 8 THEN '공휴일' END) AS weeks_nm ");
            sb.AppendLine("                          , SUM(case when z.week_num = 11 then 1 ELSE 0 END) AS otherCnt ");
            sb.AppendLine("               FROM hello100_api.eghis_doct_info z                         ");
            sb.AppendLine("               WHERE z.hosp_no = @HospNo   and doct_no!=''                     ");
            sb.AppendLine("                 AND z.use_yn= 'Y'                            ");
            sb.AppendLine("                 AND (ifnull(z.clinic_ymd,'') = '' OR z.clinic_ymd > Date_Format(NOW(), '%Y%m%d')) ");
            sb.AppendLine("               GROUP BY z.empl_no) b                          ");
            sb.AppendLine("      ON(b.empl_no = a.empl_no)                               ");
            sb.AppendLine("     WHERE a.hosp_no = @HospNo  and doct_no!=''                          ");
            sb.AppendLine(" GROUP BY  a.hosp_no                                          ");
            sb.AppendLine("   ,       a.hosp_key                                         ");
            sb.AppendLine("   ,       a.empl_no                                          ");
            sb.AppendLine("   ,       a.doct_no                                          ");
            sb.AppendLine("   ,       a.doct_nm                                          ");
            sb.AppendLine("   ,       a.dept_cd                                          ");
            sb.AppendLine("   ,       a.dept_nm                                          ");
            sb.AppendLine("   ,       a.front_view_role                                  ");
            sb.AppendLine(" ORDER BY CAST(a.empl_no AS SIGNED)) t;                       ");

            sb.AppendLine(" SELECT @total_count;                                         ");
            #endregion

            var multi = await db.QueryMultipleAsync(sb.ToString(), parameters, ct, _logger);

            var result = new ListResult<DoctorBaseRo>();

            result.Items = (await multi.ReadAsync<DoctorBaseRo>()).ToList();
            result.TotalCount = Convert.ToInt32(await multi.ReadSingleAsync<long>());

            return result;
        }
        public async Task<List<GetDoctorListResult>> GetDoctorList(string hospNo, CancellationToken cancellationToken = default)
        {
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

            return (await connection.QueryAsync<GetDoctorListResult>(sql, parameters)).ToList();
        }

        public async Task<List<GetDoctorScheduleResult>> GetDoctorList(string hospNo, string emplNo, CancellationToken cancellationToken = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@HospNo", hospNo, DbType.String);
            parameters.Add("@EmplNo", emplNo, DbType.String);

            var sql = $@"
                SELECT a.hosp_no                    AS HospNo,
                       a.hosp_key                   AS HospKey,
                       a.empl_no                    AS EmplNo,
                       IFNULL(a.clinic_ymd, '')     AS ClinicYmd,
                       ( SELECT CASE WHEN COUNT(1) > 0 THEN 'Y' ELSE 'N' END 
                           FROM hello100.tb_eghis_doct_untact_join
                          WHERE hosp_no = a.hosp_no
                            AND empl_no = a.empl_no
                            AND join_state != '03') AS UntactJoinYn,
                       ( SELECT CASE WHEN COUNT(1) > 0 THEN 'Y' ELSE 'N' END
                           FROM hello100.tb_eghis_doct_untact_join
                          WHERE hosp_no = a.hosp_no
                            AND empl_no = a.empl_no
                            AND join_state = '02')  AS DoctModifyYn,
                       a.doct_no                    AS DoctNo,
                       a.doct_nm                    AS DoctNm,
                       a.dept_cd                    AS DeptCd,
                       a.dept_nm                    AS DeptNm,
                       a.week_num                   AS WeekNum,
                       ( CASE WHEN a.week_num = 1 THEN '월요일'
                              WHEN a.week_num = 2 THEN '화요일'
                              WHEN a.week_num = 3 THEN '수요일'
                              WHEN a.week_num = 4 THEN '목요일'
                              WHEN a.week_num = 5 THEN '금요일'
                              WHEN a.week_num = 6 THEN '토요일'
                              WHEN a.week_num = 7 THEN '일요일'
                              WHEN a.week_num = 8 THEN '공휴일'
                         ELSE '예외' END )          AS WeeksNm,
                       ( SELECT COUNT(1)
                           FROM hello100_api.eghis_rsrv_info z
                           LEFT JOIN hello100_api.eghis_doct_info y
                             ON y.hosp_no = z.hospNo AND y.empl_no = z.doctEmplNo AND y.clinic_ymd = z.rsrvYmd
                          WHERE z.hospNo = a.hosp_no
                            AND z.doctEmplNo = a.empl_no
                          	AND z.rsrvYmd > date_format(NOW(), '%Y%m%d')
                            AND CASE WHEN IFNULL(y.clinic_ymd, '') = '' THEN WEEKDAY(z.rsrvYmd) + 1 = a.week_num ELSE z.rsrvYmd = a.clinic_ymd END
                            AND z.ptntState = 4 ) AS RsrvCnt,
                       ( SELECT COUNT(1)
                           FROM hello100.tb_eghis_hosp_receipt_info x
                          WHERE	hosp_no = a.hosp_no
                            AND recept_type = 'NR'
                            AND empl_no = a.empl_no
                            AND ptnt_state = '1'
                            AND empl_no = a.empl_no
                            AND WEEKDAY(req_date) + 1 = a.week_num ) AS UntactRsrvCnt,
                       a.start_hour                 AS StartHour,
                       a.start_minute               AS StartMinute,
                       a.end_hour                   AS EndHour,
                       a.end_minute                 AS EndMinute,
                       a.break_start_hour           AS BreakStartHour,
                       a.break_start_minute         AS BreakStartMinute,
                       a.break_end_hour             AS BreakEndHour,
                       a.break_end_minute           AS BreakEndMinute,
                       a.hello100_role              AS Hello100Role,
                       a.view_role                  AS ViewRole,
                       a.view_min_time              AS ViewMinTime,
                       a.view_min_cnt               AS ViewMinCnt,
                       a.interval_time              AS IntervalTime,
                       a.message                    AS Message,
                       a.use_yn                     AS UseYn,
                       IFNULL( ( SELECT z.ridx
                                   FROM hello100_api.eghis_doct_rsrv_info z                         
                                  WHERE z.hosp_no = a.hosp_no                                      
                                    AND z.empl_no = a.empl_no                                      
                                    AND z.week_num = a.week_num                                    
                                    AND IFNULL(z.clinic_ymd, '') = CASE WHEN a.week_num < 11 THEN '' ELSE a.clinic_ymd END LIMIT 1), 0) AS Ridx,
                       a.untact_start_hour         AS UntactStartHour,
                       a.untact_start_minute       AS UntactStartMinute,
                       a.untact_end_hour           AS UntactEndHour,
                       a.untact_end_minute         AS UntactEndMinute,
                       a.untact_interval_time      AS UntactIntervalTime,
                       a.untact_use_yn             AS UntactUseYn,
                       a.untact_break_start_hour   AS UntactBreakStartHour,
                       a.untact_break_start_minute AS UntactBreakStartMinute,
                       a.untact_break_end_hour     AS UntactBreakEndHour,
                       a.untact_break_end_minute   AS UntactBreakEndMinute,
                       ( SELECT z.file_path
                           FROM tb_file_info z
                         WHERE z.seq = b.doct_file_seq ) AS DoctFilePath
                 FROM hello100_api.eghis_doct_info a                                                              
                 LEFT JOIN hello100.tb_eghis_doct_info_file b
                   ON a.hosp_no = b.hosp_no AND a.hosp_key = b.hosp_key AND a.empl_no = b.empl_no 
                WHERE a.hosp_no = @HospNo                                                            
                  AND a.empl_no = @EmplNo                                                            
                  AND (IFNULL(a.clinic_ymd, '') = '' OR a.clinic_ymd >= DATE_FORMAT(NOW(), '%Y%m%d')) 
               ORDER BY a.clinic_ymd, a.week_num;
            ";

            using var connection = CreateConnection();

            return (await connection.QueryAsync<GetDoctorScheduleResult>(sql, parameters)).ToList();
        }

        public async Task<TbEghisDoctUntanctEntity?> GetDoctorUntanctInfo(string hospNo, string emplNo, CancellationToken cancellationToken = default)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("HospNo", hospNo, DbType.String);
            parameters.Add("EmplNo", emplNo, DbType.String);

            var sql = @"
                SELECT hosp_no                                                 AS HospNo,
                       hosp_key                                                AS HospKey,
                       hosp_nm                                                 AS HospNm,
                       empl_no                                                 AS EmplNo,
                       update_dt                                               AS UpdateDt,
                       hosp_tel                                                AS HospTel,
                       post_cd                                                 AS PostCd,
                       doct_no                                                 AS DoctNo,
                       doct_no_type                                            AS DoctNoType,
                       doct_license_file_seq                                   AS DoctLicenseFileSeq,
                       doct_nm                                                 AS DoctNm,
                       doct_birthday                                           AS DoctBirthday,
                       doct_tel                                                AS DoctTel,
                       doct_intro                                              AS DoctIntro,
                       doct_file_seq                                           AS DoctFileSeq,
                       doct_history                                            AS DoctHistory,
                       clinic_time                                             AS ClinicTime,
                       clinic_guide                                            AS ClinicGuide,
                       account_info_file_seq                                   AS AccountInfoFileSeq,
                       business_file_seq                                       AS BusinessFileSeq,
                       join_state                                              AS JoinState,
                       use_yn                                                  AS UseYn,
                       DATE_FORMAT(FROM_UNIXTIME(reg_dt), '%Y-%m-%d %H:%i:%s') AS RegDt
                  FROM hello100.tb_eghis_doct_untact
                 WHERE hosp_no = @HospNo
                   AND empl_no = @EmplNo
                ORDER BY update_dt DESC;
            ";

            using var connection = CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<TbEghisDoctUntanctEntity>(sql, parameters);
        }

        public async Task<EghisDoctRsrvInfoEntity?> GetEghisDoctRsrvInfo(string hospNo, string emplNo, int weekNum, string clinicYmd, CancellationToken cancellationToken = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@HospNo", hospNo, DbType.String);
            parameters.Add("@EmplNo", emplNo, DbType.String);
            parameters.Add("@WeekNum", weekNum, DbType.Int32);
            parameters.Add("@ClinicYmd", clinicYmd, DbType.String);

            var sql = $@"
                SELECT rIdx                                     AS Ridx,
                       hosp_no                                  AS HospNo,
                       empl_no                                  AS EmplNo,
                       week_num                                 AS WeekNum,
                       clinic_ymd                               AS ClinicYmd,
                       rsrv_interval_time                       AS RsrvIntervalTime,
                       rsrv_interval_cnt                        AS RsrvIntervalCnt,
                       DATE_FORMAT(reg_dt, '%Y-%m-%d %H:%i:%s') AS RegDt,
                       untact_rsrv_interval_time                AS UntactRsrvIntervalTime,
                       untact_rsrv_interval_cnt                 AS UntactRsrvIntervalCnt,
                       CASE
                         WHEN LENGTH(untact_ava_start_time) = 4 THEN
                           CONCAT(SUBSTRING(untact_ava_start_time, 1, 2), ':', SUBSTRING(untact_ava_start_time, 3, 2))
                         ELSE
                           ''
                       END                                      AS UntactAvaStartTime,
                       CASE
                         WHEN LENGTH(untact_ava_end_time) = 4 THEN
                           CONCAT(SUBSTRING(untact_ava_end_time, 1, 2), ':', SUBSTRING(untact_ava_end_time, 3, 2))
                         ELSE
                           ''
                       END                                      AS UntactAvaEndTime,
                       untact_ava_use_yn                        AS UntactAvaUseYn
                 FROM hello100_api.eghis_doct_rsrv_info
                WHERE hosp_no = @HospNo
                  AND empl_no = @EmplNo
                  AND week_num = @WeekNum
                  AND clinic_ymd = @ClinicYmd;
            ";

            using var connection = CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<EghisDoctRsrvInfoEntity>(sql, parameters);
        }

        public async Task<List<EghisDoctRsrvDetailInfoEntity>> GetEghisDoctRsrvDetailList(int ridx, string receptType, CancellationToken cancellationToken = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Ridx", ridx, DbType.Int32);
            parameters.Add("@ReceptType", receptType, DbType.String);

            var sql = $@"
                SELECT rsIdx                                    AS RsIdx,
                       ridx                                     AS Ridx,
                       start_time                               AS StartTime,
                       end_time                                 AS EndTime,
                       rsrv_cnt                                 AS RsrvCnt,
                       com_cnt                                  AS ComCnt,
                       DATE_FORMAT(reg_dt, '%Y-%m-%d %H:%i:%s') AS RegDt,
                       recept_type                              AS ReceptType
                  FROM hello100_api.eghis_doct_rsrv_detail_info
                 WHERE rIdx = @Ridx 
                   AND recept_type = @ReceptType;
            ";

            using var connection = CreateConnection();

            return (await connection.QueryAsync<EghisDoctRsrvDetailInfoEntity>(sql, parameters)).ToList();
        }

        public async Task<List<EghisRsrvInfoEntity>> GetEghisRsrvList(string hospNo, string emplNo, int weekNum, CancellationToken cancellationToken = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@HospNo", hospNo, DbType.String);
            parameters.Add("@EmplNo", emplNo, DbType.String);
            parameters.Add("@WeekNum", weekNum, DbType.Int32);

            var sql = $@"
                SELECT a.hospNo                                    AS HospNo,
                       a.rsrvReceptNo                              AS RsrvReceptNo,
                       a.receptNo                                  AS ReceptNo,
                       a.serialNo                                  AS SerialNo,
                       a.reqDate                                   AS ReqDate,
                       a.rsrvYmd                                   AS RsrvYmd,
                       a.rsrvTime                                  AS RsrvTime,
                       a.appUid                                    AS AppUid,
                       a.deptCd                                    AS DeptCd,
                       a.deptNm                                    AS DeptNm,
                       a.ptntNo                                    AS PtntNo,
                       a.doctEmplNo                                AS DoctEmplNo,
                       a.ptntState                                 AS PtntState,
                       a.waitSeq                                   AS WaitSeq,
                       a.ptntNm                                    AS PtntNm,
                       a.resultCd                                  AS ResultCd,
                       a.allergyList                               AS AllergyList,
                       DATE_FORMAT(a.regDate, '%Y-%m-%d %H:%i:%s') AS RegDate,
                       DATE_FORMAT(a.modDate, '%Y-%m-%d %H:%i:%s') AS ModDate,
                       a.transYn                                   AS TransYn,
                       a.message                                   AS Message
                 FROM hello100_api.eghis_rsrv_info a
                WHERE a.hospNo = @HospNo
                  AND a.doctEmplNo = @EmplNo
                  AND WEEKDAY(a.rsrvYmd) + 1 = @WeekNum
                  AND a.rsrvYmd > DATE_FORMAT(NOW(), '%Y%m%d')
                  AND a.rsrvYmd NOT IN ( SELECT b.clinic_ymd
                                           FROM hello100_api.eghis_doct_info b
                                          WHERE b.hosp_no = a.hospNo
                                            AND b.empl_no = a.doctEmplNo
                                            AND IFNULL(b.clinic_ymd, '') <> '' )
                  AND a.ptntState = 4
                ORDER BY a.rsrvYmd, a.rsrvTime;
            ";

            using var connection = CreateConnection();

            return (await connection.QueryAsync<EghisRsrvInfoEntity>(sql, parameters)).ToList();
        }

        public async Task<List<EghisRsrvInfoEntity>> GetEghisRsrvList(string hospNo, string emplNo, string clinicYmd, CancellationToken cancellationToken = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@HospNo", hospNo, DbType.String);
            parameters.Add("@EmplNo", emplNo, DbType.String);
            parameters.Add("@ClinicYmd", clinicYmd, DbType.String);

            var sql = $@"
                SELECT a.hospNo                                    AS HospNo,
                       a.rsrvReceptNo                              AS RsrvReceptNo,
                       a.receptNo                                  AS ReceptNo,
                       a.serialNo                                  AS SerialNo,
                       a.reqDate                                   AS ReqDate,
                       a.rsrvYmd                                   AS RsrvYmd,
                       a.rsrvTime                                  AS RsrvTime,
                       a.appUid                                    AS AppUid,
                       a.deptCd                                    AS DeptCd,
                       a.deptNm                                    AS DeptNm,
                       a.ptntNo                                    AS PtntNo,
                       a.doctEmplNo                                AS DoctEmplNo,
                       a.ptntState                                 AS PtntState,
                       a.waitSeq                                   AS WaitSeq,
                       a.ptntNm                                    AS PtntNm,
                       a.resultCd                                  AS ResultCd,
                       a.allergyList                               AS AllergyList,
                       DATE_FORMAT(a.regDate, '%Y-%m-%d %H:%i:%s') AS RegDate,
                       DATE_FORMAT(a.modDate, '%Y-%m-%d %H:%i:%s') AS ModDate,
                       a.transYn                                   AS TransYn,
                       a.message                                   AS Message
                 FROM hello100_api.eghis_rsrv_info a
                WHERE a.hospNo = @HospNo
                  AND a.doctEmplNo = @EmplNo
                  AND a.rsrvYmd = @ClinicYmd
                  AND a.rsrvYmd > DATE_FORMAT(NOW(), '%Y%m%d')
                  AND a.ptntState = 4
                ORDER BY a.rsrvYmd, a.rsrvTime;
            ";

            using var connection = CreateConnection();

            return (await connection.QueryAsync<EghisRsrvInfoEntity>(sql, parameters)).ToList();
        }

        public async Task<List<EghisRsrvInfoEntity>> GetEghisUntactRsrvList(string hospNo, string emplNo, int weekNum, CancellationToken cancellationToken = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@HospNo", hospNo, DbType.String);
            parameters.Add("@EmplNo", emplNo, DbType.String);
            parameters.Add("@WeekNum", weekNum, DbType.Int32);

            var sql = $@"
                SELECT a.hosp_no                                  AS HospNo,
                       NULL                                       AS RsrvReceptNo,
                       a.recept_no                                AS ReceptNo,
                       a.serial_no                                AS SerialNo,
                       a.req_date                                 AS ReqDate,
                       a.req_date                                 AS RsrvYmd,
                       a.req_time                                 AS RsrvTime,
                       CONCAT(a.uid, a.mid)                       AS AppUid,
                       NULL                                       AS DeptCd,
                       NULL                                       AS DeptNm,
                       a.ptnt_no                                  AS PtntNo,
                       a.doct_empl_no                             AS DoctEmplNo,
                       a.ptnt_state                               AS PtntState,
                       a.wait_seq                                 AS WaitSeq,
                       b.name                                     AS PtntNm,
                       a.result_cd                                AS ResultCd,
                       NULL                                       AS AllergyList,
                       DATE_FORMAT(a.reg_dt, '%Y-%m-%d %H:%i:%s') AS RegDate,
                       DATE_FORMAT(a.msd_dt, '%Y-%m-%d %H:%i:%s') AS ModDate,
                       2                                          AS TransYn,
                       a.msg                                      AS Message
                  FROM hello100.tb_eghis_hosp_receipt_info a
                  LEFT JOIN hello100.tb_member b
                    ON a.mid = b.mid
                 WHERE a.hosp_no = @HospNo
                   AND a.recept_type = 'NR'
                   AND a.empl_no = @EmplNo
                   AND a.ptnt_state = 1
                   AND a.req_date >= DATE_FORMAT(NOW(), '%Y%m%d')
                   AND WEEKDAY(a.req_date) + 1 = @WeekNum
                ORDER BY a.req_date, a.req_time;
            ";

            using var connection = CreateConnection();

            return (await connection.QueryAsync<EghisRsrvInfoEntity>(sql, parameters)).ToList();
        }

        public async Task<List<EghisDoctInfoMdEntity>> GetEghisDoctInfoMd(string hospNo, string emplNo, CancellationToken cancellationToken = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@HospNo", hospNo, DbType.String);
            parameters.Add("@EmplNo", emplNo, DbType.String);

            var sql = $@"
                SELECT a.hosp_no                                  AS HospNo,
                       a.hosp_key                                 AS HospKey,
                       a.empl_no                                  AS EmplNo,
                       a.md_cd                                    AS MdCd,
                       b.cm_name                                  AS MdNm,
                       DATE_FORMAT(a.reg_dt, '%Y-%m-%d %H:%i:%s') AS RegDt
                  FROM hello100_api.eghis_doct_info_md a
                  LEFT JOIN tb_common b
                    ON b.cls_cd = '03' AND b.del_yn = 'N' AND b.cm_cd = a.md_cd
                 WHERE a.hosp_no = @HospNo
                   AND a.empl_no = @EmplNo;
            ";

            using var connection = CreateConnection();

            return (await connection.QueryAsync<EghisDoctInfoMdEntity>(sql, parameters)).ToList();
        }
    }
}
