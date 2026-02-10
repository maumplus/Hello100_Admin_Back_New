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

        public async Task<int> UpdateDoctorUntanctAsync(DbSession db, TbEghisDoctUntanctEntity eghisDoctUntanct, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("HospNo", eghisDoctUntanct.HospNo, DbType.String);
            parameters.Add("EmplNo", eghisDoctUntanct.EmplNo, DbType.String);
            parameters.Add("DoctIntro", eghisDoctUntanct.DoctIntro, DbType.String);
            parameters.Add("ClinicGuide", eghisDoctUntanct.ClinicGuide, DbType.String);
            parameters.Add("DoctHistory", eghisDoctUntanct.DoctHistory, DbType.String);

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
    }
}
