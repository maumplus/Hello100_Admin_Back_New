using Hello100Admin.Modules.Auth.Domain.Entities;

namespace Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth
{
    public interface IAuthRepository
    {
        Task<RefreshTokenEntity> AddAsync(RefreshTokenEntity refreshToken, CancellationToken cancellationToken = default);
        Task UpdateAsync(RefreshTokenEntity refreshToken, CancellationToken cancellationToken = default);
        Task RevokeUserTokensAsync(string userId, CancellationToken cancellationToken = default);
        Task UpdateLoginFailureAsync(AdminEntity user, CancellationToken cancellationToken = default);
        Task UpdateLoginSuccessAsync(AdminEntity user, CancellationToken cancellationToken = default);
        Task UpdateTokensAsync(AdminEntity user, CancellationToken cancellationToken = default);
        Task UpdateAccessTokenAsync(AdminEntity admin, CancellationToken cancellationToken = default);
        Task InsertAdminLogAsync(AdminLogEntity adminLog, CancellationToken cancellationToken = default);
        Task<int> InsertAsync(AppAuthNumberInfoEntity appAuthNumberInfo, CancellationToken cancellationToken = default);
        Task UpdateConfirmAsync(AppAuthNumberInfoEntity appAuthNumberInfo, CancellationToken cancellationToken = default);
    }
}
