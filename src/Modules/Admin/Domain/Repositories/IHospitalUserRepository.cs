using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Domain.Entities;

namespace Hello100Admin.Modules.Admin.Domain.Repositories
{
    public interface IHospitalUserRepository
    {
        /// <summary>
        /// 병원 사용자 권한 수정
        /// </summary>
        /// <param name="db"></param>
        /// <param name="entity"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<int> UpdateHospitalUserRoleAsync(DbSession db, TbUserEntity entity, CancellationToken ct);

        /// <summary>
        /// 회원 가족 삭제
        /// </summary>
        /// <param name="db"></param>
        /// <param name="uId"></param>
        /// <param name="mId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<int> DeleteUserFamilyAsync(DbSession db, string uId, int mId, CancellationToken ct);

        /// <summary>
        /// 회원 삭제
        /// </summary>
        /// <param name="db"></param>
        /// <param name="uId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<int> DeleteUserAsync(DbSession db, string uId, CancellationToken ct);
    }
}
