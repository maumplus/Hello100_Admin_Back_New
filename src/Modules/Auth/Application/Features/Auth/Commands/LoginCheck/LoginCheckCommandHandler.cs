using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Services;
using Hello100Admin.Modules.Auth.Application.Common.Errors;
using Hello100Admin.Modules.Auth.Application.Common.Extensions;
using Hello100Admin.Modules.Auth.Application.Features.Auth.ReadModels;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.GetUser;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.LoginCheck;
using Hello100Admin.Modules.Auth.Domain.Entities;
using Mapster;
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
        private readonly ICryptoService _cryptoService;

        public LoginCheckCommandHandler(
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IAuthRepository authRepository,
            IAuthStore authStore,
            ILogger<LoginCheckCommandHandler> logger,
            ICryptoService cryptoService)
        {
            _authRepository = authRepository;
            _authStore = authStore;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _logger = logger;
            _cryptoService = cryptoService;
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

            if (adminInfo.Use2fa == "Y"
             && string.IsNullOrWhiteSpace(adminInfo.Email) == true
             && string.IsNullOrWhiteSpace(adminInfo.Tel) == true)
            {
                return Result.Success<LoginCheckResponse>().WithError(AuthErrorCode.NotFoundPhoneAndEmail.ToError());
            }

            var config = this.GetMapsterConfig();

            var response = new LoginCheckResponse()
            {
                User = adminInfo.Adapt<UserResponse>(config)
            };

            response.User.Email = string.IsNullOrWhiteSpace(adminInfo.Email) == false ? _cryptoService.DecryptWithNoVector(adminInfo.Email, CryptoKeyType.Email) : "";
            response.User.Tel = string.IsNullOrWhiteSpace(adminInfo.Tel) == false ? _cryptoService.DecryptWithNoVector(adminInfo.Tel, CryptoKeyType.Mobile) : "";

            return Result.Success(response);
        }

        private TypeAdapterConfig GetMapsterConfig()
        {
            var config = new TypeAdapterConfig();

            config.NewConfig<AdminModel, UserResponse>()
                .Map(d => d.Id, s => s.Aid)
                .Map(d => d.AccountId, s => s.AccId);

            return config;
        }
    }
}
