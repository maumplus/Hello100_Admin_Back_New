using Hello100Admin.Modules.Admin.Domain.Entities;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Member
{
    public interface IMemberStore
    {
        /// <summary>
        /// 멤버 조회 (가볍게)
        /// </summary>
        public Task<MemberEntity?> GetByIdAsync(string uid, CancellationToken cancellationToken = default);
        /// <summary>
        /// 멤버 정보 조회
        /// </summary>
        public Task<MemberEntity?> GetMember(string uid, CancellationToken cancellationToken = default);
        /// <summary>
        /// 멤버 가족정보 조회
        /// </summary>
        public Task<IEnumerable<MemberFamilyEntity?>> GetMemberFamilys(string uid, CancellationToken cancellationToken = default);
    }
}
