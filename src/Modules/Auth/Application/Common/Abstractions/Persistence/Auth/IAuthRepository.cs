using Hello100Admin.Modules.Auth.Domain.Entities;

namespace Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth
{
    public interface IAuthRepository
    {
        public Task UpdateLoginSuccessAsync(UserEntity user, CancellationToken cancellationToken = default);
        public Task UpdateLoginFailureAsync(UserEntity user, CancellationToken cancellationToken = default);
        public Task UpdateTokensAsync(UserEntity user, CancellationToken cancellationToken = default);
        public Task<RefreshTokenEntity> AddAsync(RefreshTokenEntity refreshToken, CancellationToken cancellationToken = default);
        public Task UpdateAsync(RefreshTokenEntity refreshToken, CancellationToken cancellationToken = default);
        public Task RevokeUserTokensAsync(string userId, CancellationToken cancellationToken = default);
    }
}
