using Dapper;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
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

        #region IHOSPITALUSERREPOSITORY IMPLEMENTS METHOD AREA **************************************

        public async Task<int> UpdateRequestBugAsync(DbSession db, TbHospitalProposalInfoEntity entity, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("HpId", entity.Hpid, DbType.String);
            parameters.Add("ApprAid", entity.ApprAid, DbType.Int32);

            string query = @"
                UPDATE tb_hospital_proposal_info
                   SET appr_aid = @ApprAId
                       , appr_dt = UNIX_TIMESTAMP(NOW())
                 WHERE hp_id = @HpId
            ";

            var result = await db.ExecuteAsync(query, parameters, ct, _logger);

            /* TODO : 에러코드 머지 후 에러 코드 넣어주기
            if (result <= 0)
                throw new BizException(AdminErrorCode.UpdateUserRoleFailed.ToError());
            */

            return result;
        }

        #endregion
    }
}
