namespace Hello100Admin.Modules.Auth.Application.DTOs;

/// <summary>
/// 사용자 정보 DTO
/// </summary>
public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;  // acc_id
    public string Name { get; set; } = string.Empty;  // name
    public string? HospNo { get; set; }  // hosp_no
    public string Grade { get; set; } = "C";  // grade
    public bool Enabled { get; set; }  // enabled
    public bool Approved { get; set; }  // approved
    public bool AccountLocked { get; set; }  // account_locked
    public DateTime? LastLoginAt { get; set; }
    public List<string> Roles { get; set; } = new();
}
