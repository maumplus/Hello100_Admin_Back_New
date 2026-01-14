namespace Hello100Admin.Modules.Auth.Infrastructure.Persistence.DbModels.Auth;

/// <summary>
/// DB와 1:1 매핑되는 User DB 모델 (tb_admin)
/// </summary>
internal record UserDbRow
{
    public string AId { get; init; } = default!;
    public string AccId { get; init; } = default!;
    public string AccPwd { get; init; } = default!;
    public string? HospKey { get; init; }
    public string? HospNo { get; init; }
    public string Grade { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string DelYn { get; init; } = default!;
    public int? LastLoginDt { get; init; }
    public string AccountLocked { get; init; } = default!;
    public int LoginFailCount { get; init; }
    public string? RefreshToken { get; init; }
    public string? AccessToken { get; init; }
    public string Approved { get; init; } = "0";
    public string Enabled { get; init; } = "1";
}