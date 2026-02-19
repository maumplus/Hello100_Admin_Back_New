using MediatR;
using Hello100Admin.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Hello100Admin.API.Infrastructure.Attributes;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Mapster;
using Hello100Admin.API.Constracts.Admin.AdminUser;
using Hello100Admin.Modules.Admin.Application.Features.AdminUser.Commands.UpdatePassword;
using Hello100Admin.Modules.Admin.Application.Features.AdminUser.Results;
using Hello100Admin.Modules.Admin.Application.Features.AdminUser.Queries;

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
    /// [병원 관리자] 비밀번호 변경
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

    /// <summary>
    /// [전체 관리자] 관리자관리 > 관리자목록 > 조회 (병원관리자 제외)
    /// </summary>
    [HttpGet("list")]
    [ProducesResponseType(typeof(ApiResponse<GetAdminUsersResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAdminUsersAsync(int pageNo, int pageSize, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GET api/admin-user/list [{Aid}]", Aid);

        var result = await _mediator.Send(new GetAdminUsersQuery(pageNo, pageSize), cancellationToken);

        return result.ToActionResult(this);
    }
}