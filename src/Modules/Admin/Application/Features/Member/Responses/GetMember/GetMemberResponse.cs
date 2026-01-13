namespace Hello100Admin.Modules.Admin.Application.Features.Member.Responses.GetMember;

/// <summary>
/// 멤버 DTO
/// </summary>
public class GetMemberResponse
{
    public required string Uid { get; set; }
    public int Mid { get; set; }
    public required string Name { get; set; }
    public string? SnsId { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public required string LoginType { get; set; }
    public required string LoginTypeName { get; set; }
    public long Said { get; set; }
    public required int RegDt { get; set; }
    public required string RegDtView { get; set; }
    public long LastLoginDt { get; set; }
    public string? LastLoginDtView { get; set; }
    public string? LastLoginDtViewNew { get; set; }
    /// <summary>
    /// 사용자권한 (0:일반사용자, 1:테스트사용자)
    /// </summary>
    public int UserRole { get; set; }
    public List<MemberFamilyDto> MemberFamilys { get; set; } = new();
}

/// <summary>
/// 멤버 가족정보 DTO
/// </summary>
public class MemberFamilyDto
{
    public required string Uid { get; set; }
    public int Mid { get; set; }
    public required string Name { get; set; }
    public required string Birthday { get; set; }
    public required string Sex { get; set; }
    public required string RegDt { get; set; }
}
