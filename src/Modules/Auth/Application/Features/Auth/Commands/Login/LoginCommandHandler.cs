using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Services;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.GetUser;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.Login;
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

    public LoginCommandHandler(
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IAuthRepository authRepository,
        IAuthStore authStore,
        ILogger<LoginCommandHandler> logger)
    {
        _authRepository = authRepository;
        _authStore = authStore;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var adminInfo = await _authStore.GetAdminByAccIdAsync(request.AccId, cancellationToken);
        
        if (adminInfo == null)
        {
            return Result.Success<LoginResponse>().WithError(GlobalErrorCode.AuthFailed.ToError());
        }

        if (!_passwordHasher.VerifyPassword(adminInfo.AccPwd, request.Password, adminInfo.Aid))
        {
            adminInfo.RecordLoginFailure();
            await _authRepository.UpdateLoginFailureAsync(adminInfo, cancellationToken);

            return Result.Success<LoginResponse>().WithError(GlobalErrorCode.AuthFailed.ToError());
        }

        var accessToken = _tokenService.GenerateAccessToken(adminInfo);
        var refreshToken = _tokenService.GenerateRefreshToken(adminInfo.Aid, request.IpAddress);

        // 10. User 테이블에 토큰 저장
        adminInfo.RefreshToken = refreshToken.Token;

        await _authRepository.UpdateLoginSuccessAsync(adminInfo, cancellationToken);

        // 5. 응답 생성
        var response = new LoginResponse
        {
            User = new UserResponse
            {
                Aid = adminInfo.Aid,
                AccId = adminInfo.AccId,
                Name = adminInfo.Name,
                Grade = adminInfo.Grade,
                AccountLocked = adminInfo.AccountLocked,
                LastLoginDt = adminInfo.LastLoginDtStr,
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
