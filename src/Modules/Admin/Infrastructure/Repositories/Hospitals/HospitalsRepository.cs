using Dapper;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper;
using System.Data;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Hello100Admin.Modules.Admin.Domain.Entities;
using System.Text;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.Hospitals
{
    public class HospitalsRepository : IHospitalsRepository
    {
        #region FIELD AREA ****************************************
        private readonly ILogger<HospitalsRepository> _logger;
        #endregion

        #region CONSTRUCTOR AREA *******************************************
        public HospitalsRepository(ILogger<HospitalsRepository> logger, IConfiguration config)
        {
            _logger = logger;
        }
        #endregion

        #region IHOSPITALSREPOSITORY IMPLEMENTS AREA **********************************
        public async Task<int> CreateHospitalAsync(
            DbSession db, string hospNo, List<TbHospitalMedicalInfoEntity>? medicalEntities, TbHospitalInfoEntity hospitalEntity, CancellationToken ct)
        {
            var parameters = new DynamicParameters();
            parameters.Add("HospClsCd", "31", DbType.String);
            parameters.Add("HospNo", hospNo, DbType.String);
            parameters.Add("Name", hospitalEntity.Name, DbType.String);
            parameters.Add("Addr", hospitalEntity.Addr, DbType.String);
            parameters.Add("PostCd", hospitalEntity.PostCd, DbType.String);
            parameters.Add("Tel", hospitalEntity.Tel, DbType.String);
            parameters.Add("Site", hospitalEntity.Site, DbType.String);
            parameters.Add("Lat", hospitalEntity.Lat, DbType.String);
            parameters.Add("Lng", hospitalEntity.Lng, DbType.String);
            parameters.Add("EncKey", "d3fa7fa7873c38097b31feb7bcd1c017ff222aee", DbType.String);

            #region == Query ==
            StringBuilder sb = new StringBuilder();

            #region [진료과 저장]
            //진료과 저장
            if (medicalEntities?.Count > 0)
            {
                StringBuilder sbDeptCodes = new StringBuilder();
                medicalEntities.ForEach(x =>
                {
                    sbDeptCodes.AppendLine($",('{x.MdCd}', func_hosp_key_generator(@EncKey,CONCAT(@HospNo,@Name)), UNIX_TIMESTAMP(NOW()))");
                });
                sb.AppendLine("  INSERT INTO tb_hospital_medical_info   ");
                sb.AppendLine("	     (md_cd, hosp_key, reg_dt)          ");
                sb.AppendLine("	 values                                 ");
                sb.AppendLine(sbDeptCodes.ToString().Substring(1, sbDeptCodes.ToString().Length - 2) + ";");
            }
            #endregion

            #region [병원 상세 저장]
            sb.AppendLine(" INSERT INTO tb_hospital_info");
            sb.AppendLine("     (   hosp_key");
            sb.AppendLine("     ,   hosp_cls_cd");
            sb.AppendLine("     ,   name");
            sb.AppendLine("     ,   addr");
            sb.AppendLine("     ,   post_cd");
            sb.AppendLine("     ,   tel");
            sb.AppendLine("     ,   site");
            sb.AppendLine("     ,   lat");
            sb.AppendLine("     ,   lng");
            sb.AppendLine("     ,   reg_dt,location");
            sb.AppendLine("     ) ");
            sb.AppendLine(" VALUES      ");
            sb.AppendLine("     (   func_hosp_key_generator(@EncKey,CONCAT(@HospNo,@Name))");
            sb.AppendLine("     ,   @HospClsCd");
            sb.AppendLine("     ,   @Name");
            sb.AppendLine("     ,   @Addr");
            sb.AppendLine("     ,   @PostCd");
            sb.AppendLine("     ,   @Tel");
            sb.AppendLine("     ,   @Site");
            sb.AppendLine("     ,   @Lat");
            sb.AppendLine("     ,   @Lng");
            sb.AppendLine("	    , 	UNIX_TIMESTAMP(NOW()),ST_GEOMFROMTEXT(CONCAT('POINT(',@Lat, ' ', @Lng,')'), 5186)  ");
            sb.AppendLine("     );");
            #endregion
            #endregion

            var result = await db.ExecuteAsync(sb.ToString(), parameters, ct, _logger);

            if (result <= 0)
                throw new BizException(AdminErrorCode.CreateHospitalFailed.ToError());

            return result;
        }

        public async Task<int> UpdateHospitalAsync(
          DbSession db, List<TbHospitalMedicalInfoEntity>? medicalEntities, TbHospitalInfoEntity hospitalEntity, CancellationToken ct)
        {
            var parameters = new DynamicParameters();
            parameters.Add("HospClsCd", "31", DbType.String);
            parameters.Add("HospKey", hospitalEntity.HospKey, DbType.String);
            parameters.Add("Name", hospitalEntity.Name, DbType.String);
            parameters.Add("Addr", hospitalEntity.Addr, DbType.String);
            parameters.Add("PostCd", hospitalEntity.PostCd, DbType.String);
            parameters.Add("Tel", hospitalEntity.Tel, DbType.String);
            parameters.Add("Site", hospitalEntity.Site, DbType.String);
            parameters.Add("Lat", hospitalEntity.Lat, DbType.String);
            parameters.Add("Lng", hospitalEntity.Lng, DbType.String);
            parameters.Add("IsTest", hospitalEntity.IsTest, DbType.String);

            #region == Query ==
            StringBuilder sb = new StringBuilder();

            #region [진료과 저장]
            sb.AppendLine("  DELETE FROM tb_hospital_medical_info   ");
            sb.AppendLine("   WHERE hosp_key =  @HospKey   ;  ");

            if (medicalEntities?.Count > 0)
            {
                StringBuilder sbDeptCodes = new StringBuilder();
                medicalEntities.ForEach(x =>
                {
                    sbDeptCodes.AppendLine($",('{x.MdCd}', @HospKey, UNIX_TIMESTAMP(NOW()))");
                });
                sb.AppendLine("  INSERT INTO tb_hospital_medical_info   ");
                sb.AppendLine("	     (md_cd, hosp_key, reg_dt)          ");
                sb.AppendLine("	 values                                 ");
                sb.AppendLine(sbDeptCodes.ToString().Substring(1, sbDeptCodes.ToString().Length - 2) + ";");
            }
            #endregion

            #region [병원 상세 업데이트]
            sb.AppendLine("  UPDATE tb_hospital_info");
            sb.AppendLine("     SET  hosp_key     =@HospKey");
            sb.AppendLine("      ,   hosp_cls_cd  =@HospClsCd");
            sb.AppendLine("      ,   name         =@Name   ");
            sb.AppendLine("      ,   addr         =@Addr   ");
            sb.AppendLine("      ,   post_cd      =@PostCd ");
            sb.AppendLine("      ,   tel          =@Tel    ");
            sb.AppendLine("      ,   site         =@Site    ");
            sb.AppendLine("      ,   lat          =@Lat    ");
            sb.AppendLine("      ,   lng          =@Lng    ");
            sb.AppendLine("      ,   is_test      =@IsTest ");
            sb.AppendLine("   WHERE hosp_key =  @HospKey ;  ");
            #endregion
            #endregion

            var result = await db.ExecuteAsync(sb.ToString(), parameters, ct, _logger);

            if (result <= 0)
                throw new BizException(AdminErrorCode.UpsertHospitalFailed.ToError());

            return result;
        }

        public async Task<int> DeleteHospitalAsync(DbSession db, string? hospNo, string hospKey, CancellationToken ct)
        {
            var parameters = new DynamicParameters();
            parameters.Add("HospNo", hospNo, DbType.String);
            parameters.Add("HospKey", hospKey, DbType.String);

            #region == Query ==
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" DELETE FROM tb_hospital_info");
            sb.AppendLine("  WHERE hosp_key = @HospKey ;");

            // tb_hospital_medical_info 삭제도 필요할 것으로 보임

            if (string.IsNullOrWhiteSpace(hospNo) == false)
            {
                sb.AppendLine(" UPDATE tb_eghis_hosp_info");
                sb.AppendLine("    SET del_yn = 'Y'");
                sb.AppendLine("  WHERE hosp_key = @HospKey;");

                sb.AppendLine(" UPDATE tb_admin");
                sb.AppendLine("    SET del_yn = 'Y'");
                sb.AppendLine("  WHERE hosp_no = @HospNo;");
            }
            #endregion

            var result = await db.ExecuteAsync(sb.ToString(), parameters, ct, _logger);

            if (result <= 0)
                throw new BizException(AdminErrorCode.DeleteHospitalFailed.ToError());

            return result;
        }
        #endregion
    }
}
