namespace Hello100Admin.Modules.Auth.Application.DTOs;

/// <summary>
/// 로그인 응답 DTO
/// </summary>
public class LoginResponseDto
{
    public UserDto User { get; set; } = null!;
    public TokenDto Token { get; set; } = null!;
}
