using Hello100Admin.Modules.Admin.Domain.Entities;

namespace Hello100Admin.Modules.Admin.Domain.Interfaces;

/// <summary>
/// 관리자 사용자 리포지토리 인터페이스
/// </summary>
public interface IAdminUserRepository
{
    /// <summary>
    /// 관리자 사용자 조회 (가볍게)
    /// </summary>
    Task<AdminUser?> GetByIdAsync(string uid, CancellationToken cancellationToken = default);

    /// <summary>
    /// AdminUser 조회 (상세 정보)
    /// </summary>
    Task<AdminUser?> GetByIdWithAdminUserAsync(string uid, CancellationToken cancellationToken = default);

    /// <summary>
    /// 삭제된 것 포함하여 조회 (관리 목적)
    /// </summary>
    Task<AdminUser?> GetByIdIncludeDeletedAsync(string uid, CancellationToken cancellationToken = default);

    /// <summary>
    /// 페이징 조회
    /// </summary>
    Task<(List<AdminUser> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// AdminUser 저장 (Soft Delete 등 변경 반영)
    /// </summary>
    Task SaveAsync(AdminUser adminUser, CancellationToken cancellationToken = default);
}

