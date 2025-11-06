using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Auth.Application.DTOs;

namespace Hello100Admin.Modules.Auth.Application.Commands.Login;

/// <summary>
/// 로그인 커맨드
/// </summary>
public record LoginCommand : ICommand<Result<LoginResponseDto>>
{
    public string AccountId { get; init; } = string.Empty;  // acc_id
    public string Password { get; init; } = string.Empty;
    public string? IpAddress { get; init; }  // 클라이언트 IP 주소
}
