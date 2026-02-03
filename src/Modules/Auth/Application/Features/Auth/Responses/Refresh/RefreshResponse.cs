using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.GetUser;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.Refresh
{
    public class RefreshResponse
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
    }
}
