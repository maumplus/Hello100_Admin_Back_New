using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.Login;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.LoginCheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.LoginCheck
{
    /// <summary>
    /// 로그인 커맨드
    /// </summary>
    public record LoginCheckCommand : ICommand<Result<LoginCheckResponse>>
    {
        public string AccountId { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        [JsonIgnore]
        public string? IpAddress { get; init; } = string.Empty;
    }
}
