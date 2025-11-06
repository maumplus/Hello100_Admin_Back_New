using Dapper;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence;
using Hello100Admin.Modules.Admin.Domain.Interfaces;
using Hello100Admin.Modules.Admin.Infrastructure.Models;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories;

public class AdminUserRepository : IAdminUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<AdminUserRepository> _logger;

    public AdminUserRepository(IDbConnectionFactory connectionFactory, ILogger<AdminUserRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<AdminUser?> GetByIdAsync(string accountId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting AdminUser by AccountId: {AccountId}", accountId);
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM tb_admin WHERE account_id = @AccountId AND is_deleted = 0 LIMIT 1";
            var dbUser = await connection.QueryFirstOrDefaultAsync<AdminUserDbModel>(sql, new { AccountId = accountId });
            if (dbUser == null)
            {
                _logger.LogWarning("No AdminUser found for AccountId: {AccountId}", accountId);
                return null;
            }
            return MapToDomain(dbUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting AdminUser by AccountId: {AccountId}", accountId);
            throw;
        }
    }

    public async Task<AdminUser?> GetByIdWithAdminUserAsync(string accountId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting AdminUser (with details) by AccountId: {AccountId}", accountId);
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM tb_admin WHERE account_id = @AccountId AND is_deleted = 0 LIMIT 1";
            var dbUser = await connection.QueryFirstOrDefaultAsync<AdminUserDbModel>(sql, new { AccountId = accountId });
            if (dbUser == null)
            {
                _logger.LogWarning("No AdminUser (with details) found for AccountId: {AccountId}", accountId);
                return null;
            }
            return MapToDomain(dbUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting AdminUser (with details) by AccountId: {AccountId}", accountId);
            throw;
        }
    }

    public async Task<AdminUser?> GetByIdIncludeDeletedAsync(string accountId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting AdminUser (include deleted) by AccountId: {AccountId}", accountId);
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM tb_admin WHERE account_id = @AccountId LIMIT 1";
            var dbUser = await connection.QueryFirstOrDefaultAsync<AdminUserDbModel>(sql, new { AccountId = accountId });
            if (dbUser == null)
            {
                _logger.LogWarning("No AdminUser (include deleted) found for AccountId: {AccountId}", accountId);
                return null;
            }
            return MapToDomain(dbUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting AdminUser (include deleted) by AccountId: {AccountId}", accountId);
            throw;
        }
    }

    public async Task<(List<AdminUser> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting paged AdminUsers. Page: {Page}, PageSize: {PageSize}, IncludeDeleted: {IncludeDeleted}", page, pageSize, includeDeleted);
            using var connection = _connectionFactory.CreateConnection();
            var offset = (page - 1) * pageSize;
            var sql = includeDeleted
                ? "SELECT * FROM tb_admin LIMIT @PageSize OFFSET @Offset"
                : "SELECT * FROM tb_admin WHERE del_yn = 'N' LIMIT @PageSize OFFSET @Offset";
            var dbItems = (await connection.QueryAsync<AdminUserDbModel>(sql, new { PageSize = pageSize, Offset = offset })).ToList();
            var items = dbItems.Select(MapToDomain).ToList();
            var countSql = includeDeleted ? "SELECT COUNT(*) FROM tb_admin" : "SELECT COUNT(*) FROM tb_admin WHERE del_yn = 'N'";
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql);
            return (items, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged AdminUsers. Page: {Page}, PageSize: {PageSize}, IncludeDeleted: {IncludeDeleted}", page, pageSize, includeDeleted);
            throw;
        }
    }

    public async Task SaveAsync(AdminUser adminUser, CancellationToken cancellationToken = default)
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

    // DB 모델 → 도메인 모델 매핑
    private AdminUser MapToDomain(AdminUserDbModel db)
    {
        return new AdminUser
        {
            Aid = db.Aid,
            AccId = db.AccountId,
            AccPwd = db.Password ?? string.Empty,
            Grade = db.Role ?? string.Empty,
            Name = db.Name ?? string.Empty,
            DelYn = db.DelYn ?? "N",
            AccountLocked = db.AccountLocked ?? "0",
            Approved = db.Approved ?? "1",
            Enabled = db.Enabled ?? "1",
        };
    }
}
