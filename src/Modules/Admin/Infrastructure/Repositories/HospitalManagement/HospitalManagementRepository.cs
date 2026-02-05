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
            parameters.Add("EncKey", "d3fa7fa7873c38097b31feb7bcd1c017ff222aee", DbType.Int32);

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
            parameters.Add("EncKey", "d3fa7fa7873c38097b31feb7bcd1c017ff222aee", DbType.Int32);
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
    }
}
