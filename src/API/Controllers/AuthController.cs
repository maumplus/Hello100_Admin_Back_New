using MediatR;
using Hello100Admin.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hello100Admin.API.Infrastructure.Attributes;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.Login;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.Logout;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Queries.GetUser;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.LoginCheck;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.SendAuthNumberToEmail;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.SendAuthNumber;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.VerifyAuthNumber;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.Refresh;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.LoginCheck;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.SendAuthNumber;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.Login;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.Refresh;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.GetUser;

namespace Hello100Admin.API.Controllers;

/// <summary>
/// 인증 관련 API 컨트롤러
/// </summary>
[Route("api/auth")]
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
    /// 1차 로그인
    /// </summary>
    [HttpPost("login-check")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginCheckResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginCheckCommand command)
    {
        _logger.LogInformation("Login Check attempt for AccountId: {AccId} from IP: {IpAddress}",
            command.AccountId, GetClientIpAddress());

        // 클라이언트 UserAgent 추출
        var userAgent = GetClientUserAgent();

        // 클라이언트 IP 추출
        var ipAddress = GetClientIpAddress();

        var commandWithIp = command with { UserAgent = userAgent, IpAddress = ipAddress };

        var result = await _mediator.Send(commandWithIp);

        _logger.LogInformation("User {AccountId} logged in process completed", command.AccountId);

        // 중앙화된 매퍼 사용: 성공/실패 모두 ToActionResult에서 처리합니다.
        return result.ToActionResult(this, authEndpoint: true);
    }

    /// <summary>
    /// 인증번호 이메일로 전송
    /// </summary>
    [HttpPost("send-auth-number-to-email")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<SendAuthNumberResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendAuthNumberToEmail(SendAuthNumberToEmailCommand command)
    {
        _logger.LogInformation("Send Auth Number To Email.");

        var result = await _mediator.Send(command);

        _logger.LogInformation("Send Auth Number To Email logged in process completed");

        return result.ToActionResult(this);
    }

    /// <summary>
    /// 인증번호 SMS로 전송
    /// </summary>
    [HttpPost("send-auth-number-to-sms")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<SendAuthNumberResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendAuthNumberToSms(SendAuthNumberToSmsCommand command)
    {
        _logger.LogInformation("Send Auth Number To Sms.");

        var result = await _mediator.Send(command);

        _logger.LogInformation("Send Auth Number To Sms logged in process completed");

        return result.ToActionResult(this);
    }

    /// <summary>
    /// 인증번호 검증
    /// </summary>
    [HttpPost("verify-auth-number")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyAuthNumber(VerifyAuthNumberCommand command)
    {
        _logger.LogInformation("Verify Auth Number.");

        var result = await _mediator.Send(command);

        _logger.LogInformation("Verify Auth Number logged in process completed");

        return result.ToActionResult(this);
    }

    /// <summary>
    /// 2차 로그인
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        _logger.LogInformation("Login attempt for AccountId: {AccId} from IP: {IpAddress}",
            command.AccountId, GetClientIpAddress());

        // 클라이언트 UserAgent 추출
        var userAgent = GetClientUserAgent();

        // 클라이언트 IP 추출
        var ipAddress = GetClientIpAddress();

        var commandWithIp = command with { UserAgent = userAgent, IpAddress = ipAddress };

        var result = await _mediator.Send(commandWithIp);

        _logger.LogInformation("User {AccountId} logged in process completed", command.AccountId);

        // 중앙화된 매퍼 사용: 성공/실패 모두 ToActionResult에서 처리합니다.
        return result.ToActionResult(this, authEndpoint: true);
    }

    /// <summary>
    /// 토큰 갱신
    /// </summary>
    [HttpPost("refresh")]
    [Auth]
    [ProducesResponseType(typeof(ApiResponse<RefreshResponse>), StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Refresh([FromBody] RefreshCommand command)
    {
        _logger.LogInformation("Refresh attempt");

        // 클라이언트 UserAgent 추출
        var userAgent = GetClientUserAgent();

        // 클라이언트 IP 추출
        var ipAddress = GetClientIpAddress();

        var commandWithIp = command with { Aid = base.Aid, UserAgent = userAgent, IpAddress = ipAddress };

        var result = await _mediator.Send(commandWithIp);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// 로그아웃
    /// </summary>
    [HttpPost("logout")]
    [Auth]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
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
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMe()
    {
        if (string.IsNullOrWhiteSpace(base.Aid) == true)
        {
            return Unauthorized();
        }

        var query = new GetUserQuery { Aid = base.Aid };
        var result = await _mediator.Send(query);

        // 제네릭 Result<UserDto> 경로는 ToActionResult에서 성공/실패를 모두 처리합니다.
        return result.ToActionResult(this);
    }
}
