using Dapper;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Microsoft.Extensions.Logging;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.AdminUser;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.AdminUser;

public class AdminUserRepository : IAdminUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<AdminUserRepository> _logger;

    public AdminUserRepository(IDbConnectionFactory connectionFactory, ILogger<AdminUserRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task SaveAsync(AdminUserEntity adminUser, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Saving AdminUser. Aid: {Aid}", adminUser.Aid);
            using var connection = _connectionFactory.CreateConnection();
            var sql = "UPDATE tb_admin SET is_deleted = 1 WHERE account_id = @AccountId";
            await connection.ExecuteAsync(sql, new { AccountId = adminUser.Aid });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving AdminUser. Aid: {Aid}", adminUser.Aid);
            throw;
        }
    }
}
