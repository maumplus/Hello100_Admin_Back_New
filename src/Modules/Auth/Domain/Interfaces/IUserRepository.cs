using Hello100Admin.Modules.Auth.Domain.Entities;

namespace Hello100Admin.Modules.Auth.Domain.Interfaces;

/// <summary>
/// User 엔티티를 위한 저장소 인터페이스
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByAidAsync(string aid, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string accId, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateLoginSuccessAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateLoginFailureAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateTokensAsync(User user, CancellationToken cancellationToken = default);
}
