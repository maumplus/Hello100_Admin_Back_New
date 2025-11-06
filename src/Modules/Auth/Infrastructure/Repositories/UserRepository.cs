using System.Data;
using Dapper;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence;
using Hello100Admin.Modules.Auth.Domain.Entities;
using Hello100Admin.Modules.Auth.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Auth.Infrastructure.Repositories;

/// <summary>
/// Dapper 기반 UserRepository 구현체
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(IDbConnectionFactory connectionFactory, ILogger<UserRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    private IDbConnection CreateConnection()
    {
        var conn = _connectionFactory.CreateConnection();
        if (conn == null)
            throw new InvalidOperationException("IDbConnectionFactory returned null connection.");
        return conn;
    }

    public async Task<User?> GetByAidAsync(string aid, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting User by Aid: {Aid}", aid);
            using var connection = CreateConnection();
            var sql = "SELECT * FROM tb_admin WHERE aid = @Aid LIMIT 1";
            var dbUser = await connection.QueryFirstOrDefaultAsync<Models.UserDbModel>(sql, new { Aid = aid });
            if (dbUser == null)
            {
                _logger.LogWarning("No User found for Aid: {Aid}", aid);
                return null;
            }
            return MapToDomain(dbUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting User by Aid: {Aid}", aid);
            throw;
        }
    }

    public async Task<User?> GetByUsernameAsync(string accId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting User by AccId: {AccId}", accId);
            using var connection = CreateConnection();
            var sql = "SELECT * FROM tb_admin WHERE acc_id = @AccId LIMIT 1";
            var dbUser = await connection.QueryFirstOrDefaultAsync<Models.UserDbModel>(sql, new { AccId = accId });
            if (dbUser == null)
            {
                _logger.LogWarning("No User found for AccId: {AccId}", accId);
                return null;
            }
            return MapToDomain(dbUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting User by AccId: {AccId}", accId);
            throw;
        }
    }
    // DB 모델 → 도메인 모델 매핑
    private User MapToDomain(Models.UserDbModel db)
    {
        return new User
        {
            Aid = db.Aid,
            AccId = db.AccId,
            AccPwd = db.AccPwd ?? string.Empty,
            HospNo = db.HospNo,
            Grade = db.Grade,
            Name = db.Name,
            DelYn = db.DelYn,
            LastLoginDt = DateTimeOffset.FromUnixTimeSeconds(db.LastLoginDt ?? 0).DateTime,
            AccountLocked = db.AccountLocked,
            LoginFailCount = db.LoginFailCount,
            RefreshToken = db.RefreshToken,
            AccessToken = db.AccessToken,
            Approved = db.Approved,
            Enabled = db.Enabled
        };
        }
    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating User. Aid: {Aid}", user.Aid);
            using var connection = CreateConnection();
            var sql = @"UPDATE tb_admin SET acc_id = @AccId, password_hash = @PasswordHash, salt = @Salt, roles = @Roles, is_active = @IsActive, mod_dt = @ModDt WHERE aid = @Aid";
            await connection.ExecuteAsync(sql, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating User. Aid: {Aid}", user.Aid);
            throw;
        }
    }

    public async Task UpdateLoginSuccessAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating login success for User. Aid: {Aid}", user.Aid);
            using var connection = CreateConnection();
            var sql = @"UPDATE tb_admin SET last_login_dt = @LastLoginDt, login_fail_count = 0, account_locked = @AccountLocked, refresh_token = @RefreshToken WHERE aid = @Aid";
            var dbUser = ToDbModel(user);
            await connection.ExecuteAsync(sql, new {
                Aid = dbUser.Aid,
                LastLoginDt = dbUser.LastLoginDt,
                AccountLocked = dbUser.AccountLocked,
                RefreshToken = dbUser.RefreshToken,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating login success for User. Aid: {Aid}", user.Aid);
            throw;
        }
    }
    // 도메인 User → DB 모델 변환
    private Models.UserDbModel ToDbModel(User user)
    {
        return new Models.UserDbModel
        {
            Aid = user.Aid,
            AccId = user.AccId,
            AccPwd = user.AccPwd,
            HospNo = user.HospNo,
            Grade = user.Grade,
            Name = user.Name,
            DelYn = user.DelYn,
            LastLoginDt = user.LastLoginDt.HasValue ? (int)new DateTimeOffset(user.LastLoginDt.Value).ToUnixTimeSeconds() : (int?)null,
            AccountLocked = user.AccountLocked,
            LoginFailCount = user.LoginFailCount,
            RefreshToken = user.RefreshToken,
            AccessToken = user.AccessToken,
            Approved = user.Approved,
            Enabled = user.Enabled
        };
    }

    public async Task UpdateLoginFailureAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating login failure for User. Aid: {Aid}", user.Aid);
            using var connection = CreateConnection();
            var sql = @"UPDATE tb_admin SET login_fail_count = @LoginFailCount, account_locked = @AccountLocked WHERE aid = @Aid";
            await connection.ExecuteAsync(sql, new {
                user.LoginFailCount,
                user.AccountLocked,
                user.Aid
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating login failure for User. Aid: {Aid}", user.Aid);
            throw;
        }
    }

    public async Task UpdateTokensAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating tokens for User. Aid: {Aid}", user.Aid);
            using var connection = CreateConnection();
            var sql = @"UPDATE tb_admin SET refresh_token = @RefreshToken, mod_dt = @ModDt WHERE aid = @Aid";
            await connection.ExecuteAsync(sql, new {
                user.RefreshToken,
                user.ModDt,
                user.Aid
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tokens for User. Aid: {Aid}", user.Aid);
            throw;
        }
    }
}

