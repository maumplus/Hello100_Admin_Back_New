using Dapper;
using Hello100Admin.BuildingBlocks.Common.Domain;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Domain.Interfaces;
using Hello100Admin.Modules.Admin.Infrastructure.Models;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<MemberRepository> _logger;

    public MemberRepository(IDbConnectionFactory connectionFactory, ILogger<MemberRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }


    public async Task<Member?> GetByIdAsync(
        string uid,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting Member by Uid: {Uid}", uid);
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM VM_USERS VM WHERE Uid = @Uid";
            var dbMember = await connection.QueryFirstOrDefaultAsync<MemberDbModel>(sql, new { Uid = uid });
            if (dbMember == null)
            {
                _logger.LogWarning("No Member found for Uid: {Uid}", uid);
                return null;
            }
            return MapToDomain(dbMember);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Member by Uid: {Uid}", uid);
            throw;
        }
    }

    /// <summary>
    /// VM_USERS 뷰 기반 멤버 상세 조회 (레거시 쿼리 참고)
    /// </summary>

    public async Task<Member?> GetMember(string uid, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting detailed Member by Uid: {Uid}", uid);
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT *
                FROM VM_USERS VM
                WHERE uid = @Uid
                    AND CASE WHEN last_login_dt_view_new IS NULL THEN
                        last_login_dt = (SELECT MAX(last_login_dt) FROM VM_USERS WHERE UID = @Uid)
                    ELSE
                        last_login_dt_view_new = (SELECT MAX(last_login_dt_view_new) FROM VM_USERS WHERE UID = @Uid)
                    END
            ";
            var dbMember = await connection.QueryFirstOrDefaultAsync<MemberDbModel>(sql, new { Uid = uid });
            if (dbMember == null)
            {
                _logger.LogWarning("No detailed Member found for Uid: {Uid}", uid);
                return null;
            }
            return MapToDomain(dbMember);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting detailed Member by Uid: {Uid}", uid);
            throw;
        }
    }
    
    /// <summary>
    /// tb_member 테이블 기반 멤버 가족정보 조회
    /// </summary>

    public async Task<IEnumerable<MemberFamily?>> GetMemberFamilys(string uid, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting Member families for Uid: {Uid}", uid);
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT *
                FROM tb_member
                WHERE uid = @Uid
                    AND del_yn='N'
            ";
            var dbMemberFamilys = await connection.QueryAsync<MemberFamilyDbModel>(sql, new { Uid = uid });
            if (dbMemberFamilys == null)
            {
                _logger.LogWarning("No Member families found for Uid: {Uid}", uid);
                return Enumerable.Empty<MemberFamily?>();
            }
            return dbMemberFamilys.Select(MapToDomain);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Member families for Uid: {Uid}", uid);
            throw;
        }
    }

    // DB 모델 → 도메인 모델 매핑
    private Member MapToDomain(MemberDbModel db)
    {
        var member = new Member
        {
            Uid = db.Uid,
            Name = EncryptedData.FromEncrypted(db.Name, CryptoKeyType.Name),
            SnsId = db.SnsId != null ? EncryptedData.FromEncrypted(db.SnsId) : null,
            Email = db.Email != null ? EncryptedData.FromEncrypted(db.Email, CryptoKeyType.Email) : null,
            LoginType = db.LoginType,
            LoginTypeName = db.LoginTypeName,
            Said = db.Said,
            RegDt = DateTimeOffset.FromUnixTimeSeconds(db.RegDt).UtcDateTime,
            RegDtView = db.RegDtView ?? string.Empty,
            UserRole = db.UserRole,
            LastLoginDt = DateTimeOffset.FromUnixTimeSeconds(db.LastLoginDt).UtcDateTime,
            LastLoginDtView = db.LastLoginDtView ?? string.Empty,
            LastLoginDtViewNew = db.LastLoginDtViewNew ?? string.Empty
        };
        return member;
    }

    private MemberFamily MapToDomain(MemberFamilyDbModel db)
    {
        var memberFamily = new MemberFamily
        {
            Uid = db.Uid,
            Mid = db.Mid,
            Name = EncryptedData.FromEncrypted(db.Name, CryptoKeyType.Name),
            Sex = EncryptedData.FromEncrypted(db.Sex),
            Birthday = EncryptedData.FromEncrypted(db.Birthday),
            RegDt = DateTimeOffset.FromUnixTimeSeconds(db.RegDt).UtcDateTime
        };
        return memberFamily;
    }
}