using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.Login;
using System.Text.Json.Serialization;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.Login;

/// <summary>
/// 로그인 커맨드
/// </summary>
public record LoginCommand : ICommand<Result<LoginResponse>>
{
    public string AccountId { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    [JsonIgnore]
    public string AppCd { get; set; } = "H02";
    public int AuthId { get; set; }
    public string AuthNumber { get; set; } = string.Empty;
    [JsonIgnore]
    public string? UserAgent { get; init; } = string.Empty;
    [JsonIgnore]
    public string? IpAddress { get; init; } = string.Empty;
}
