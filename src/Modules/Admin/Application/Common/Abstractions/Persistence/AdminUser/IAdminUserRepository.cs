using Hello100Admin.Modules.Admin.Domain.Entities;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.AdminUser
{
    public interface IAdminUserRepository
    {
        /// <summary>
        /// AdminUser 저장 (Soft Delete 등 변경 반영)
        /// </summary>
        public Task SaveAsync(AdminUserEntity adminUser, CancellationToken cancellationToken = default);
    }
}
