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

        public async Task<ListResult<GetMedicalDepartmentsResult>> GetMedicalDepartmentsAsync(DbSession db, string clsCd, CancellationToken ct = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ClsCode", clsCd, DbType.String);

            var sql = @"
                SELECT cm_seq                             AS CmSeq,
                       cls_cd                             AS ClsCode,
                       cm_cd                              AS CmCode,
                       cls_name                           AS ClsName,
                       cm_name                            AS CmName,
                       locale                             AS Locale,
                       del_yn                             AS DelYn,
                       sort                               AS Sort,
                       DATE_FORMAT(reg_dt, '%Y-%m-%d %T') AS RegDate
                  FROM tb_common 
                 WHERE cls_cd = @ClsCode
                   AND del_yn = 'N'
                 ORDER BY sort asc; 

                SELECT COUNT(*) AS TotalCount 
                  FROM tb_common 
                 WHERE cls_cd = @ClsCode 
                   AND del_yn = 'N'
            ";

            var multi = await db.QueryMultipleAsync(sql, parameters, ct, _logger);

            var result = new ListResult<GetMedicalDepartmentsResult>();

            result.Items = (await multi.ReadAsync<GetMedicalDepartmentsResult>()).ToList();
            result.TotalCount = Convert.ToInt32(await multi.ReadSingleAsync<long>());

            return result;
        }

        public async Task<List<GetClinicalKeywordsResult>> GetClinicalKeywordsAsync(DbSession db, string? keyword, string? masterSeq, CancellationToken ct = default)
        {
            // 파라미터 사용 안함
            //var parameters = new DynamicParameters();
            //parameters.Add("@Keyword", keyword, DbType.String);

            #region == Query ==
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("   select	");
            sb.AppendLine("      CONCAT(master_seq,'_',detail_seq) as Kid,   	");
            sb.AppendLine("      master_seq as MasterSeq,   	");
            sb.AppendLine("      detail_seq as DetailSeq,	");
            sb.AppendLine("      detail_name as Keyword,	");
            sb.AppendLine("      sort_no as SortNo,	");
            sb.AppendLine("      search_cnt as SearchCnt,	");
            sb.AppendLine("      'N' as DelYn	");
            sb.AppendLine("   from	");
            sb.AppendLine("      hello100.tb_keyword_detail tkd 	");
            sb.AppendLine("   where	 ");
            sb.AppendLine("   (select detail_use_yn from hello100.tb_keyword_master tkm where tkm.master_seq=tkd.master_seq)!='N' ");
            if (!string.IsNullOrEmpty(masterSeq))
            {
                sb.AppendLine("     and master_seq=" + masterSeq + " 	");
            }
            if (keyword != "")
            {
                sb.AppendLine("     and detail_name like '%" + keyword + "%'");
            }

            sb.AppendLine("     union	");
            sb.AppendLine("     select	");
            sb.AppendLine("        CONCAT(master_seq,'_',0) as Kid,     	");
            sb.AppendLine("        master_seq as MasterSeq,      	");
            sb.AppendLine("        0 as DetailSeq,  	");
            sb.AppendLine("        concat('[대표] ',master_name) as Keyword,      	");
            sb.AppendLine("        sort_no as SortNo,    	");
            sb.AppendLine("        search_cnt as SearchCnt,   	");
            sb.AppendLine("        'N' as DelYn	");
            sb.AppendLine("     from	");
            sb.AppendLine("        hello100.tb_keyword_master tkm 	");
            sb.AppendLine("   where	 show_yn='Y' ");
            if (!string.IsNullOrEmpty(masterSeq))
            {
                sb.AppendLine("     and master_seq=" + masterSeq + " 	");
            }
            if (keyword != "")
            {
                sb.AppendLine("     and master_name like '%" + keyword + "%'");
            }

            sb.AppendLine("        order by MasterSeq asc ,DetailSeq asc  	");
            #endregion

            var result = (await db.QueryAsync<GetClinicalKeywordsResult>(sb.ToString(), ct: ct, logger: _logger)).ToList();

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
                       CONCAT('/', ( SELECT file_path FROM tb_file_info tfi WHERE tfi.seq = b.doct_file_seq )) AS DoctFilePath
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
    }
}
