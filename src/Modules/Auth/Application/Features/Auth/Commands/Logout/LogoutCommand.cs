using Hello100Admin.BuildingBlocks.Common.Application;
using System.Text.Json.Serialization;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.Logout;

/// <summary>
/// 로그아웃 커맨드
/// </summary>
public record LogoutCommand : ICommand<Result>
{
    [JsonIgnore]
    public string Aid { get; set; } = string.Empty;
}
