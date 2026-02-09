using MediatR;
using Hello100Admin.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Hello100Admin.API.Infrastructure.Attributes;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Mapster;
using Hello100Admin.API.Constracts.Admin.AdminUser;
using Hello100Admin.Modules.Admin.Application.Features.AdminUser.Commands.UpdatePassword;
using Hello100Admin.Modules.Admin.Application.Features.AdminUser.Responses.Shared;
using Hello100Admin.Modules.Admin.Application.Features.AdminUser.Responses.GetAdminUser;
using Hello100Admin.Modules.Admin.Application.Features.AdminUser.Queries.GetAdminUser;

namespace Hello100Admin.API.Controllers;

/// <summary>
/// 고객 관리 API Controller
/// </summary>
[Auth]
[Route("api/admin-user")]
public class AdminUserController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminUserController> _logger;

    public AdminUserController(
        IMediator mediator,
        ILogger<AdminUserController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// 관리자 목록 조회 (페이징)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminUserResponse>), StatusCodes.Status200OK)]
    [Obsolete("템플릿. 미사용. 추후 삭제 예정.")]
    public async Task<IActionResult> GetAdminUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GET /api/admin-user - Page: {Page}, PageSize: {PageSize}", page, pageSize);

        var query = new GetAdminUserQuery(page, pageSize, includeDeleted);
        var result = await _mediator.Send(query, cancellationToken);

        // 중앙화된 매퍼로 Result -> IActionResult 변환
        return result.ToActionResult(this);
        // if (result.IsFailure)
        //     return BadRequest(new { error = result.Error });

        // return Ok(result.Value);
    }

    /// <summary>
    /// 비밀번호 변경
    /// </summary>
    /// <param name="req"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPatch("update-password")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePassword(UpdatePasswordRequest req, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("PATCH /api/adminuser/update-password [{Aid}]", Aid);

        var command = req.Adapt<UpdatePasswordCommand>() with { Aid = base.Aid };

        var result = await _mediator.Send(command, cancellationToken);

        return result.ToActionResult(this);
    }
}