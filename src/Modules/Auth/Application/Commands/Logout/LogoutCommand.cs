using Hello100Admin.BuildingBlocks.Common.Application;

namespace Hello100Admin.Modules.Auth.Application.Commands.Logout;

/// <summary>
/// 로그아웃 커맨드
/// </summary>
public record LogoutCommand : ICommand<Result<string>>
{
    public string UserId { get; init; } = string.Empty;
    public string? RefreshToken { get; init; }
}
