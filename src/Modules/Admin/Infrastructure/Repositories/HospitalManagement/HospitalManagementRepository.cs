using Dapper;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using System.Data;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper;
using System.Text;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Application.Common.Definitions.Enums;
using DocumentFormat.OpenXml.Spreadsheet;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Org.BouncyCastle.Ocsp;
using Renci.SshNet;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.HospitalManagement
{
    public class HospitalManagementRepository : IHospitalManagementRepository
    {
        #region FIELD AREA ****************************************
        private readonly ILogger<HospitalManagementRepository> _logger;
        #endregion

        #region CONSTRUCTOR AREA *******************************************
        public HospitalManagementRepository(ILogger<HospitalManagementRepository> logger)
        {
            _logger = logger;
        }
        #endregion

        public async Task<int> UpsertHospitalAsync(
            DbSession db, string aId, string hospKey, string apprType, string reqJson, List<string> imageUrls, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("HospKey", hospKey, DbType.String);
            parameters.Add("ApprType", apprType, DbType.String);
            parameters.Add("ApprData", reqJson, DbType.String);
            parameters.Add("ReqAid", aId, DbType.String);
            parameters.Add("EncKey", "d3fa7fa7873c38097b31feb7bcd1c017ff222aee", DbType.String);

            StringBuilder sbImages = new StringBuilder();
            imageUrls.ForEach(x =>
            {
                sbImages.AppendLine($",(func_HMACSHA256( @EncKey, CONCAT('hospimsi', @HospKey)), '{x}', 'N', UNIX_TIMESTAMP(NOW())) ");
            });

            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(sbImages.ToString()))
            {
                sb.AppendLine("  DELETE FROM tb_image_info      ");
                sb.AppendLine("	  WHERE img_key = func_HMACSHA256( @EncKey, CONCAT('hospimsi', @HospKey))   ;  ");

                sb.AppendLine(" INSERT INTO tb_image_info	    ");
                sb.AppendLine(" 	(	img_key                 ");
                sb.AppendLine(" 	,	url                     ");
                sb.AppendLine(" 	,	del_yn	                ");
                sb.AppendLine(" 	,	reg_dt                  ");
                sb.AppendLine(" 	)                           ");
                sb.AppendLine("   VALUES                        ");
                sb.AppendLine(sbImages.ToString().Substring(1, sbImages.ToString().Length - 2) + ";");
            }

            sb.AppendLine(" INSERT INTO tb_eghis_hosp_approval_info	    ");
            sb.AppendLine(" 	(	appr_type               ");
            sb.AppendLine(" 	,	hosp_key                ");
            sb.AppendLine(" 	,	data                    ");
            sb.AppendLine(" 	,	req_aid                 ");
            sb.AppendLine(" 	,	reg_dt                  ");
            sb.AppendLine(" 	)                           ");
            sb.AppendLine("   VALUES                        ");
            sb.AppendLine(" 	(	@ApprType               ");
            sb.AppendLine(" 	,	@HospKey               ");
            sb.AppendLine(" 	,	@ApprData               ");
            sb.AppendLine(" 	,	@ReqAid                 ");
            sb.AppendLine(" 	,	UNIX_TIMESTAMP(NOW())   ");
            sb.AppendLine(" 	);                           ");

            sb.AppendLine(" SELECT IFNULL(LAST_INSERT_ID(), 0);");

            var result = await db.ExecuteScalarAsync<int>(sb.ToString(), parameters, ct, _logger);

            if (result <= 0)
                throw new BizException(AdminErrorCode.UpsertHospitalFailed.ToError());

            return result;
        }

        public async Task<int> UpsertAdmHospitalAsync
            (DbSession db, string aId, int apprId, string hospNo, string hospKey, string? description, string? businessNo, string? businessLevel, string? mainMdCd,
            List<TbEghisHospMedicalTimeEntity> clinicTimesEntity, List<TbEghisHospMedicalTimeNewEntity> clinicTimesNewEntity, 
            List<TbHospitalMedicalInfoEntity> deptCodesEntity, List<TbEghisHospKeywordInfoEntity> keywordsEntity, List<TbImageInfoEntity> imagesEntity, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("HospNo", hospNo, DbType.String);
            parameters.Add("HospKey", hospKey, DbType.String);
            parameters.Add("Descrption", description, DbType.String);
            parameters.Add("AId", aId, DbType.String);
            parameters.Add("EncKey", "d3fa7fa7873c38097b31feb7bcd1c017ff222aee", DbType.String);
            parameters.Add("ApprId", apprId, DbType.Int32);
            parameters.Add("BusinessNo", businessNo, DbType.String);
            parameters.Add("BusinessLevel", businessLevel, DbType.String);

            #region [요청사항으로 저장]
            StringBuilder sbTimes = new StringBuilder();
            StringBuilder sbKeywords = new StringBuilder();
            StringBuilder sbDeptCodes = new StringBuilder();
            StringBuilder sbImages = new StringBuilder();
            StringBuilder sbTimesNew = new StringBuilder();

            clinicTimesEntity.ForEach(x =>
            {
                sbTimes.AppendLine($",(@HospKey,'','{x.MtNm}', 'N', UNIX_TIMESTAMP(NOW())) ");
            });

            keywordsEntity.ForEach(x =>
            {
                sbKeywords.AppendLine($",({x.MasterSeq},{x.DetailSeq}, @HospKey,@HospNo,'{x.TagNm}', 'N', UNIX_TIMESTAMP(NOW())) ");
            });
            imagesEntity.ForEach(x =>
            {
                sbImages.AppendLine($",(func_HMACSHA256( @EncKey, CONCAT('hospital', @HospKey)), '{x.Url}', 'N', UNIX_TIMESTAMP(NOW())) ");
            });
            deptCodesEntity.ForEach(x =>
            {
                sbDeptCodes.AppendLine($",('{x.MdCd}', @HospKey, UNIX_TIMESTAMP(NOW()))");
            });

            #endregion

            #region == Query ==
            StringBuilder sb = new StringBuilder();
            #region [요청사항으로 저장]
            if (!string.IsNullOrEmpty(sbTimes.ToString()))
            {
                sb.AppendLine("  DELETE FROM tb_eghis_hosp_medical_time         ");
                sb.AppendLine("	  WHERE hosp_key = @HospKey   ;                 ");

                sb.AppendLine("  INSERT INTO tb_eghis_hosp_medical_time         ");
                sb.AppendLine("	     (hosp_key, mt_wk, mt_nm, del_yn, reg_dt)   ");
                sb.AppendLine("	 values                                         ");
                sb.AppendLine(sbTimes.ToString().Substring(1, sbTimes.ToString().Length - 2) + ";");
            }
            else
            {
                sb.AppendLine("  UPDATE tb_eghis_hosp_medical_time              ");
                sb.AppendLine("	    SET del_yn = 'Y'                            ");
                sb.AppendLine("	  WHERE hosp_key = @HospKey   ;                 ");
            }


            if (!string.IsNullOrEmpty(sbKeywords.ToString()))
            {
                sb.AppendLine("  DELETE FROM tb_eghis_hosp_keyword_info    ");
                sb.AppendLine("	  WHERE hosp_key = @HospKey   ;             ");

                sb.AppendLine("  INSERT INTO tb_eghis_hosp_keyword_info    ");
                sb.AppendLine("	     (master_seq, detail_seq, hosp_key, hosp_no, tag_nm, del_yn, reg_dt) ");
                sb.AppendLine("	 values                                     ");
                sb.AppendLine(sbKeywords.ToString().Substring(1, sbKeywords.ToString().Length - 2) + ";");
            }
            else
            {
                sb.AppendLine("  UPDATE tb_eghis_hosp_keyword_info         ");
                sb.AppendLine("	    SET del_yn = 'Y'                        ");
                sb.AppendLine("	  WHERE hosp_key = @HospKey   ;             ");
            }

            if (!string.IsNullOrEmpty(sbDeptCodes.ToString()))
            {
                sb.AppendLine("  DELETE FROM tb_hospital_medical_info       ");
                sb.AppendLine("	  WHERE hosp_key = @HospKey   ;             ");

                sb.AppendLine("  INSERT INTO tb_hospital_medical_info    ");
                sb.AppendLine("	     (md_cd, hosp_key, reg_dt)");
                sb.AppendLine("	 values                                     ");
                sb.AppendLine(sbDeptCodes.ToString().Substring(1, sbDeptCodes.ToString().Length - 2) + ";");
            }

            if (!string.IsNullOrEmpty(sbImages.ToString()))
            {
                sb.AppendLine("  DELETE FROM tb_image_info      ");
                sb.AppendLine("	  WHERE img_key = func_HMACSHA256( @EncKey, CONCAT('hospital', @HospKey))   ;  ");

                sb.AppendLine(" INSERT INTO tb_image_info	    ");
                sb.AppendLine(" 	(	img_key                 ");
                sb.AppendLine(" 	,	url                     ");
                sb.AppendLine(" 	,	del_yn	                ");
                sb.AppendLine(" 	,	reg_dt                  ");
                sb.AppendLine(" 	)                           ");
                sb.AppendLine("   VALUES                        ");
                sb.AppendLine(sbImages.ToString().Substring(1, sbImages.ToString().Length - 2) + ";");
            }
            else
            {
                sb.AppendLine("  UPDATE tb_image_info                       ");
                sb.AppendLine("	    SET del_yn = 'Y'                        ");
                sb.AppendLine("	  WHERE img_key = func_HMACSHA256( @EncKey, CONCAT('hospital', @HospKey))   ;  ");
            }

            sb.AppendLine("	 UPDATE tb_eghis_hosp_info          ");
            sb.AppendLine("	    SET `desc` = @Descrption        ");
            sb.AppendLine("	    ,   business_no = @BusinessNo   ");
            sb.AppendLine("	    ,   business_level = @BusinessLevel   ");
            sb.AppendLine("	   WHERE hosp_no = @HospNo;         ");
            #endregion

            sb.AppendLine("  UPDATE tb_eghis_hosp_approval_info	    ");
            sb.AppendLine(" 	SET	aid     = @AId                  ");
            sb.AppendLine(" 	,	appr_yn = 'Y'                   ");
            sb.AppendLine(" 	,	appr_dt = UNIX_TIMESTAMP(NOW()) ");
            sb.AppendLine("   WHERE appr_id = @ApprId;              ");


            if (clinicTimesNewEntity != null)
            {
                clinicTimesNewEntity.ForEach(x =>
                {
                    sb.AppendLine(" DELETE FROM  hello100.tb_eghis_hosp_medical_time_new where hosp_no = @HospNo and week_num = " + x.WeekNum + "; ");
                    sb.AppendLine(" INSERT INTO hello100.tb_eghis_hosp_medical_time_new  ");
                    sb.AppendLine(" (hosp_key, hosp_no, week_num, start_hour, start_minute, end_hour, end_minute, break_start_hour, break_start_minute, break_end_hour, break_end_minute, use_yn, reg_dt)  ");
                    sb.AppendLine(" values ('" + x.HospKey + "', '" + x.HospNo + "', " + x.WeekNum + ", " + x.StartHour + ", " + x.StartMinute + ", " + x.EndHour + ", " + x.EndMinute + ", " + x.BreakStartHour + ", " + x.BreakStartMinute + ", " + x.BreakEndHour + "," + x.BreakEndMinute + ", '" + x.UseYn + "',UNIX_TIMESTAMP(NOW()));  ");
                });
            }

            //대표 진료과 설정
            sb.AppendLine("  UPDATE tb_hospital_medical_info	    ");
            sb.AppendLine(" 	SET	main_yn     = 'N'                  ");
            sb.AppendLine("   WHERE hosp_key = '" + hospKey + "';              ");

            sb.AppendLine("  UPDATE tb_hospital_medical_info	    ");
            sb.AppendLine(" 	SET	main_yn     = 'Y'                  ");
            sb.AppendLine("   WHERE hosp_key = '" + hospKey + "' and md_cd='" + mainMdCd + "';              ");

            #endregion

            var result = await db.ExecuteAsync(sb.ToString(), parameters, ct, _logger);

            if (result <= 0)
                throw new BizException(AdminErrorCode.UpsertHospitalFailed.ToError());

            return result;
        }

        public async Task<int> UpsertHello100SettingAsync(DbSession db, string hospNo, TbEghisHospSettingsInfoEntity settingEntity, TbNoticeEntity noticeEntity, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("HospNo", hospNo, DbType.String);
            parameters.Add("HospKey", settingEntity.HospKey, DbType.String);
            parameters.Add("Role", settingEntity.Role, DbType.Int32);
            parameters.Add("ReceptEndTime", settingEntity.ReceptEndTime, DbType.String);
            parameters.Add("AwaitRole", settingEntity.AwaitRole, DbType.Int32);
            parameters.Add("Notice", noticeEntity.Content ?? string.Empty, DbType.String);
            parameters.Add("NoticeId", noticeEntity.NotiId, DbType.Int32);
            parameters.Add("StId", settingEntity.StId, DbType.Int32);
            parameters.Add("ExamPushSet", settingEntity.ExamPushSet, DbType.Int32);

            #region == Query ==
            StringBuilder sb = new StringBuilder();

            if (settingEntity.StId == 0)
            {
                // 모바일 접수 셋팅
                sb.AppendLine("  INSERT INTO tb_eghis_hosp_settings_info ");
                sb.AppendLine("     (   hosp_key                ");
                sb.AppendLine("     ,   wait_tm                 ");
                sb.AppendLine("     ,   role                    ");
                sb.AppendLine("     ,   recept_end_time         ");
                sb.AppendLine("     ,   await_role              ");
                sb.AppendLine("     ,   exampush_set              ");
                sb.AppendLine("     ,   reg_dt                  ");
                sb.AppendLine("     )                           ");
                sb.AppendLine(" VALUES                          ");
                sb.AppendLine("     (   @HospKey                ");
                sb.AppendLine("     ,   ''                      ");
                sb.AppendLine("     ,   @Role                   ");
                sb.AppendLine("     ,   @ReceptEndTime          ");
                sb.AppendLine("     ,   @AwaitRole              ");
                sb.AppendLine("     ,   @ExamPushSet              ");
                sb.AppendLine("     ,   UNIX_TIMESTAMP(now())   ");
                sb.AppendLine("     );                          ");
            }
            else
            {
                sb.AppendLine("  UPDATE tb_eghis_hosp_settings_info ");
                sb.AppendLine("     SET wait_tm = wait_tm");
                sb.AppendLine("     ,   role = @Role");
                if (settingEntity.Role == 2 || (settingEntity.Role & (int)Hello100RoleType.NoRecept) == 0)
                {
                    sb.AppendLine("     ,   recept_end_time = ''");
                }
                else
                {
                    sb.AppendLine("     ,   recept_end_time = @ReceptEndTime");
                }
                sb.AppendLine("     ,   await_role = @AwaitRole");
                sb.AppendLine("     ,   exampush_set = @ExamPushSet");
                sb.AppendLine("     ,   reg_dt = UNIX_TIMESTAMP(now())");
                sb.AppendLine("   WHERE st_id = @StId;");
            }

            // 공지사항 저장
            if (noticeEntity.NotiId == 0)
            {
                sb.AppendLine("  INSERT INTO tb_notice          ");
                sb.AppendLine("     (   title                   ");
                sb.AppendLine("     ,   content                 ");
                sb.AppendLine("     ,   grade                   ");
                sb.AppendLine("     ,   hosp_key                ");
                sb.AppendLine("     ,   reg_dt                  ");
                sb.AppendLine("     )                           ");
                sb.AppendLine(" VALUES                          ");
                sb.AppendLine("     (   ''                      ");

                if (noticeEntity.Content == null)
                {
                    sb.AppendLine("     ,    ''                 ");
                }

                else
                {
                    sb.AppendLine("     ,    @Notice            ");
                }
                sb.AppendLine("     ,   '01'                    ");
                sb.AppendLine("     ,   @HospKey                ");
                sb.AppendLine("     ,   UNIX_TIMESTAMP(now())   ");
                sb.AppendLine("     );                          ");
            }
            else
            {
                sb.AppendLine(" UPDATE tb_notice            ");
                sb.AppendLine("     SET content = @Notice   ");
                sb.AppendLine("   WHERE noti_id = @NoticeId;");
            }

            #endregion

            var result = await db.ExecuteAsync(sb.ToString(), parameters, ct, _logger);

            if (result <= 0)
                throw new BizException(AdminErrorCode.UpsertHello100SettingFailed.ToError());

            return result;
        }

        public async Task<int> UpsertDeviceSettingAsync(
            DbSession db, string hospNo, string hospNm, string emplNo, string deviceNm, int deviceType, string hospKey, string infoTxt, string useYn, string? setJson, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("HospNo", hospNo, DbType.String);
            parameters.Add("EmplNo", emplNo, DbType.String);
            parameters.Add("DeviceNm", deviceNm, DbType.String);
            parameters.Add("HospNm", hospNm, DbType.String);
            parameters.Add("DeviceType", deviceType, DbType.Int32);
            parameters.Add("HospKey", hospKey, DbType.String);
            parameters.Add("InfoTxt", infoTxt, DbType.String);
            parameters.Add("SetJson", setJson, DbType.String);
            parameters.Add("UseYn", useYn, DbType.String);

            var query = @"
                INSERT INTO hello100.tb_eghis_hosp_device_settings_info
                            ( hosp_no, empl_no, device_nm, hosp_nm, device_type, hosp_key, info_txt, set_json, use_yn, reg_dt )
                     VALUES
                            ( @HospNo, @EmplNo, @DeviceNm, @HospNm, @DeviceType, @HospKey, @InfoTxt, @SetJson, @UseYn, NOW() )
                         ON DUPLICATE KEY
                     UPDATE hosp_nm = @HospNm,
                            hosp_key = @HospKey,
                            info_txt = @InfoTxt,
                            set_json = @SetJson,
                            use_yn = @UseYn
            ";

            var result = await db.ExecuteAsync(query, parameters, ct, _logger);

            if (result <= 0)
                throw new BizException(AdminErrorCode.UpsertDeviceSettingFailed.ToError());

            return result;
        }

        public async Task<int> UpdateDoctorListAsync(DbSession db, List<EghisDoctInfoEntity> eghisDoctInfoList, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();

            var query = string.Empty;
            var queries = new List<string>();

            for (int i = 0; i< eghisDoctInfoList.Count; i++)
            {
                var eghisDoctInfo = eghisDoctInfoList[i];

                parameters.Add($"HospNo{i}", eghisDoctInfo.HospNo, DbType.String);
                parameters.Add($"EmplNo{i}", eghisDoctInfo.EmplNo, DbType.String);
                parameters.Add($"FrontViewRole{i}", eghisDoctInfo.FrontViewRole, DbType.Int32);

                query = $@"
                    UPDATE hello100_api.eghis_doct_info
                       SET front_view_role = @FrontViewRole{i}
                     WHERE hosp_no  = @HospNo{i}
                       AND empl_no  = @EmplNo{i};
                ";

                queries.Add(query);
            }

            query = string.Join("", queries.ToArray());

            return await db.ExecuteAsync(query, parameters, ct, _logger);
        }

        public async Task<int> UpdateDoctorInfoAsync(DbSession db, EghisDoctInfoEntity eghisDoctInfo, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("HospNo", eghisDoctInfo.HospNo, DbType.String);
            parameters.Add("HospKey", eghisDoctInfo.HospKey, DbType.String);
            parameters.Add("EmplNo", eghisDoctInfo.EmplNo, DbType.String);
            parameters.Add("DoctNm", eghisDoctInfo.DoctNm, DbType.String);
            parameters.Add("ViewMinTime", eghisDoctInfo.ViewMinTime, DbType.Int32);
            parameters.Add("ViewMinCnt", eghisDoctInfo.ViewMinCnt, DbType.Int32);

            var query = @"
                UPDATE hello100_api.eghis_doct_info
                   SET doct_nm       = @DoctNm,
                       view_min_time = @ViewMinTime,
                       view_min_cnt  = @ViewMinCnt
                 WHERE hosp_no  = @HospNo
                   AND hosp_key = @HospKey
                   AND empl_no  = @EmplNo;
            ";

            return await db.ExecuteAsync(query, parameters, ct, _logger);
        }

        public async Task<int> UpdateDoctorInfoFileAsync(DbSession db, TbEghisDoctInfoFileEntity eghisDoctInfoFile, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("HospNo", eghisDoctInfoFile.HospNo, DbType.String);
            parameters.Add("HospKey", eghisDoctInfoFile.HospKey, DbType.String);
            parameters.Add("EmplNo", eghisDoctInfoFile.EmplNo, DbType.String);
            parameters.Add("ClsCd", eghisDoctInfoFile.TbFileInfoEntity.ClsCd, DbType.String);
            parameters.Add("CmCd", eghisDoctInfoFile.TbFileInfoEntity.CmCd, DbType.String);
            parameters.Add("FilePath", eghisDoctInfoFile.TbFileInfoEntity.FilePath, DbType.String);
            parameters.Add("OriginFileName", eghisDoctInfoFile.TbFileInfoEntity.OriginFileName, DbType.String);
            parameters.Add("FileSize", eghisDoctInfoFile.TbFileInfoEntity.FileSize, DbType.Int32);
            parameters.Add("DelYn", eghisDoctInfoFile.TbFileInfoEntity.DelYn, DbType.String);

            var query = @"
                SET @maxFileSeq4 := ( SELECT IFNULL(MAX(seq), 0) + 1
                                        FROM tb_file_info );

                INSERT INTO tb_file_info
                  (seq, ord_seq, cls_cd, cm_cd, file_path, origin_file_name, file_size, del_yn, del_dt, reg_dt)
                VALUES
                  (@maxFileSeq4, ( SELECT IFNULL(MAX(tfi.ord_seq), 0) + 1 FROM tb_file_info tfi WHERE tfi.seq = @maxFileSeq4 ), @ClsCd, @CmCd, @FilePath, @OriginFileName, @FileSize, @DelYn, NULL, UNIX_TIMESTAMP(NOW()));

                UPDATE tb_eghis_doct_untact
                   SET doct_file_seq  = @maxFileSeq4
                 WHERE hosp_no = @HospNo
                   AND hosp_key = @HospKey
                   AND empl_no = @EmplNo;

                DELETE FROM hello100.tb_eghis_doct_info_file
                      WHERE hosp_no = @HospNo
                        AND empl_no = @EmplNo;

                INSERT INTO tb_eghis_doct_info_file
                  (hosp_no, hosp_key, empl_no, doct_file_seq, reg_dt)
                VALUES
                  (@HospNo, @HospKey, @EmplNo, @maxFileSeq4, UNIX_TIMESTAMP(NOW()));
            ";

            return await db.ExecuteAsync(query, parameters, ct, _logger);
        }

        public async Task<int> UpdateDoctorUntactAsync(DbSession db, TbEghisDoctUntactEntity eghisDoctUntact, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("HospNo", eghisDoctUntact.HospNo, DbType.String);
            parameters.Add("EmplNo", eghisDoctUntact.EmplNo, DbType.String);
            parameters.Add("DoctIntro", eghisDoctUntact.DoctIntro, DbType.String);
            parameters.Add("ClinicGuide", eghisDoctUntact.ClinicGuide, DbType.String);
            parameters.Add("DoctHistory", eghisDoctUntact.DoctHistory, DbType.String);

            var query = @"
                UPDATE hello100.tb_eghis_doct_untact
                   SET doct_intro   = @DoctIntro,
                       clinic_guide = @ClinicGuide,
                       doct_history = @DoctHistory
                 WHERE hosp_no = @HospNo
                   AND empl_no = @EmplNo;
            ";

            return await db.ExecuteAsync(query, parameters, ct, _logger);
        }

        public async Task<int> RemoveEghisDoctRsrvAsync(DbSession db, int ridx, string receptType, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Ridx", ridx, DbType.Int32);
            parameters.Add("ReceptType", receptType, DbType.String);

            var query = $@"
                DELETE FROM hello100_api.eghis_doct_rsrv_detail_info
                      WHERE ridx = @Ridx
                        AND recept_type = @ReceptType;
            ";

            return await db.ExecuteAsync(query, parameters, ct, _logger);
        }
        
        public async Task<int> RemoveEghisDoctRsrvAsync(DbSession db, EghisDoctRsrvInfoEntity eghisDoctRsrvInfoEntity, string receptType, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("HospNo", eghisDoctRsrvInfoEntity.HospNo, DbType.String);
            parameters.Add("EmplNo", eghisDoctRsrvInfoEntity.EmplNo, DbType.String);
            parameters.Add("WeekNum", eghisDoctRsrvInfoEntity.WeekNum, DbType.Int32);
            parameters.Add("ClinicYmd", eghisDoctRsrvInfoEntity.ClinicYmd, DbType.String);
            parameters.Add("ReceptType", receptType, DbType.String);

            var query =$@"
                DELETE FROM hello100_api.eghis_doct_rsrv_detail_info
                      WHERE ridx IN ( SELECT rIdx
                                        FROM hello100_api.eghis_doct_rsrv_info
                                       WHERE hosp_no = @HospNo
                                         AND empl_no = @EmplNo
                                         AND week_num = @WeekNum
                                         AND clinic_ymd = @ClinicYmd )
                        AND recept_type = @ReceptType;
            ";

            return await db.ExecuteAsync(query, parameters, ct, _logger);
        }

        public async Task<int> UpdateDoctorInfoScheduleAsync(DbSession db, List<EghisDoctInfoEntity> eghisDoctInfoList, CancellationToken ct)
        {
            var hospNo = eghisDoctInfoList[0].HospNo;
            var hospKey = eghisDoctInfoList[0].HospKey;
            var emplNo = eghisDoctInfoList[0].EmplNo;
            var doctNo = eghisDoctInfoList[0].DoctNo;
            var doctNm = eghisDoctInfoList[0].DoctNm;
            var deptCd = eghisDoctInfoList[0].DeptCd;
            var deptNm = eghisDoctInfoList[0].DeptNm;

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("HospNo", hospNo, DbType.String);
            parameters.Add("HospKey", hospKey, DbType.String);
            parameters.Add("EmplNo", emplNo, DbType.String);
            parameters.Add("DoctNo", doctNo, DbType.String);
            parameters.Add("DoctNm", doctNm, DbType.String);
            parameters.Add("DeptCd", deptCd, DbType.String);
            parameters.Add("DeptNm", deptNm, DbType.String);

            var queries = new List<string>();
            var query = string.Empty;

            query = @"
                DROP TABLE IF EXISTS hello100_api.tmp_doct_sche;
                CREATE TEMPORARY TABLE hello100_api.tmp_doct_sche (
                  `hosp_no` VARCHAR(10) NOT NULL COMMENT '요양기관번호',
                  `hosp_key` VARCHAR(128) NOT NULL COMMENT '요양기관키',
                  `empl_no` VARCHAR(10) NOT NULL COMMENT '의사사번',
                  `clinic_ymd` VARCHAR(8) NOT NULL DEFAULT '' COMMENT '진료일[\'\': 기본템플릿 사용 ]',
                  `doct_no` VARCHAR(128) NOT NULL COMMENT '의사 면허번호',
                  `doct_nm` VARCHAR(10) NOT NULL COMMENT '의사명',
                  `dept_cd` VARCHAR(10) NULL DEFAULT NULL COMMENT '진료과코드',
                  `dept_nm` VARCHAR(20) NULL DEFAULT NULL COMMENT '진료과명',
                  `week_num` INT(11) NOT NULL DEFAULT '0' COMMENT '요일순번',
                  `start_hour` INT(11) NOT NULL DEFAULT '0' COMMENT '진료시작시간',
                  `start_minute` INT(11) NOT NULL DEFAULT '0' COMMENT '진료시작분',
                  `end_hour` INT(11) NOT NULL DEFAULT '0' COMMENT '진료종료시간',
                  `end_minute` INT(11) NOT NULL DEFAULT '0' COMMENT '진료종료분',
                  `break_start_hour` INT(11) NOT NULL DEFAULT '0' COMMENT '점심시작시간',
                  `break_start_minute` INT(11) NOT NULL DEFAULT '0' COMMENT '점심시작분',
                  `break_end_hour` INT(11) NOT NULL DEFAULT '0' COMMENT '점심종료시간',
                  `break_end_minute` INT(11) NOT NULL DEFAULT '0' COMMENT '점심종료분',
                  `interval_time` INT(11) NOT NULL DEFAULT '0' COMMENT '환자 진료 시간',
                  `message` VARCHAR(1000) NULL DEFAULT NULL COMMENT '요일별 의사 메세지',
                  `hello100_role` INT(11) NOT NULL DEFAULT '0' COMMENT '부가서비스[1: qr접수 , 2:당일접수, 4:예약, 8:qr 접수마감, 16:당일접수마감]',
                  `ridx` INT(11) NOT NULL COMMENT '예약번호',
                  `view_role` INT(11) NOT NULL DEFAULT '0' COMMENT '화면 대기인원 표시[0:사용안함, 1:인원수, 2:시간, 3: 인원수, 시간 모두표시]',
                  `view_min_time` VARCHAR(50) NOT NULL DEFAULT '' COMMENT '대기 시간표시에 따른 최소시간',
                  `view_min_cnt` VARCHAR(50) NOT NULL DEFAULT '' COMMENT '대기 인원표시에 따른 최소인원',
                  `use_yn` CHAR(1) NOT NULL DEFAULT 'Y' COMMENT '사용유무',
                  `reg_dt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '등록날짜',
                  PRIMARY KEY (`hosp_no`, `empl_no`, `clinic_ymd`, `week_num`),
                  INDEX `hosp_key_empl_no` (`hosp_key`, `empl_no`)
                ) COLLATE='utf8_general_ci' ENGINE=InnoDB;
            ";

            queries.Add(query);

            var values = string.Empty;

            for (int i = 0; i < eghisDoctInfoList.Count; i++)
            {
                var eghisDoctInfo = eghisDoctInfoList[i];

                parameters.Add($"Insert_ClinicYmd{i}", eghisDoctInfo.ClinicYmd, DbType.String);
                parameters.Add($"Insert_WeekNum{i}", eghisDoctInfo.WeekNum, DbType.Int32);
                parameters.Add($"Insert_StartHour{i}", eghisDoctInfo.StartHour, DbType.Int32);
                parameters.Add($"Insert_StartMinute{i}", eghisDoctInfo.StartMinute, DbType.Int32);
                parameters.Add($"Insert_EndHour{i}", eghisDoctInfo.EndHour, DbType.Int32);
                parameters.Add($"Insert_EndMinute{i}", eghisDoctInfo.EndMinute, DbType.Int32);
                parameters.Add($"Insert_BreakStartHour{i}", eghisDoctInfo.BreakStartHour, DbType.Int32);
                parameters.Add($"Insert_BreakStartMinute{i}", eghisDoctInfo.BreakStartMinute, DbType.Int32);
                parameters.Add($"Insert_BreakEndHour{i}", eghisDoctInfo.BreakEndHour, DbType.Int32);
                parameters.Add($"Insert_BreakEndMinute{i}", eghisDoctInfo.BreakEndMinute, DbType.Int32);
                parameters.Add($"Insert_IntervalTime{i}", eghisDoctInfo.IntervalTime, DbType.Int32);
                parameters.Add($"Insert_Message{i}", eghisDoctInfo.Message, DbType.String);
                parameters.Add($"Insert_Hello100Role{i}", eghisDoctInfo.Hello100Role, DbType.Int32);
                parameters.Add($"Insert_Ridx{i}", eghisDoctInfo.Ridx, DbType.Int32);
                parameters.Add($"Insert_ViewRole{i}", eghisDoctInfo.ViewRole, DbType.Int32);
                parameters.Add($"Insert_ViewMinTime{i}", eghisDoctInfo.ViewMinTime, DbType.String);
                parameters.Add($"Insert_ViewMinCnt{i}", eghisDoctInfo.ViewMinCnt, DbType.String);
                parameters.Add($"Insert_UseYn{i}", eghisDoctInfo.UseYn, DbType.String);

                if (!string.IsNullOrEmpty(values))
                {
                    values += ", ";
                }

                values += $@"
                    ( @HospNo, @HospKey, @EmplNo, @Insert_ClinicYmd{i}, @DoctNo, @DoctNm, @DeptCd, @DeptNm,
                      @Insert_WeekNum{i}, @Insert_StartHour{i}, @Insert_StartMinute{i}, @Insert_EndHour{i}, @Insert_EndMinute{i}, @Insert_BreakStartHour{i}, @Insert_BreakStartMinute{i}, @Insert_BreakEndHour{i}, @Insert_BreakEndMinute{i}, @Insert_IntervalTime{i},
                      @Insert_Message{i}, @Insert_Hello100Role{i}, @Insert_Ridx{i}, @Insert_ViewRole{i}, @Insert_ViewMinTime{i}, @Insert_ViewMinCnt{i}, @Insert_UseYn{i}, NOW() )
                ";
            }

            query = $@"
                INSERT INTO hello100_api.tmp_doct_sche
                  ( hosp_no, hosp_key, empl_no, clinic_ymd, doct_no, doct_nm, dept_cd, dept_nm,
                    week_num, start_hour, start_minute, end_hour, end_minute, break_start_hour, break_start_minute, break_end_hour, break_end_minute, interval_time,
                    message, hello100_role, ridx, view_role, view_min_time, view_min_cnt, use_yn, reg_dt )
                VALUES {values};
            ";

            queries.Add(query);

            query = @"
                INSERT INTO hello100_api.eghis_doct_info
                  ( hosp_no, hosp_key, empl_no, clinic_ymd, doct_no, doct_nm, dept_cd, dept_nm,
                    week_num, start_hour, start_minute, end_hour, end_minute, break_start_hour, break_start_minute, break_end_hour, break_end_minute, interval_time,
                    message, hello100_role, ridx, view_role, view_min_time, view_min_cnt, use_yn, reg_dt )
                SELECT z.hosp_no, z.hosp_key, z.empl_no, z.clinic_ymd, z.doct_no, z.doct_nm, z.dept_cd, z.dept_nm,
                       z.week_num, z.start_hour, z.start_minute, z.end_hour, z.end_minute, z.break_start_hour, z.break_start_minute, z.break_end_hour, z.break_end_minute, z.interval_time,
                       z.message, z.hello100_role, z.ridx, z.view_role, z.view_min_time, z.view_min_cnt, z.use_yn, z.reg_dt
                  FROM hello100_api.tmp_doct_sche z
                ON DUPLICATE KEY UPDATE
                  doct_no = VALUES(doct_no),
                  doct_nm = VALUES(doct_nm),
                  dept_cd = VALUES(dept_cd),
                  dept_nm = VALUES(dept_nm),
                  week_num = VALUES(week_num),
                  start_hour = VALUES(start_hour),
                  start_minute = VALUES(start_minute),
                  end_hour = VALUES(end_hour),
                  end_minute = VALUES(end_minute),
                  break_start_hour = VALUES(break_start_hour),
                  break_start_minute = VALUES(break_start_minute),
                  break_end_hour = VALUES(break_end_hour),
                  break_end_minute = VALUES(break_end_minute),
                  interval_time = VALUES(interval_time),
                  message = VALUES(message),
                  hello100_role = VALUES(hello100_role),
                  ridx = VALUES(ridx),
                  view_role = VALUES(view_role),
                  view_min_time = VALUES(view_min_time),
                  view_min_cnt = VALUES(view_min_cnt),
                  use_yn = VALUES(use_yn),
                  reg_dt = VALUES(reg_dt);
            ";

            queries.Add(query);

            query = string.Join("", queries.ToArray());

            return await db.ExecuteAsync(query, parameters, ct, _logger);
        }

        public async Task<int> UpdateDoctorInfoUntactScheduleAsync(DbSession db, List<EghisDoctInfoEntity> eghisDoctInfoList, CancellationToken ct)
        {
            var hospNo = eghisDoctInfoList[0].HospNo;
            var hospKey = eghisDoctInfoList[0].HospKey;
            var emplNo = eghisDoctInfoList[0].EmplNo;

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("HospNo", hospNo, DbType.String);
            parameters.Add("HospKey", hospKey, DbType.String);
            parameters.Add("EmplNo", emplNo, DbType.String);

            var queries = new List<string>();
            var query = string.Empty;

            query = @"
                DROP TABLE IF EXISTS hello100_api.tmp_doct_sche_untact;
                CREATE TEMPORARY TABLE hello100_api.tmp_doct_sche_untact (
                  `hosp_no` VARCHAR(10) NOT NULL COMMENT '요양기관번호',
                  `hosp_key` VARCHAR(128) NOT NULL COMMENT '요양기관키',
                  `empl_no` VARCHAR(10) NOT NULL COMMENT '의사사번',
                  `week_num` INT(11) NOT NULL DEFAULT '0' COMMENT '요일순번',
                  `untact_start_hour` INT(11) NOT NULL DEFAULT '0' COMMENT '비대면 진료시작시간',
                  `untact_start_minute` INT(11) NOT NULL DEFAULT '0' COMMENT '비대면 진료시작분',
                  `untact_end_hour` INT(11) NOT NULL DEFAULT '0' COMMENT '비대면 진료종료시간',
                  `untact_end_minute` INT(11) NOT NULL DEFAULT '0' COMMENT '비대면 진료종료분',
                  `untact_interval_time` INT(11) NOT NULL DEFAULT '0' COMMENT '비대면 소요시간',
                  `untact_use_yn` VARCHAR(1) NOT NULL DEFAULT 'N' COMMENT '사용유무',
                  `untact_break_start_hour` INT(11) NOT NULL DEFAULT '0' COMMENT '비대면 점심시작시간',
                  `untact_break_start_minute` INT(11) NOT NULL DEFAULT '0' COMMENT '비대면 점심시작분',
                  `untact_break_end_hour` INT(11) NOT NULL DEFAULT '0' COMMENT '비대면 점심종료시간',
                  `untact_break_end_minute` INT(11) NOT NULL DEFAULT '0' COMMENT '비대면 점심종료분',
                  PRIMARY KEY (`hosp_no`, `empl_no`, `week_num`),
                  INDEX `hosp_key_empl_no` (`hosp_key`, `empl_no`)
                ) COLLATE='utf8_general_ci' ENGINE=InnoDB;
            ";

            queries.Add(query);

            var values = string.Empty;

            for (int i = 0; i < eghisDoctInfoList.Count; i++)
            {
                var eghisDoctInfo = eghisDoctInfoList[i];

                parameters.Add($"Insert_WeekNum{i}", eghisDoctInfo.WeekNum, DbType.Int32);
                parameters.Add($"Insert_UntactStartHour{i}", eghisDoctInfo.UntactStartHour, DbType.Int32);
                parameters.Add($"Insert_UntactStartMinute{i}", eghisDoctInfo.UntactStartMinute, DbType.Int32);
                parameters.Add($"Insert_UntactEndHour{i}", eghisDoctInfo.UntactEndHour, DbType.Int32);
                parameters.Add($"Insert_UntactEndMinute{i}", eghisDoctInfo.UntactEndMinute, DbType.Int32);
                parameters.Add($"Insert_UntactIntervalTime{i}", eghisDoctInfo.UntactIntervalTime, DbType.Int32);
                parameters.Add($"Insert_UntactUseYn{i}", eghisDoctInfo.UntactUseYn, DbType.String);
                parameters.Add($"Insert_UntactBreakStartHour{i}", eghisDoctInfo.UntactBreakStartHour, DbType.Int32);
                parameters.Add($"Insert_UntactBreakStartMinute{i}", eghisDoctInfo.UntactBreakStartMinute, DbType.Int32);
                parameters.Add($"Insert_UntactBreakEndHour{i}", eghisDoctInfo.UntactBreakEndHour, DbType.Int32);
                parameters.Add($"Insert_UntactBreakEndMinute{i}", eghisDoctInfo.UntactBreakEndMinute, DbType.Int32);

                if (!string.IsNullOrEmpty(values))
                {
                    values += ", ";
                }

                values += $@"
                    ( @HospNo, @HospKey, @EmplNo,
                      @Insert_WeekNum{i}, @Insert_UntactStartHour{i}, @Insert_UntactStartMinute{i}, @Insert_UntactEndHour{i}, @Insert_UntactEndMinute{i}, @Insert_UntactIntervalTime{i}, @Insert_UntactUseYn{i},
                      @Insert_UntactBreakStartHour{i}, @Insert_UntactBreakStartMinute{i}, @Insert_UntactBreakEndHour{i}, @Insert_UntactBreakEndMinute{i} )"; 
            }

            queries.Add(query);

            query = $@"
                INSERT INTO hello100_api.tmp_doct_sche_untact
                  ( hosp_no, hosp_key, empl_no,
                    week_num, untact_start_hour, untact_start_minute, untact_end_hour, untact_end_minute, untact_interval_time, untact_use_yn,
                    untact_break_start_hour, untact_break_start_minute, untact_break_end_hour, untact_break_end_minute )
                VALUES {values};
            ";

            queries.Add(query);

            query = @"
                UPDATE hello100_api.eghis_doct_info a
                 INNER JOIN ( SELECT a.hosp_no, a.hosp_key, a.empl_no,
                                     a.week_num, a.untact_start_hour, a.untact_start_minute, a.untact_end_hour, a.untact_end_minute, a.untact_interval_time, a.untact_use_yn,
                                     a.untact_break_start_hour, a.untact_break_start_minute, a.untact_break_end_hour, a.untact_break_end_minute
                                FROM hello100_api.tmp_doct_sche_untact a
                               WHERE a.hosp_no = @HospNo
                                 AND a.empl_no = @EmplNo ) AS b
                    ON a.hosp_no = b.hosp_no AND a.empl_no = b.empl_no AND a.week_num = b.week_num
                   SET a.untact_start_hour = b.untact_start_hour,
                       a.untact_start_minute  = b.untact_start_minute,
                       a.untact_end_hour = b.untact_end_hour,
                       a.untact_end_minute = b.untact_end_minute,
                       a.untact_break_start_hour = b.untact_break_start_hour,
                       a.untact_break_start_minute  = b.untact_break_start_minute,
                       a.untact_break_end_hour = b.untact_break_end_hour,
                       a.untact_break_end_minute = b.untact_break_end_minute,
                       a.untact_interval_time = b.untact_interval_time,
                       a.untact_use_yn = b.untact_use_yn;
            ";

            queries.Add(query);

            query = string.Join("", queries.ToArray());

            return await db.ExecuteAsync(query, parameters, ct, _logger);
        }

        public async Task<int> InsertEghisDoctRsrvAsync(DbSession db, EghisDoctRsrvInfoEntity eghisDoctRsrvInfoEntity, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("HospNo", eghisDoctRsrvInfoEntity.HospNo, DbType.String);
            parameters.Add("EmplNo", eghisDoctRsrvInfoEntity.EmplNo, DbType.String);
            parameters.Add("ClinicYmd", eghisDoctRsrvInfoEntity.ClinicYmd, DbType.String);
            parameters.Add("WeekNum", eghisDoctRsrvInfoEntity.WeekNum, DbType.Int32);
            parameters.Add("RsrvIntervalTime", eghisDoctRsrvInfoEntity.RsrvIntervalTime, DbType.Int32);
            parameters.Add("RsrvIntervalCnt", eghisDoctRsrvInfoEntity.RsrvIntervalCnt, DbType.Int32);

            var query = @"
                INSERT INTO hello100_api.eghis_doct_rsrv_info
                  (hosp_no, empl_no, clinic_ymd, week_num, rsrv_interval_time, rsrv_interval_cnt, reg_dt)
                VALUES
                  (@HospNo, @EmplNo, @ClinicYmd, @WeekNum, @RsrvIntervalTime, @RsrvIntervalCnt, NOW())
                ON DUPLICATE KEY UPDATE
                  rsrv_interval_time = VALUES(rsrv_interval_time),
                  rsrv_interval_cnt = VALUES(rsrv_interval_cnt),
                  reg_dt = VALUES(reg_dt);
                SELECT IFNULL(MAX(rIdx), 0)
                  FROM hello100_api.eghis_doct_rsrv_info
                 WHERE hosp_no = @HospNo
                   AND empl_no = @EmplNo
                   AND clinic_ymd = @ClinicYmd
                   AND week_num = @WeekNum;
            ";

            return await db.ExecuteScalarAsync<int>(query, parameters, ct, _logger);
        }

        public async Task<int> InsertEghisDoctUntactRsrvAsync(DbSession db, EghisDoctRsrvInfoEntity eghisDoctRsrvInfoEntity, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("HospNo", eghisDoctRsrvInfoEntity.HospNo, DbType.String);
            parameters.Add("EmplNo", eghisDoctRsrvInfoEntity.EmplNo, DbType.String);
            parameters.Add("ClinicYmd", eghisDoctRsrvInfoEntity.ClinicYmd, DbType.String);
            parameters.Add("WeekNum", eghisDoctRsrvInfoEntity.WeekNum, DbType.Int32);
            parameters.Add("RsrvIntervalTime", eghisDoctRsrvInfoEntity.RsrvIntervalTime, DbType.Int32);
            parameters.Add("RsrvIntervalCnt", eghisDoctRsrvInfoEntity.RsrvIntervalCnt, DbType.Int32);
            parameters.Add("UntactRsrvIntervalTime", eghisDoctRsrvInfoEntity.UntactRsrvIntervalTime, DbType.Int32);
            parameters.Add("UntactRsrvIntervalCnt", eghisDoctRsrvInfoEntity.UntactRsrvIntervalCnt, DbType.Int32);
            parameters.Add("UntactAvaStartTime", eghisDoctRsrvInfoEntity.UntactAvaStartTime, DbType.String);
            parameters.Add("UntactAvaEndTime", eghisDoctRsrvInfoEntity.UntactAvaEndTime, DbType.String);
            parameters.Add("UntactAvaUseYn", eghisDoctRsrvInfoEntity.UntactAvaUseYn, DbType.String);

            var query = @"
                INSERT INTO hello100_api.eghis_doct_rsrv_info
                  ( hosp_no, empl_no, clinic_ymd, week_num, rsrv_interval_time, rsrv_interval_cnt, reg_dt,
                    untact_rsrv_interval_time, untact_rsrv_interval_cnt, untact_ava_start_time, untact_ava_end_time, untact_ava_use_yn )
                VALUES
                  ( @HospNo, @EmplNo, @ClinicYmd, @WeekNum, @RsrvIntervalTime, @RsrvIntervalCnt, NOW(),
                    @UntactRsrvIntervalTime, @UntactRsrvIntervalCnt, @UntactAvaStartTime, @UntactAvaEndTime, @UntactAvaUseYn )
                ON DUPLICATE KEY UPDATE
                  rsrv_interval_time = VALUES(rsrv_interval_time),
                  rsrv_interval_cnt = VALUES(rsrv_interval_cnt),
                  reg_dt = VALUES(reg_dt),
                  untact_rsrv_interval_time = VALUES(untact_rsrv_interval_time),
                  untact_rsrv_interval_cnt = VALUES(untact_rsrv_interval_cnt),
                  untact_ava_start_time = VALUES(untact_ava_start_time),
                  untact_ava_end_time = VALUES(untact_ava_end_time),
                  untact_ava_use_yn = VALUES(untact_ava_use_yn);
                SELECT IFNULL(MAX(rIdx), 0)
                  FROM hello100_api.eghis_doct_rsrv_info
                 WHERE hosp_no = @HospNo
                   AND empl_no = @EmplNo
                   AND clinic_ymd = @ClinicYmd
                   AND week_num = @WeekNum;
            ";

            return await db.ExecuteScalarAsync<int>(query, parameters, ct, _logger);
        }

        public async Task<int> InsertEghisDoctRsrvDetailAsync(DbSession db, List<EghisDoctRsrvDetailInfoEntity> eghisDoctRsrvDetailInfoList, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();

            var values = string.Empty;

            for (int i = 0; i < eghisDoctRsrvDetailInfoList.Count; i++)
            {
                var eghisDoctRsrvDetailInfoEntity = eghisDoctRsrvDetailInfoList[i];

                parameters.Add($"Ridx{i}", eghisDoctRsrvDetailInfoEntity.Ridx, DbType.String);
                parameters.Add($"StartTime{i}", eghisDoctRsrvDetailInfoEntity.StartTime, DbType.String);
                parameters.Add($"EndTime{i}", eghisDoctRsrvDetailInfoEntity.EndTime, DbType.String);
                parameters.Add($"RsrvCnt{i}", eghisDoctRsrvDetailInfoEntity.RsrvCnt, DbType.Int32);
                parameters.Add($"ComCnt{i}", eghisDoctRsrvDetailInfoEntity.ComCnt, DbType.Int32);
                parameters.Add($"ReceptType{i}", eghisDoctRsrvDetailInfoEntity.ReceptType, DbType.Int32);

                if (!string.IsNullOrEmpty(values))
                {
                    values += ", ";
                }
                
                values += $"( @Ridx{i}, @StartTime{i}, @EndTime{i}, @RsrvCnt{i}, @ComCnt{i}, NOW(), @ReceptType{i} )";
            }

            var query = $@"
                INSERT INTO hello100_api.eghis_doct_rsrv_detail_info
                  (ridx, start_time, end_time, rsrv_cnt, com_cnt, reg_dt, recept_type)
                VALUES {values};
            ";

            return await db.ExecuteScalarAsync<int>(query, parameters, ct, _logger);
        }

        public async Task<int> RemoveEghisDoctInfoMdAsync(DbSession db, EghisDoctInfoMdEntity eghisDoctInfoMdEntity, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("HospNo", eghisDoctInfoMdEntity.HospNo, DbType.String);
            parameters.Add("EmplNo", eghisDoctInfoMdEntity.EmplNo, DbType.String);

            var query = @"
                DELETE FROM hello100_api.eghis_doct_info_md
                      WHERE hosp_no = @HospNo
                        AND empl_no = @EmplNo;
            ";

            return await db.ExecuteScalarAsync<int>(query, parameters, ct, _logger);
        }

        public async Task<int> InsertEghisDoctInfoMdAsync(DbSession db, EghisDoctInfoMdEntity eghisDoctInfoMdEntity, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("HospNo", eghisDoctInfoMdEntity.HospNo, DbType.String);
            parameters.Add("HospKey", eghisDoctInfoMdEntity.HospKey, DbType.String);
            parameters.Add("EmplNo", eghisDoctInfoMdEntity.EmplNo, DbType.String);
            parameters.Add("MdCd", eghisDoctInfoMdEntity.MdCd, DbType.String);

            var query = @"
                INSERT INTO hello100_api.eghis_doct_info_md
                  ( hosp_no, hosp_key, empl_no, md_cd, reg_dt)
                VALUES ( @HospNo, @HospKey, @EmplNo, @MdCd, NOW());
            ";

            return await db.ExecuteScalarAsync<int>(query, parameters, ct, _logger);
        }

        public async Task<int> InsertEghisDoctUntactJoinAsync(DbSession db, TbEghisDoctUntactJoinEntity tbEghisDoctUntactJoinEntity, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("HospNo", tbEghisDoctUntactJoinEntity.HospNo, DbType.String);
            parameters.Add("HospKey", tbEghisDoctUntactJoinEntity.HospKey, DbType.String);
            parameters.Add("HospNm", tbEghisDoctUntactJoinEntity.HospNm, DbType.String);
            parameters.Add("EmplNo", tbEghisDoctUntactJoinEntity.EmplNo, DbType.String);
            parameters.Add("HospTel", tbEghisDoctUntactJoinEntity.HospTel, DbType.String);
            parameters.Add("PostCd", tbEghisDoctUntactJoinEntity.PostCd, DbType.String);
            parameters.Add("DoctNo", tbEghisDoctUntactJoinEntity.DoctNo, DbType.String);
            parameters.Add("DoctNoType", tbEghisDoctUntactJoinEntity.DoctNoType, DbType.String);
            parameters.Add("DoctBirthday", tbEghisDoctUntactJoinEntity.DoctBirthday, DbType.String);
            parameters.Add("DoctTel", tbEghisDoctUntactJoinEntity.DoctTel, DbType.String);
            parameters.Add("DoctIntro", tbEghisDoctUntactJoinEntity.DoctIntro, DbType.String);
            parameters.Add("DoctHistory", tbEghisDoctUntactJoinEntity.DoctHistory, DbType.String);
            parameters.Add("ClinicTime", tbEghisDoctUntactJoinEntity.ClinicTime, DbType.String);
            parameters.Add("ClinicGuide", tbEghisDoctUntactJoinEntity.ClinicGuide, DbType.String);
            parameters.Add("JoinState", tbEghisDoctUntactJoinEntity.JoinState, DbType.String);

            var queries = new List<string>();
            var query = string.Empty;

            var fileInfoList = new List<TbFileInfoEntity?> {
                tbEghisDoctUntactJoinEntity.DoctLicenseFileInfo,
                tbEghisDoctUntactJoinEntity.DoctFileSeqInfo,
                tbEghisDoctUntactJoinEntity.AccountInfoFileInfo,
                tbEghisDoctUntactJoinEntity.BusinessFileInfo,
            };
            
            for (int i = 0; i <  fileInfoList.Count(); i++)
            {
                var fileInfo = fileInfoList[i];

                if (fileInfo == null)
                {
                    query = $@"
                        SET @maxFileSeq{i} := 0;
                    ";

                    queries.Add(query);
                }
                else
                {
                    query = $@"
                        SET @maxFileSeq{i} := ( SELECT IFNULL(MAX(seq), 0) + 1
                                                FROM tb_file_info );

                        INSERT INTO tb_file_info
                          (seq, ord_seq, cls_cd, cm_cd, file_path, origin_file_name, file_size, del_yn, del_dt, reg_dt)
                        VALUES
                          (@maxFileSeq{i}, ( SELECT IFNULL(MAX(tfi.ord_seq), 0) + 1 FROM tb_file_info tfi WHERE tfi.seq = @maxFileSeq{i} ), @ClsCd{i}, @CmCd{i}, @FilePath{i}, @OriginFileName{i}, @FileSize{i}, @DelYn{i}, NULL, UNIX_TIMESTAMP(NOW()));
                    ";

                    queries.Add(query);
                }
                
            }

            query = @"
                INSERT INTO hello100.tb_eghis_doct_untact_join
                  ( hosp_no, hosp_key, hosp_nm, empl_no, hosp_tel, post_cd,
                    doct_no, doct_no_type, doct_license_file_seq, doct_nm, doct_birthday,
                    doct_tel, doct_intro, doct_file_seq, doct_history,
                    clinic_time, clinic_guide, account_info_file_seq, business_file_seq, join_state, reg_dt )
                VALUES
                  ( @HospNo, @HospKey, @HospNm, @EmplNo, @HospTel, @PostCd,
                    @DoctNo, @DoctNoType, @maxFileSeq0, @DoctNm, @DoctBirthday,
                    @DoctTel, @DoctIntro, @maxFileSeq1, @DoctHistory,
                    @ClinicTime, @ClinicGuide, @maxFileSeq2, @maxFileSeq3, @JoinState, UNIX_TIMESTAMP(NOW()) );
            ";

            queries.Add(query);

            return await db.ExecuteAsync(query, parameters, ct, _logger);
        }
    }
}
