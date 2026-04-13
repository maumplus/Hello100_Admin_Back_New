using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Auth.Application.Common.Models;
using Hello100Admin.Modules.Auth.Application.Common.Views;
using Hello100Admin.Modules.Auth.Domain.Entities;

namespace Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth
{
    public interface IAuthStore
    {
        Task<TbAdminView?> GetAdminByAidAsync(string aid, CancellationToken cancellationToken = default);
        Task<TbAdminView?> GetAdminByAccIdAsync(string accId, CancellationToken cancellationToken = default);
        Task<TbAdminView?> GetAdminByHospNoAsync(string hospNo, CancellationToken cancellationToken = default);
        Task<RefreshTokenEntity?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
        Task<AppAuthNumberInfoEntity?> GetAppAuthNumberInfoAsync(int authId, CancellationToken cancellationToken = default);
        /// <summary>
        /// 현재 로그인 사용자의 요양기관 정보 가져오기
        /// </summary>
        /// <param name="db"></param>
        /// <param name="hospNo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<CurrentHospitalInfo?> GetHospitalInfoByHospNoAsync(DbSession db, string hospNo, CancellationToken cancellationToken = default);
    }
}
