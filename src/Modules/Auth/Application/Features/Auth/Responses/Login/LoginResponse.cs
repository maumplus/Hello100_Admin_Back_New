using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.GetUser;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.Login;

/// <summary>
/// 로그인 응답 Response
/// </summary>
public class LoginResponse
{
    public UserResponse User { get; set; } = null!;
    public TokenInfo Token { get; set; } = null!;
    public HospitalInfo? Hospital { get; set; }
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

/// <summary>
/// 병원 정보 (Front에서 Default로 가져야할 병원 정보 객체)
/// </summary>
public class HospitalInfo
{
    /// <summary>
    /// 차트타입 [E: 이지스전자차트, N: 닉스펜차트]
    /// </summary>
    public string ChartType { get; set; } = null!;
    /// <summary>
    /// 병원명
    /// </summary>
    public string HospName { get; set; } = null!;
    /// <summary>
    /// 태블릿 수 (헬로데스크 사용)
    /// </summary>
    public int TabletCount { get; set; }
    /// <summary>
    /// 키오스크 수
    /// </summary>
    public int KioskCount { get; set; }
}
