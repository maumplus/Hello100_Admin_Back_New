using Hello100Admin.Modules.Auth.Domain.Entities;

namespace Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth
{
    public interface IAuthStore
    {
        public Task<UserEntity?> GetByAidAsync(string aid, CancellationToken cancellationToken = default);
        public Task<UserEntity?> GetByUsernameAsync(string accId, CancellationToken cancellationToken = default);
        public Task<RefreshTokenEntity?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    }
}
