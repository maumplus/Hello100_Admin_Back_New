using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.GetUser;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.LoginCheck
{
    /// <summary>
    /// 1차 로그인 응답 Response
    /// </summary>
    public class LoginCheckResponse
    {
        public UserResponse User { get; set; } = null!;
    }
}
