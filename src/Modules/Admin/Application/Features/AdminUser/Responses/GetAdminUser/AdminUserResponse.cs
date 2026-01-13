namespace Hello100Admin.Modules.Admin.Application.Features.AdminUser.Responses.GetAdminUser;

/// <summary>
/// 관리자 사용자 목록 조회용 DTO
/// </summary>
public class AdminUserResponse
{
    public string Id { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;  // acc_id
    public string Name { get; set; } = string.Empty;  // name
    public string? Tel { get; set; }  // tel
    public string? HospNo { get; set; }  // hosp_no
    public string Grade { get; set; } = "C";  // grade
    public bool Enabled { get; set; }  // enabled
    public bool Approved { get; set; }  // approved
    public bool AccountLocked { get; set; }  // account_locked
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public List<string> Roles { get; set; } = new();
}

