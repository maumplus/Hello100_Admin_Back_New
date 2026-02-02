using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Services;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.GetUser;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.Login;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.LoginCheck;
using Hello100Admin.Modules.Auth.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.LoginCheck
{
    /// <summary>
    /// 로그인 커맨드 핸들러
    /// </summary>
    public class LoginCheckCommandHandler : IRequestHandler<LoginCheckCommand, Result<LoginCheckResponse>>
    {
        private readonly IAuthRepository _authRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IAuthStore _authStore;
        private readonly ILogger<LoginCheckCommandHandler> _logger;

        public LoginCheckCommandHandler(
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IAuthRepository authRepository,
            IAuthStore authStore,
            ILogger<LoginCheckCommandHandler> logger)
        {
            _authRepository = authRepository;
            _authStore = authStore;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<Result<LoginCheckResponse>> Handle(LoginCheckCommand request, CancellationToken cancellationToken)
        {
            var adminInfo = await _authStore.GetAdminByAccIdAsync(request.AccountId, cancellationToken);

            if (adminInfo == null)
            {
                return Result.Success<LoginCheckResponse>().WithError(GlobalErrorCode.AuthFailed.ToError());
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

            if (!_passwordHasher.VerifyPassword(adminInfo.AccPwd, request.Password, adminInfo.Aid))
            {
                adminInfo.LoginFailCount++;

                if (adminInfo.LoginFailCount >= 5)  // 5회 실패 시 계정 잠금
                {
                    adminInfo.AccountLocked = "Y";
                }

                await _authRepository.UpdateLoginFailureAsync(adminEntity, cancellationToken);

                return Result.Success<LoginCheckResponse>().WithError(GlobalErrorCode.AuthFailed.ToError());
            }

            await _authRepository.UpdateLoginSuccessAsync(adminEntity, cancellationToken);


            var response = new LoginCheckResponse
            {
                User = new UserResponse
                {
                    Id = adminInfo.Aid,
                    AccountId = adminInfo.AccId,
                    Name = adminInfo.Name,
                    Grade = adminInfo.Grade,
                    AccountLocked = adminInfo.AccountLocked,
                    LastLoginDt = adminInfo.LastLoginDt,
                }
            };

            return Result.Success(response);
        }
    }
}
