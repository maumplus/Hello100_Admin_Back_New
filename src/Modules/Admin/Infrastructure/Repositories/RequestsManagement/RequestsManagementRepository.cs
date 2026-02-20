using Dapper;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.RequestsManagement
{
    public class RequestsManagementRepository : IRequestsManagementRepository
    {
        #region FIELD AREA ****************************************
        private readonly ILogger<RequestsManagementRepository> _logger;
        #endregion

        #region CONSTRUCTOR AREA *******************************************
        public RequestsManagementRepository(ILogger<RequestsManagementRepository> logger)
        {
            _logger = logger;
        }
        #endregion

        #region IREQUESTSMANAGEMENTREPOSITORY IMPLEMENTS METHOD AREA **************************************

        public async Task<int> UpdateRequestBugAsync(DbSession db, TbHospitalProposalInfoEntity entity, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("HpId", entity.Hpid, DbType.String);
            parameters.Add("ApprAid", entity.ApprAid, DbType.Int32);

            string query = @"
                UPDATE tb_hospital_proposal_info
                   SET appr_aid = @ApprAId
                       , appr_dt = UNIX_TIMESTAMP(NOW())
                 WHERE hp_id = @HpId;
            ";

            System.Diagnostics.Debug.WriteLine("Executing SQL:\n" + query);
            foreach (var paramName in parameters.ParameterNames)
            {
                System.Diagnostics.Debug.WriteLine($"Parameter {paramName} = {parameters.Get<object>(paramName)}");
            }

            var result = await db.ExecuteAsync(query, parameters, ct, _logger);

            if (result <= 0)
                throw new BizException(AdminErrorCode.UpdateRequestBug.ToError());

            return result;
        }

        public async Task<int> UpdateRequestUntactAsync(DbSession db, TbEghisDoctUntactJoinEntity entity, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Seq", entity.Seq, DbType.Int32);
            parameters.Add("JoinState", entity.JoinState, DbType.String);
            parameters.Add("ReturnReason", entity.ReturnReason, DbType.String);

            string query = @"
                UPDATE tb_eghis_doct_untact_join
                   SET join_state = @JoinState
                       , return_reason = @ReturnReason
                       , join_state_mod_dt = UNIX_TIMESTAMP(NOW())
                 WHERE seq = @Seq;
            ";

            //승인 : 02
            if (entity.JoinState == "02")
            {
                query += @"
                    INSERT INTO hello100.tb_eghis_doct_untact
                        (hosp_no, hosp_key, hosp_nm, empl_no, update_dt, hosp_tel, post_cd, doct_no, doct_no_type, doct_license_file_seq, doct_nm, doct_birthday, doct_tel, doct_intro, doct_file_seq, doct_history, clinic_time, clinic_guide, account_info_file_seq, business_file_seq, join_state, use_yn, reg_dt)
                    SELECT hosp_no, hosp_key, hosp_nm, empl_no,unix_timestamp(now()), hosp_tel, post_cd, doct_no, doct_no_type, doct_license_file_seq, doct_nm, doct_birthday, doct_tel, doct_intro, doct_file_seq, doct_history, clinic_time, clinic_guide, account_info_file_seq, business_file_seq, join_state, 'Y', unix_timestamp(now())
                      FROM hello100.tb_eghis_doct_untact_join
                     where seq = @Seq;
                ";

                //의사사진 저장
                //2025.02.25 의사사진을 비대면 신청 하기 전 미리 저장해놓은 경우 insert시 에러 발생.
                //DUPLICATE KEY UPDATE 비대면 신청 사진으로 업데이트 하기로 협의함.(정설아프로)
                query += @"
                    INSERT INTO hello100.tb_eghis_doct_info_file (hosp_no, hosp_key, empl_no, doct_file_seq, reg_dt)
                    SELECT hosp_no, hosp_key, empl_no, tj.doct_file_seq, UNIX_TIMESTAMP(NOW())
                      FROM hello100.tb_eghis_doct_untact_join tj
                     WHERE seq = @Seq
                        ON DUPLICATE KEY UPDATE doct_file_seq = tj.doct_file_seq;
                ";
            }

            System.Diagnostics.Debug.WriteLine("Executing SQL:\n" + query);
            foreach (var paramName in parameters.ParameterNames)
            {
                System.Diagnostics.Debug.WriteLine($"Parameter {paramName} = {parameters.Get<object>(paramName)}");
            }

            var result = await db.ExecuteAsync(query, parameters, ct, _logger);

            if (result <= 0)
                throw new BizException(AdminErrorCode.UpdateRequestUntact.ToError());

            return result;
        }

        #endregion
    }
}
