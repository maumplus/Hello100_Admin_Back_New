using MediatR;
using Hello100Admin.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Hello100Admin.API.Infrastructure.Attributes;
using Hello100Admin.Modules.Admin.Application.Features.Member.Responses.GetMember;
using Hello100Admin.Modules.Admin.Application.Features.Member.Queries.GetMember;

namespace Hello100Admin.API.Controllers;

/// <summary>
/// 멤버 관리 API Controller
/// </summary>
[Auth]
[Route("api/member")]
public class MemberController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<MemberController> _logger;

    public MemberController(
        IMediator mediator,
        ILogger<MemberController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// 고객 상세 조회
    /// </summary>
    [HttpGet("{uid}")]
    [ProducesResponseType(typeof(GetMemberResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Obsolete("템플릿. 미사용. 추후 삭제 예정.")]
    public async Task<IActionResult> GetMember(
        string uid,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GET /api/member/{Uid}", uid);

        var query = new GetMemberQuery(uid);
        var result = await _mediator.Send(query, cancellationToken);

        // 중앙화된 매퍼로 Result -> IActionResult 변환
        return result.ToActionResult(this);
    }
}