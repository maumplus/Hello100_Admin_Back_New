using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Auth.Domain.Interfaces;
using MediatR;

namespace Hello100Admin.Modules.Auth.Application.Commands.Logout;

/// <summary>
/// 로그아웃 커맨드 핸들러
/// </summary>
public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<string>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<Result<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // 1. 특정 토큰만 무효화
        if (!string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
            if (refreshToken != null)
            {
                refreshToken.Revoke();
                await _refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);
            }
        }
        // 2. 또는 사용자의 모든 토큰 무효화
        else
        {
            await _refreshTokenRepository.RevokeUserTokensAsync(request.UserId, cancellationToken);
        }

        // 성공 시 메시지 반환
        return Result.Success<string>("로그아웃 성공");
    }
}
