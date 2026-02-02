using Hello100Admin.Modules.Auth.Application.Features.Auth.ReadModels;
using Hello100Admin.Modules.Auth.Domain.Entities;

namespace Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth
{
    public interface IAuthStore
    {
        Task<AdminModel?> GetAdminByAidAsync(string aid, CancellationToken cancellationToken = default);
        Task<AdminModel?> GetAdminByAccIdAsync(string accId, CancellationToken cancellationToken = default);
        Task<RefreshTokenEntity?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    }
}
