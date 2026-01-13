using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.Login;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.Login;

/// <summary>
/// 로그인 커맨드
/// </summary>
public record LoginCommand : ICommand<Result<LoginResponse>>
{
    public string AccountId { get; init; } = string.Empty;  // acc_id
    public string Password { get; init; } = string.Empty;
    public string? IpAddress { get; init; }  // 클라이언트 IP 주소
}
