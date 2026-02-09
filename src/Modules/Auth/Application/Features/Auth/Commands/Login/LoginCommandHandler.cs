using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Services;
using Hello100Admin.Modules.Auth.Application.Common.Errors;
using Hello100Admin.Modules.Auth.Application.Common.Extensions;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.GetUser;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.Login;
using Hello100Admin.Modules.Auth.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.Login;

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

    public LoginCommandHandler(
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IAuthRepository authRepository,
        IAuthStore authStore,
        ILogger<LoginCommandHandler> logger,
        ICryptoService cryptoService)
    {
        _authRepository = authRepository;
        _authStore = authStore;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _logger = logger;
        _cryptoService = cryptoService;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var adminInfo = await _authStore.GetAdminByAccIdAsync(request.AccountId, cancellationToken);
        
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

        if (!_passwordHasher.VerifyPassword(adminInfo.AccPwd, request.Password, adminInfo.Aid))
        {
            return Result.Success<LoginResponse>().WithError(GlobalErrorCode.AuthFailed.ToError());
        }

        if (adminInfo.Use2fa == "Y")
        {
            var appAuthNumberInfo = await _authStore.GetAppAuthNumberInfoAsync(request.AuthId);

            if (appAuthNumberInfo == null)
            {
                return Result.Success<LoginResponse>().WithError(GlobalErrorCode.InvalidVerificationCode.ToError());
            }

            if (!_passwordHasher.VerifyAuthNumber(appAuthNumberInfo.AuthNumber, request.AuthNumber, request.AppCd))
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
        var refreshToken = _tokenService.GenerateRefreshToken(adminInfo.Aid, request.IpAddress);

        adminEntity.AccessToken = accessToken;
        adminEntity.RefreshToken = refreshToken.Token;

        await _authRepository.UpdateTokensAsync(adminEntity, cancellationToken);

        var adminLog = new AdminLogEntity()
        {
            Aid = adminInfo.Aid,
            UserAgent = request.UserAgent,
            IP = request.IpAddress
        };
        await _authRepository.InsertAdminLogAsync(adminLog, cancellationToken);

        var response = new LoginResponse
        {
            User = new UserResponse
            {
                Id = adminInfo.Aid,
                AccountId = adminInfo.AccId,
                Name = adminInfo.Name,
                Grade = adminInfo.Grade,
                HospNo = adminInfo.HospNo,
                AccountLocked = adminInfo.AccountLocked,
                LastLoginDt = adminInfo.LastLoginDt,
                Use2fa = adminInfo.Use2fa,
                Email = string.IsNullOrWhiteSpace(adminInfo.Email) == false ? _cryptoService.DecryptWithNoVector(adminInfo.Email, CryptoKeyType.Email) : "",
                Tel = string.IsNullOrWhiteSpace(adminInfo.Tel) == false ? _cryptoService.DecryptWithNoVector(adminInfo.Tel, CryptoKeyType.Mobile) : ""
            },
            Token = new TokenInfo
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = refreshToken.ExpiresAt
            }
        };

        return Result.Success(response);
    }

    private string GetRoleNameByGrade(string grade) => grade switch
    {
        "S" => "SuperAdmin",
        "C" => "HospitalAdmin",
        "A" => "GeneralAdmin",
        _ => "GeneralAdmin"
    };
}
