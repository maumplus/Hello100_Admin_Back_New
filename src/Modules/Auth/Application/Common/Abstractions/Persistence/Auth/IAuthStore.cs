using Hello100Admin.Modules.Auth.Domain.Entities;

namespace Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth
{
    public interface IAuthStore
    {
        Task<UserEntity?> GetAdminByAidAsync(string aid, CancellationToken cancellationToken = default);
        Task<UserEntity?> GetAdminByAccIdAsync(string accId, CancellationToken cancellationToken = default);
        Task<RefreshTokenEntity?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    }
}
