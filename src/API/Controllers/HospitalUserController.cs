using Hello100Admin.API.Constracts.Admin.HospitalUser;
using Hello100Admin.API.Extensions;
using Hello100Admin.API.Infrastructure.Attributes;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.HospitalUser.Queries;
using Hello100Admin.Modules.Admin.Application.Features.HospitalUser.Results;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hello100Admin.API.Controllers
{
    /// <summary>
    /// 회원 관리(병원 사용자 관리) API Controller
    /// </summary>
    [Auth]
    [Route("api/hospital-user")]
    public class HospitalUserController : BaseController
    {
        private readonly ILogger<HospitalUserController> _logger;
        private readonly IMediator _mediator;

        public HospitalUserController(IMediator mediator, ILogger<HospitalUserController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// [전체관리자] 회원목록 > 조회
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<ListResult<SearchHospitalUsersResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchHospitalUsers([FromQuery] SearchHospitalUsersRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET api/hospital-user [{AId}]", AId);

            var query = req.Adapt<SearchHospitalUsersQuery>();

            var result = await _mediator.Send(query, cancellationToken);

            return result.ToActionResult(this);
        }
    }
}
