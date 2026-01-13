using Hello100Admin.Modules.Admin.Domain.Entities;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.AdminUser
{
    public interface IAdminUserStore
    {
        /// <summary>
        /// 관리자 사용자 조회 (가볍게)
        /// </summary>
        public Task<AdminUserEntity?> GetByIdAsync(string uid, CancellationToken cancellationToken = default);

        /// <summary>
        /// AdminUser 조회 (상세 정보)
        /// </summary>
        public Task<AdminUserEntity?> GetByIdWithAdminUserAsync(string uid, CancellationToken cancellationToken = default);

        /// <summary>
        /// 삭제된 것 포함하여 조회 (관리 목적)
        /// </summary>
        public Task<AdminUserEntity?> GetByIdIncludeDeletedAsync(string uid, CancellationToken cancellationToken = default);

        /// <summary>
        /// 페이징 조회
        /// </summary>
        public Task<(List<AdminUserEntity> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            bool includeDeleted = false,
            CancellationToken cancellationToken = default);
    }
}
