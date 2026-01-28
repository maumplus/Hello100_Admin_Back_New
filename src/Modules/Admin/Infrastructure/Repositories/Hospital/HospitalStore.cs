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

        public async Task<GetHospitalModel?> GetHospital(string hospNo, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting Hospital by HospNo: {HospNo}", hospNo);

                var parameters = new DynamicParameters();
                parameters.Add("@HospNo", hospNo, DbType.String);

                var sql = @"
                    SELECT b.hosp_key              AS HospKey,
                           a.hosp_no               AS HospNo,
                           a.business_no           AS BusinessNo,
                           b.name                  AS Name,
                           b.hosp_cls_cd           AS HospClsCd,
                           b.addr                  AS Addr,
                           b.post_cd               AS PostCd,
                           b.tel                   AS Tel,
                           b.site                  AS Site,
                           b.lat                   AS Lat,
                           b.lng                   AS Lng,
                           a.closing_yn            AS ClosingYn,
                           a.del_yn                AS DelYn,
                           a.`desc`                AS Descrption,
                           FROM_UNIXTIME(a.reg_dt) AS RegDt,
                           a.chart_type            AS ChartType,
                           b.is_test               AS IsTest,
                           ( SELECT GROUP_CONCAT(z.md_cd SEPARATOR ',')
                               FROM tb_hospital_medical_info z 
                              WHERE a.hosp_key = z.hosp_key ) AS MdCd,
                           ( SELECT MAX(z.md_cd)
                               FROM tb_hospital_medical_info z 
                              WHERE main_yn = 'Y'
                                AND a.hosp_key = z.hosp_key ) AS MainMdCd,
                           ( SELECT COUNT(1)
                               FROM tb_eghis_hosp_device_settings_info z
                              WHERE a.hosp_no = z.hosp_no
                                AND z.device_type = 1
                                AND z.use_yn = 'Y' ) AS KioskCnt,
                           ( SELECT COUNT(1)
                               FROM tb_eghis_hosp_device_settings_info z
                              WHERE a.hosp_no = z.hosp_no
                                AND z.device_type = 2
                                AND z.use_yn = 'Y' ) AS TabletCnt,
                           ( SELECT COUNT(1)
                               FROM tb_eghis_hosp_approval_info z
                              WHERE z.appr_type = 'HI'
                                AND z.appr_yn = 'N'
                                AND a.hosp_key = z.hosp_key) AS RequestApprYn
                      FROM tb_eghis_hosp_info a
                     INNER JOIN tb_hospital_info b
                        ON a.hosp_key = b.hosp_key
                     WHERE a.hosp_no = @HospNo
                       AND a.del_yn = 'N';
                ";

                using var connection = CreateConnection();

                return await connection.QueryFirstOrDefaultAsync<GetHospitalModel>(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Hospital by HospNo: {HospNo}", hospNo);
                throw;
            }
        }

        public async Task<List<GetHospMedicalTimeModel>> GetHospMedicalTimeList(string hospKey, CancellationToken cancellationToken = default)
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

                return (await connection.QueryAsync<GetHospMedicalTimeModel>(sql, parameters)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Hosp Medical Time by HospKey: {HospKey}", hospKey);
                throw;
            }
        }

        public async Task<List<GetHospKeywordModel>> GetHospKeywordList(string hospKey, CancellationToken cancellationToken = default)
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

                return (await connection.QueryAsync<GetHospKeywordModel>(sql, parameters)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Hosp Keyword by HospKey: {HospKey}", hospKey);
                throw;
            }
        }

        public async Task<List<GetHospitalMedicalModel>> GetHospitalMedicalList(string hospKey, CancellationToken cancellationToken = default)
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

                return (await connection.QueryAsync<GetHospitalMedicalModel>(sql, parameters)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Hospital Medical by HospKey: {HospKey}", hospKey);
                throw;
            }
        }

        public async Task<List<GetImageModel>> GetImageList(string hospKey, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting Image by HospKey: {HospKey}", hospKey);

                var parameters = new DynamicParameters();
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

                return (await connection.QueryAsync<GetImageModel>(sql, parameters)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Image by HospKey: {HospKey}", hospKey);
                throw;
            }
        }

        public async Task<List<GetHospMedicalTimeNewModel>> GetHospMedicalTimeNewList(string hospKey, CancellationToken cancellationToken = default)
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

                return (await connection.QueryAsync<GetHospMedicalTimeNewModel>(sql, parameters)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Hosp Medical Time New by HospKey: {HospKey}", hospKey);
                throw;
            }
        }

        public async Task<List<GetKeywordMasterModel>> GetKeywordMasterList(string hospKey, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting Keyword Master by HospKey: {HospKey}", hospKey);

                var parameters = new DynamicParameters();
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

                return (await connection.QueryAsync<GetKeywordMasterModel>(sql, parameters)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Keyword Master by HospKey: {HospKey}", hospKey);
                throw;
            }
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

                return (await connection.QueryAsync<GetDoctorListModel>(sql, parameters)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Doctor by HospNo: {HospNo}", hospNo);
                throw;
            }
        }
    }
}
