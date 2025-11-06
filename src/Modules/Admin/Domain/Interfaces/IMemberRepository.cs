using Hello100Admin.Modules.Admin.Domain.Entities;

namespace Hello100Admin.Modules.Admin.Domain.Interfaces;

/// <summary>
/// 멤버 리포지토리 인터페이스
/// </summary>
public interface IMemberRepository
{
    /// <summary>
    /// 멤버 조회 (가볍게)
    /// </summary>
    Task<Member?> GetByIdAsync(string uid, CancellationToken cancellationToken = default);
    /// <summary>
    /// 멤버 정보 조회
    /// </summary>
    Task<Member?> GetMember(string uid, CancellationToken cancellationToken = default);
    /// <summary>
    /// 멤버 가족정보 조회
    /// </summary>
    Task<IEnumerable<MemberFamily?>> GetMemberFamilys(string uid, CancellationToken cancellationToken = default);
}