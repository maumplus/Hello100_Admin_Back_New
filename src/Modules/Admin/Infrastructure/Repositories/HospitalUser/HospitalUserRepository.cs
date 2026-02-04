using Dapper;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using System.Data;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.HospitalUser
{
    public class HospitalUserRepository : IHospitalUserRepository
    {
        #region FIELD AREA ****************************************
        private readonly ILogger<HospitalUserRepository> _logger;
        #endregion

        #region CONSTRUCTOR AREA *******************************************
        public HospitalUserRepository(ILogger<HospitalUserRepository> logger)
        {
            _logger = logger;
        }
        #endregion

        #region IHOSPITALUSERREPOSITORY IMPLEMENTS METHOD AREA **************************************
        public async Task<int> UpdateHospitalUserRoleAsync(DbSession db, TbUserEntity entity, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("UId", entity.UId, DbType.String);
            parameters.Add("UserRole", entity.UserRole, DbType.Int32);

            string query = @"
                UPDATE tb_user
                   SET user_role = @UserRole
                 WHERE uid = @UId
            ";

            var result = await db.ExecuteAsync(query, parameters, ct, _logger);

            if (result <= 0)
                throw new BizException(AdminErrorCode.UpdateUserRoleFailed.ToError());

            return result;
        }

        public async Task<int> DeleteUserFamilyAsync(DbSession db, string uId, int mId, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("UId", uId, DbType.String);
            parameters.Add("MId", mId, DbType.Int32);

            string query = @"
                UPDATE tb_member
                   SET del_yn = 'Y'
                 WHERE uid = @UId
                   AND mid = @MId
            ";

            var result = await db.ExecuteAsync(query, parameters, ct, _logger);

            if (result <= 0)
                throw new BizException(AdminErrorCode.DeleteUserFamilyFailed.ToError());

            return result;
        }

        public async Task<int> DeleteUserAsync(DbSession db, string uId, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("UId", uId, DbType.String);

            string query = @"
                UPDATE tb_user
                   SET del_yn = 'Y'
                 WHERE uid = @UId
            ";

            var result = await db.ExecuteAsync(query, parameters, ct, _logger);

            if (result <= 0)
                throw new BizException(AdminErrorCode.DeleteUserFailed.ToError());

            return result;
        }
        #endregion
    }
}
