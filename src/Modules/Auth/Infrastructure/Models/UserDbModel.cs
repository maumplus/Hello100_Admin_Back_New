namespace Hello100Admin.Modules.Auth.Infrastructure.Models;

/// <summary>
/// DB와 1:1 매핑되는 User DB 모델 (tb_admin)
/// </summary>
public class UserDbModel
{
    public string Aid { get; set; } = string.Empty;
    public string AccId { get; set; } = string.Empty;
    public string? AccPwd { get; set; }
    public string? HospNo { get; set; }
    public string Grade { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DelYn { get; set; } = string.Empty;
    public int? LastLoginDt { get; set; }
    public string AccountLocked { get; set; } = "0";
    public int LoginFailCount { get; set; }
    public string? RefreshToken { get; set; }
    public string? AccessToken { get; set; }
    public string Approved { get; set; } = "0";
    public string Enabled { get; set; } = "1";
}