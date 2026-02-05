using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.Refresh;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Services;
using Hello100Admin.Modules.Auth.Domain.Entities;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.GetUser;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.Refresh
{
    public class RefreshCommandHandler : IRequestHandler<RefreshCommand, Result<RefreshResponse>>
    {
        private readonly ILogger<RefreshCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IAuthRepository _authRepository;
        private readonly IAuthStore _authStore;
        private readonly IConfiguration _config;

        public RefreshCommandHandler(ILogger<RefreshCommandHandler> logger, ITokenService tokenService, IAuthRepository authRepository, IAuthStore authStore, IConfiguration config)
        {
            _logger = logger;
            _tokenService = tokenService;
            _authRepository = authRepository;
            _authStore = authStore;
            _config = config;
        }

        public async Task<Result<RefreshResponse>> Handle(RefreshCommand request, CancellationToken cancellationToken)
        {
            var adminInfo = await _authStore.GetAdminByAidAsync(request.Aid);

            if (adminInfo == null)
            {
                return Result.Success<RefreshResponse>().WithError(GlobalErrorCode.AuthFailed.ToError());
            }

            var adminEntity = new AdminEntity()
            {
                Aid = adminInfo.Aid,
                AccId = adminInfo.AccId,
                AccPwd = adminInfo.AccPwd,
                Grade = adminInfo.Grade,
                Name = adminInfo.Name,
                DelYn = "N",
                AccountLocked = adminInfo.AccountLocked,
                LoginFailCount = adminInfo.LoginFailCount
            };

            var roles = new[] { GetRoleNameByGrade(adminInfo.Grade) };

            var accessToken = _tokenService.GenerateAccessToken(adminInfo, roles);

            adminEntity.AccessToken = accessToken;

            await _authRepository.UpdateAccessTokenAsync(adminEntity, cancellationToken);

            var response = new RefreshResponse
            {
                User = new UserResponse
                {
                    Id = adminInfo.Aid,
                    AccountId = adminInfo.AccId,
                    Name = adminInfo.Name,
                    Grade = adminInfo.Grade,
                    AccountLocked = adminInfo.AccountLocked,
                    LastLoginDt = adminInfo.LastLoginDt,
                    Use2fa = adminInfo.Use2fa
                },
                Token = new TokenInfoForRefresh
                {
                    AccessToken = accessToken
                }
            };

            return Result.Success<RefreshResponse>(response);
        }

        private string GetRoleNameByGrade(string grade) => grade switch
        {
            "S" => "SuperAdmin",
            "C" => "HospitalAdmin",
            "A" => "GeneralAdmin",
            _ => "GeneralAdmin"
        };
    }
}
