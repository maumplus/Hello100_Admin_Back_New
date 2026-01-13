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
        _logger.LogDebug("Processing login for AccountId: {AccountId} from IP: {IpAddress}", 
            request.AccountId, request.IpAddress);

        // 1. 사용자 조회 (AccountId)
        var user = await _authStore.GetByUsernameAsync(request.AccountId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Login failed: User not found for AccountId: {AccountId}", request.AccountId);
            // return Result.Failure<LoginResponseDto>("계정 ID 또는 비밀번호가 올바르지 않습니다.");
            return Result.SuccessWithError<LoginResponse>(GlobalErrorCode.AuthFailed.ToError());
        }

        // 2. 계정 상태 확인 (CanLogin 헬퍼 메서드 사용)
        // if (!user.CanLogin())
        // {
        //     string reason = user.DelYn == "Y" ? "삭제된 계정입니다." :
        //                    user.Approved == "0" ? "승인되지 않은 계정입니다." :
        //                    user.Enabled == "0" ? "비활성화된 계정입니다." :
        //                    user.AccountLocked == "1" ? "계정이 잠겨 있습니다. 관리자에게 문의하세요." :
        //                    "로그인할 수 없는 계정입니다.";
            
        //     _logger.LogWarning("Login failed: {Reason} for UserId: {UserId}, AccountId: {AccountId}", 
        //         reason, user.Id, request.AccountId);
        //     return Result.Failure<LoginResponseDto>(reason);
        // }

        // 3. 비밀번호 검증 (SHA256 + Salt(aid))
        if (!_passwordHasher.VerifyPassword(user.AccPwd, request.Password, user.Aid))
        {
            // 실패 관련 동작은 이게 아닌듯?
            // _logger.LogWarning("Login failed: Invalid password for UserId={UserId}, AccountId={AccountId}, FailCount={FailCount}", 
            //     user.Id, request.AccountId, user.LoginFailCount + 1);

            // // 로그인 실패 기록
            // user.RecordLoginFailure();
            // await _userRepository.UpdateAsync(user, cancellationToken);

            _logger.LogWarning("Login failed: Invalid password for UserId: {UserId}, AccountId: {AccountId}",
                user.Id, request.AccountId);

            user.RecordLoginFailure();
            await _authRepository.UpdateLoginFailureAsync(user, cancellationToken);

            return Result.SuccessWithError<LoginResponse>(GlobalErrorCode.AuthFailed.ToError());
        }

        _logger.LogInformation("Password verified successfully for UserId: {UserId}, AccountId: {AccountId}", 
            user.Id, request.AccountId);

        // 4. 로그인 기록
        user.RecordLogin();

        // 8. Grade 기반 역할 설정
        var roleNames = new[] { GetRoleNameByGrade(user.Grade) };

        // 9. 토큰 생성
        var accessToken = _tokenService.GenerateAccessToken(user, roleNames);
        var refreshToken = _tokenService.GenerateRefreshToken(user.Aid, request.IpAddress);  // string aid

        // 10. User 테이블에 토큰 저장
        user.RefreshToken = refreshToken.Token;

        await _authRepository.UpdateLoginSuccessAsync(user, cancellationToken);

        _logger.LogInformation("Login successful for Aid: {Aid}, AccountId: {AccountId}, Grade: {Grade}, Role: {Role}",
            user.Aid, request.AccountId, user.Grade, roleNames[0]);

        // 5. 응답 생성
        var response = new LoginResponse
        {
            User = new UserResponse
            {
                Id = user.Aid,
                AccountId = user.AccId,
                Name = user.Name,
                Grade = user.Grade,
                Enabled = user.Enabled == "1",
                Approved = user.Approved == "1",
                AccountLocked = user.AccountLocked == "1",
                LastLoginAt = user.LastLoginDt,
                Roles = roleNames.ToList()
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
