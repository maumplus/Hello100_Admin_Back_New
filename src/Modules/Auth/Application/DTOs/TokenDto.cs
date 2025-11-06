namespace Hello100Admin.Modules.Auth.Application.DTOs;

/// <summary>
/// 인증 토큰 DTO
/// </summary>
public class TokenDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string TokenType { get; set; } = "Bearer";
}
