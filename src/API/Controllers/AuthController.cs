using MediatR;
using Hello100Admin.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hello100Admin.API.Infrastructure.Attributes;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.Login;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.Logout;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Queries.GetUser;

namespace Hello100Admin.API.Controllers;

/// <summary>
/// 인증 관련 API 컨트롤러
/// </summary>
public class AuthController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// 로그인
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        _logger.LogInformation("Login attempt for AccountId: {AccountId} from IP: {IpAddress}",
            command.AccountId, GetClientIpAddress());

        // 클라이언트 IP 추출
        var ipAddress = GetClientIpAddress();
        var commandWithIp = command with { IpAddress = ipAddress };

        var result = await _mediator.Send(commandWithIp);

        _logger.LogInformation("User {AccountId} logged in process completed", command.AccountId);

        // 중앙화된 매퍼 사용: 성공/실패 모두 ToActionResult에서 처리합니다.
        return result.ToActionResult(this, authEndpoint: true);
    }

    /// <summary>
    /// 토큰 갱신
    /// </summary>
    // [HttpPost("refresh")]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    // {
    //     _logger.LogInformation("Token refresh attempt");

    //     var result = await _mediator.Send(command);

    //     if (!result.IsSuccess)
    //     {
    //         _logger.LogWarning("Token refresh failed: {Error}", result.Error);
    //         return BadRequest(new { message = result.Error });
    //     }

    //     _logger.LogInformation("Token refreshed successfully");
    //     return Ok(result.Value);
    // }

    /// <summary>
    /// 로그아웃
    /// </summary>
    [HttpPost("logout")]
    [Auth]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        _logger.LogInformation("Logout attempt for UserId: {UserId}", userId);

        var result = await _mediator.Send(command);

        // 이제 Logout 핸들러는 Result<string>을 반환하므로
        // 제네릭 ToActionResult 경로로 통일하여 성공/실패를 중앙에서 처리합니다.
        return result.ToActionResult(this);
    }
    
    /// <summary>
    /// 현재 사용자 정보 조회
    /// </summary>
    [HttpGet("me")]
    [Auth]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMe()
    {
        if (string.IsNullOrWhiteSpace(base.AId) == true)
        {
            return Unauthorized();
        }

        var query = new GetUserQuery { AId = base.AId };
        var result = await _mediator.Send(query);

        // 제네릭 Result<UserDto> 경로는 ToActionResult에서 성공/실패를 모두 처리합니다.
        return result.ToActionResult(this);
    }

    /// <summary>
    /// 클라이언트 IP 주소 추출
    /// </summary>
    private string? GetClientIpAddress()
    {
        // X-Forwarded-For 헤더 확인 (프록시/로드밸런서 환경)
        var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',').FirstOrDefault()?.Trim();
        }

        // X-Real-IP 헤더 확인 (Nginx 등)
        var realIp = Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        // HttpContext의 RemoteIpAddress 사용
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
