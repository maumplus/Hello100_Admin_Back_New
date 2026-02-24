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
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.AdminUser.Commands;

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
        _logger.LogInformation("PATCH /api/admin-user/update-password [{Aid}]", Aid);

        var command = req.Adapt<UpdatePasswordCommand>() with { Aid = base.Aid };

        var result = await _mediator.Send(command, cancellationToken);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// [전체 관리자] 관리자관리 > 관리자목록 > 조회 (병원관리자 제외)
    /// </summary>
    [HttpGet("list")]
    [ProducesResponseType(typeof(ApiResponse<ListResult<GetAdminUsersResult>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAdminUsersAsync(int pageNo, int pageSize, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GET api/admin-user/list [{Aid}]", Aid);

        var result = await _mediator.Send(new GetAdminUsersQuery(pageNo, pageSize), cancellationToken);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// [전체 관리자] 관리자관리 > 병원관리자목록 > 조회
    /// </summary>
    [HttpGet("hospital-admins")]
    [ProducesResponseType(typeof(ApiResponse<ListResult<GetHospitalAdminListResult>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHospitalAdminListAsync([FromQuery] GetHospitalAdminListRequest req, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GET api/admin-user/hospital-admins [{Aid}]", Aid);

        var query = req.Adapt<GetHospitalAdminListQuery>();

        var result = await _mediator.Send(query, cancellationToken);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// [전체 관리자] 관리자관리 > 병원관리자목록 > 상세정보 > 조회
    /// </summary>
    [HttpPost("hospital-admins/detail")]
    [ProducesResponseType(typeof(ApiResponse<GetHospitalAdminDetailResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHospitalAdminDetailAsync(GetHospitalAdminDetailRequest req, CancellationToken ct = default)
    {
        _logger.LogInformation("POST api/admin-user/hospital-admins/detail [{Aid}]", Aid);

        var query = req.Adapt<GetHospitalAdminDetailQuery>();

        var result = await _mediator.Send(new GetHospitalAdminDetailQuery(req.AId, base.ClientIpAddress), ct);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// [전체 관리자] 관리자관리 > 병원관리자목록 > 상세정보 > 패스워드변경
    /// </summary>
    /// <param name="req"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPatch("hospital-admins/update-password")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateHospitalAdminPassword(UpdateHospitalAdminPasswordRequest req, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("PATCH /api/admin-user/hospital-admins/update-password [{Aid}]", Aid);

        var result = await _mediator.Send(new UpdateHospitalAdminPasswordCommand(req.AId, req.NewPassword), cancellationToken);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// [전체 관리자] 관리자관리 > 병원관리자목록 > 상세정보 > 맵핑삭제
    /// </summary>
    [HttpDelete("hospital-admins/mapping")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteHospitalAdminMappingAsync([FromQuery]DeleteHospitalAdminMappingRequest req, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("DELETE /api/admin-user/hospital-admins/mapping [{AId}]", Aid);

        var result = await _mediator.Send(new DeleteHospitalAdminMappingCommand(base.Aid, req.AccPwd, req.HospitalAId), cancellationToken);

        return result.ToActionResult(this);
    }
}