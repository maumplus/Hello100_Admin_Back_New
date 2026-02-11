using MediatR;
using Microsoft.AspNetCore.Mvc;
using Hello100Admin.API.Extensions;
using Hello100Admin.API.Infrastructure.Attributes;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.RequestsManagement.Commands;
using Hello100Admin.Modules.Admin.Application.Features.RequestsManagement.Queries;
using Hello100Admin.Modules.Admin.Application.Features.RequestsManagement.Results;
using Hello100Admin.Modules.Admin.Application.Features.HospitalUser.Queries;
using Mapster;
using Hello100Admin.Modules.Admin.Application.Features.Advertisement.Queries;

namespace Hello100Admin.API.Controllers
{
    /// <summary>
    /// 요청사항관리(전체관리자) API Controller
    /// </summary>
    [Auth]
    [Route("api/requests-management")]
    public class RequestsManagementController : BaseController
    {
        private readonly ILogger<RequestsManagementController> _logger;
        private readonly IMediator _mediator;

        public RequestsManagementController(IMediator mediator, ILogger<RequestsManagementController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet("bugs")]
        [ProducesResponseType(typeof(ApiResponse<ListResult<GetRequestBugsResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRequestBugs(int pageNo, int pageSize, bool apprYn, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET api/requests-management/bugs [{Aid}]", Aid);

            var result = await _mediator.Send(new GetRequestBugsQuery(pageNo, pageSize, apprYn), cancellationToken);

            return result.ToActionResult(this);
        }

    }
}
