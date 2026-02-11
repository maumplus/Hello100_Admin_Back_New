using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Services;
using Hello100Admin.Modules.Auth.Application.Common.Errors;
using Hello100Admin.Modules.Auth.Application.Common.Extensions;
using Hello100Admin.Modules.Auth.Application.Common.Models;
using Hello100Admin.Modules.Auth.Application.Features.Auth.ReadModels;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.GetUser;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.Login;
using Hello100Admin.Modules.Auth.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.Login
{
    /// <summary>
    /// 로그인 커맨드
    /// </summary>
    public record LoginCommand : ICommand<Result<LoginResponse>>
    {
        public string AccountId { get; init; } = default!;
        public string Password { get; init; } = default!;
        public string AppCd { get; set; } = "H02";
        public int AuthId { get; set; }
        public string AuthNumber { get; set; } = default!;
        public string? UserAgent { get; init; } = default!;
        public string? IpAddress { get; init; } = default!;
    }

    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.AccountId)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("계정 아이디는 필수입니다.");
            RuleFor(x => x.Password)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("비밀번호는 필수입니다.");
            RuleFor(x => x.UserAgent)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("유저 에이전트는 필수입니다.");
            RuleFor(x => x.IpAddress)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("IP는 필수입니다.");
        }
    }

    /// <summary>
    /// 로그인 커맨드 핸들러
    /// </summary>
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
    {
        private readonly IAuthRepository _authRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IAuthStore _authStore;
        private readonly ILogger<LoginCommandHandler> _logger;
        private readonly ICryptoService _cryptoService;
        private readonly IDbSessionRunner _db;

        public LoginCommandHandler(
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IAuthRepository authRepository,
            IAuthStore authStore,
            ILogger<LoginCommandHandler> logger,
            ICryptoService cryptoService,
            IDbSessionRunner db)
        {
            _authRepository = authRepository;
            _authStore = authStore;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _logger = logger;
            _cryptoService = cryptoService;
            _db = db;
        }

        public async Task<Result<LoginResponse>> Handle(LoginCommand req, CancellationToken ct)
        {
            var adminInfo = await _authStore.GetAdminByAccIdAsync(req.AccountId, ct);

            if (adminInfo == null)
            {
                return Result.Success<LoginResponse>().WithError(GlobalErrorCode.AuthFailed.ToError());
            }

            if (adminInfo.Use2fa == "Y"
             && string.IsNullOrWhiteSpace(adminInfo.Email) == true
             && string.IsNullOrWhiteSpace(adminInfo.Tel) == true)
            {
                return Result.Success<LoginResponse>().WithError(AuthErrorCode.NotFoundPhoneAndEmail.ToError());
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

            if (!_passwordHasher.VerifyPassword(adminInfo.AccPwd, req.Password, adminInfo.Aid))
            {
                return Result.Success<LoginResponse>().WithError(GlobalErrorCode.AuthFailed.ToError());
            }

            if (adminInfo.Use2fa == "Y")
            {
                var appAuthNumberInfo = await _authStore.GetAppAuthNumberInfoAsync(req.AuthId);

                if (appAuthNumberInfo == null)
                {
                    return Result.Success<LoginResponse>().WithError(GlobalErrorCode.InvalidVerificationCode.ToError());
                }

                if (!_passwordHasher.VerifyAuthNumber(appAuthNumberInfo.AuthNumber, req.AuthNumber, req.AppCd))
                {
                    return Result.Success<LoginResponse>().WithError(GlobalErrorCode.InvalidVerificationCode.ToError());
                }

                try
                {
                    await _authRepository.UpdateConfirmAsync(appAuthNumberInfo);
                }
                catch
                {
                    return Result.Success<LoginResponse>().WithError(GlobalErrorCode.InvalidVerificationCode.ToError());
                }
            }

            var roles = new[] { GetRoleNameByGrade(adminInfo.Grade) };

            var accessToken = _tokenService.GenerateAccessToken(adminInfo, roles);
            var refreshToken = _tokenService.GenerateRefreshToken(adminInfo.Aid, req.IpAddress);

            adminEntity.AccessToken = accessToken;
            adminEntity.RefreshToken = refreshToken.Token;

            await _authRepository.UpdateTokensAsync(adminEntity, ct);

            var adminLog = new AdminLogEntity()
            {
                Aid = adminInfo.Aid,
                UserAgent = req.UserAgent!,
                IP = req.IpAddress!
            };

            await _authRepository.InsertAdminLogAsync(adminLog, ct);

            // 병원관리자 한정 병원정보 조회
            CurrentHospitalInfo? hospInfo = null;
            if (adminInfo.Grade == GlobalConstant.AdminRoles.HospitalAdmin && string.IsNullOrWhiteSpace(adminInfo.HospNo) == false)
            {
                hospInfo = await _db.RunAsync(DataSource.Hello100,
                    (session, token) => _authStore.GetHospitalInfoByHospNoAsync(session, adminInfo.HospNo, token),
                ct);
            }

            var config = this.GetMapsterConfig();

            var response = new LoginResponse()
            {
                User = adminInfo.Adapt<UserResponse>(config),
                Hospital = hospInfo.Adapt<HospitalInfo>(),
                Token = new TokenInfo
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                    ExpiresAt = refreshToken.ExpiresAt
                }
            };

            response.User.Email = string.IsNullOrWhiteSpace(adminInfo.Email) == false ? _cryptoService.DecryptWithNoVector(adminInfo.Email, CryptoKeyType.Email) : "";
            response.User.Tel = string.IsNullOrWhiteSpace(adminInfo.Tel) == false ? _cryptoService.DecryptWithNoVector(adminInfo.Tel, CryptoKeyType.Mobile) : "";

            return Result.Success(response);
        }

        private string GetRoleNameByGrade(string grade) => grade switch
        {
            GlobalConstant.AdminRoles.SuperAdmin => nameof(GlobalConstant.AdminRoles.SuperAdmin),
            GlobalConstant.AdminRoles.HospitalAdmin => nameof(GlobalConstant.AdminRoles.HospitalAdmin),
            GlobalConstant.AdminRoles.GeneralAdmin => nameof(GlobalConstant.AdminRoles.GeneralAdmin),
            _ => nameof(GlobalConstant.AdminRoles.GeneralAdmin)
        };

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
