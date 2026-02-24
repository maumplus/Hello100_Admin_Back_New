using Dapper;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Account;
using Hello100Admin.Modules.Admin.Domain.Entities;
using System.Data;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.Account
{
    public class AccountRepository : IAccountRepository
    {
        public async Task<TbEghisHospInfoEntity?> GetEghisHospInfoAsync(DbSession db, string HospNo, CancellationToken cancellationToken)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("HospNo", HospNo, DbType.String);

            string query = @"
                SELECT a.hosp_key   AS HospKey,
                       a.hosp_no    AS HospNo,
                       a.desc       AS Description,
                       a.closing_yn AS ClosingYn,
                       a.del_yn     AS DelYn,
                       a.chart_type AS ChartType
                  FROM tb_eghis_hosp_info a
                 WHERE a.hosp_no = @HospNo;
            ";

            return await db.QueryFirstOrDefaultAsync<TbEghisHospInfoEntity>(query, parameters, cancellationToken);
        }

        public async Task<int> UpdateAdminAsync(DbSession db, TbAdminEntity entity, CancellationToken cancellationToken)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("AId", entity.Aid, DbType.String);
            parameters.Add("HospNo", entity.HospNo, DbType.String);

            string query = @"
                UPDATE tb_admin
                   SET hosp_no = @HospNo
                 WHERE aid = @AId;
            ";

            return await db.ExecuteAsync(query, parameters, cancellationToken);
        }

        public async Task<int> UpdateEghisHospInfoAsync(DbSession db, TbEghisHospInfoEntity entity, CancellationToken cancellationToken)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("HospKey", entity.HospKey, DbType.String);
            parameters.Add("HospNo", entity.HospNo, DbType.String);
            parameters.Add("ChartType", entity.ChartType, DbType.String);

            string query = @"
                INSERT INTO tb_eghis_hosp_info (hosp_key, hosp_no, reg_dt, chart_type)
                SELECT a.hosp_key,
                       @HospNo,
                       UNIX_TIMESTAMP(NOW()),
                       @ChartType
                  FROM tb_hospital_info a
                 WHERE a.hosp_key = @HospKey
                   AND NOT EXISTS ( SELECT 'Y'
                                      FROM tb_eghis_hosp_info t
                                     WHERE t.hosp_key = a.hosp_key );
            ";

            return await db.ExecuteAsync(query, parameters, cancellationToken);
        }

        public async Task<int> UpdateEghisHospQrInfoAsync(DbSession db, TbEghisHospQrInfoEntity entity, CancellationToken cancellationToken)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Aid", entity.Aid, DbType.String);
            parameters.Add("HospKey", entity.HospKey, DbType.String);

            string query = @"
                INSERT INTO tb_eghis_hosp_qr_info (qid, hosp_key, reg_dt, iss_dt)
                SELECT a.qid,
                       @HospKey,
                       UNIX_TIMESTAMP(NOW()),
                       UNIX_TIMESTAMP(NOW())
                  FROM tb_make_qr_info a
                 WHERE a.aid = @Aid
                ON DUPLICATE KEY UPDATE
                  hosp_key = @HospKey,
                  reg_dt = UNIX_TIMESTAMP(NOW());
            ";

            return await db.ExecuteAsync(query, parameters, cancellationToken);
        }

        public async Task<int> UpdateEghisRecertDocInfoAsync(DbSession db, TbEghisRecertDocInfoEntity entity, CancellationToken cancellationToken)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("HospKey", entity.HospKey, DbType.String);

            string query = @"
                INSERT INTO tb_eghis_recert_doc_info (hosp_key, re_doc_cd, show_yn, sort_no, reg_dt)
                SELECT @HospKey,
                       a.cm_cd,
                       'Y',
                       a.sort,
                       UNIX_TIMESTAMP(NOW())
                  FROM tb_common a
                 WHERE cls_cd = '13'
                   AND del_yn = 'N';
            ";

            return await db.ExecuteAsync(query, parameters, cancellationToken);
        }

        public async Task<int> InsertEghisHospVisitPurposeInfoAsync(DbSession db, List<TbEghisHospVisitPurposeInfoEntity> entities, CancellationToken cancellationToken)
        {
            DynamicParameters parameters = new DynamicParameters();

            var values = "";

            for (var i = 0; i < entities.Count(); i++)
            {
                var entity = entities[i];

                parameters.Add($"VpCd{i}", entity.VpCd, DbType.String);
                parameters.Add($"ParentCd{i}", entity.ParentCd, DbType.String);
                parameters.Add($"HospKey{i}", entity.HospKey, DbType.String);
                parameters.Add($"InpuiryUrl{i}", entity.InpuiryUrl, DbType.String);
                parameters.Add($"InpuiryIdx{i}", entity.InpuiryIdx, DbType.Int32);
                parameters.Add($"InpuirySkipYn{i}", entity.InpuirySkipYn, DbType.String);
                parameters.Add($"Name{i}", entity.Name, DbType.String);
                parameters.Add($"ShowYn{i}", entity.ShowYn, DbType.String);
                parameters.Add($"SortNo{i}", entity.SortNo, DbType.Int32);
                parameters.Add($"DelYn{i}", entity.DelYn, DbType.String);
                parameters.Add($"Role{i}", entity.Role, DbType.Int32);

                if (!string.IsNullOrEmpty(values))
                {
                    values += ", ";
                }

                values += $"(@VpCd{i}, @ParentCd{i}, @HospKey{i}, @InpuiryUrl{i}, @InpuiryIdx{i}, @InpuirySkipYn{i}, @Name{i}, @ShowYn{i}, @SortNo{i}, @DelYn{i}, @Role{i}, UNIX_TIMESTAMP(NOW()))";
            }

            string query = $@"
                INSERT INTO tb_eghis_hosp_visit_purpose_info (vp_cd, parent_cd, hosp_key, inpuiry_url, inpuiry_idx, inpuiry_skip_yn, name, show_yn, sort_no, del_yn, role, reg_dt)
                VALUES {values};
            ";

            return await db.ExecuteAsync(query, parameters, cancellationToken);
        }

        public async Task<int> InsertEghisHospSettingsInfoAsync(DbSession db, TbEghisHospSettingsInfoEntity entity, CancellationToken cancellationToken)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("HospKey", entity.HospKey, DbType.String);

            string query = @"
                INSERT INTO tb_eghis_hosp_settings_info
                  (  hosp_key,
                     role,
                     wait_tm,
                     reg_dt
                  )                                    
                VALUES                                  
                  (
                     @HospKey,
                     '3',
                     '',
                     UNIX_TIMESTAMP(NOW())
                  );";

            return await db.ExecuteAsync(query, parameters, cancellationToken);
        }

        public async Task<int> InsertTbEghisHospMedicalTimeNewAsync(DbSession db, List<TbEghisHospMedicalTimeNewEntity> entities, CancellationToken cancellationToken)
        {
            DynamicParameters parameters = new DynamicParameters();

            var values = "";

            for (var i = 0; i < entities.Count(); i++)
            {
                var entity = entities[i];

                parameters.Add($"HospKey{i}", entity.HospKey, DbType.String);
                parameters.Add($"HospNo{i}", entity.HospNo, DbType.String);
                parameters.Add($"WeekNum{i}", entity.WeekNum, DbType.Int32);
                parameters.Add($"StartHour{i}", entity.StartHour, DbType.Int32);
                parameters.Add($"StartMinute{i}", entity.StartMinute, DbType.Int32);
                parameters.Add($"EndHour{i}", entity.EndHour, DbType.Int32);
                parameters.Add($"EndMinute{i}", entity.EndMinute, DbType.Int32);
                parameters.Add($"BreakStartHour{i}", entity.BreakStartHour, DbType.Int32);
                parameters.Add($"BreakStartMinute{i}", entity.BreakStartMinute, DbType.Int32);
                parameters.Add($"BreakEndHour{i}", entity.BreakEndHour, DbType.Int32);
                parameters.Add($"BreakEndMinute{i}", entity.BreakEndMinute, DbType.Int32);
                parameters.Add($"UseYn{i}", entity.UseYn, DbType.String);

                if (!string.IsNullOrEmpty(values))
                {
                    values += ", ";
                }

                values += $"(@HospKey{i}, @HospNo{i}, @WeekNum{i}, @StartHour{i}, @StartMinute{i}, @EndHour{i}, @EndMinute{i}, @BreakStartHour{i}, @BreakStartMinute{i}, @BreakEndHour{i}, @BreakEndMinute{i}, @UseYn{i}, UNIX_TIMESTAMP(NOW()))";
            }

            string query = $@"
                INSERT INTO hello100.tb_eghis_hosp_medical_time_new
                  (hosp_key, hosp_no, week_num, start_hour, start_minute, end_hour, end_minute, break_start_hour, break_start_minute, break_end_hour, break_end_minute, use_yn, reg_dt)
                VALUES {values};";

            return await db.ExecuteAsync(query, parameters, cancellationToken);
        }
    }
}