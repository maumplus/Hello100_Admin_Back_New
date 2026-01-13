using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.Logout;

/// <summary>
/// 로그아웃 커맨드 핸들러
/// </summary>
public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<string>>
{
    private readonly ILogger<LogoutCommandHandler> _logger;
    private readonly IAuthRepository _authRepository;
    private readonly IAuthStore _authStore;

    public LogoutCommandHandler(ILogger<LogoutCommandHandler> logger, IAuthRepository authRepository, IAuthStore authStore)
    {
        _logger = logger;
        _authRepository = authRepository;
        _authStore = authStore;
    }

    public async Task<Result<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // 1. 특정 토큰만 무효화
        if (!string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            var refreshToken = await _authStore.GetByTokenAsync(request.RefreshToken, cancellationToken);
            if (refreshToken != null)
            {
                refreshToken.Revoke();
                await _authRepository.UpdateAsync(refreshToken, cancellationToken);
            }
        }
        // 2. 또는 사용자의 모든 토큰 무효화
        else
        {
            await _authRepository.RevokeUserTokensAsync(request.UserId, cancellationToken);
        }

        // 성공 시 메시지 반환
        return Result.Success("로그아웃 성공");
    }
}
