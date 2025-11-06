using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.DTOs;
using Hello100Admin.Modules.Admin.Application.Queries;
using MediatR;
using Hello100Admin.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hello100Admin.API.Controllers;

/// <summary>
/// 고객 관리 API Controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AdminUsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminUsersController> _logger;

    public AdminUsersController(
        IMediator mediator,
        ILogger<AdminUsersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// 관리자 목록 조회 (페이징)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminUserListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAdminUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GET /api/adminusers - Page: {Page}, PageSize: {PageSize}", page, pageSize);

        var query = new GetAdminUsersQuery(page, pageSize, includeDeleted);
        var result = await _mediator.Send(query, cancellationToken);

        // 중앙화된 매퍼로 Result -> IActionResult 변환
        return result.ToActionResult(this);
        // if (result.IsFailure)
        //     return BadRequest(new { error = result.Error });

        // return Ok(result.Value);
    }
}