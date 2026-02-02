using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.GetUser;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.Login;

/// <summary>
/// 로그인 응답 Response
/// </summary>
public class LoginResponse
{
    public UserResponse User { get; set; } = null!;
    public TokenInfo Token { get; set; } = null!;
}


/// <summary>
/// 인증 토큰 DTO
/// </summary>
public class TokenInfo
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string TokenType { get; set; } = "Bearer";
}
