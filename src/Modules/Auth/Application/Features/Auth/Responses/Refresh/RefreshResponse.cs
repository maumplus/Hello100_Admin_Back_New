using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.GetUser;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.Refresh
{
    public class RefreshResponse
    {
        public UserResponse User { get; set; } = null!;
        public TokenInfoForRefresh Token { get; set; } = null!;
    }

    /// <summary>
    /// 인증 토큰 DTO
    /// </summary>
    public class TokenInfoForRefresh
    {
        public string AccessToken { get; set; } = string.Empty;
    }
}
