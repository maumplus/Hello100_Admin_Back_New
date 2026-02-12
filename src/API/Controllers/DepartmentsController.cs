using Hello100Admin.API.Extensions;
using Hello100Admin.API.Infrastructure.Attributes;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.Departments.Queries;
using Hello100Admin.Modules.Admin.Application.Features.Departments.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hello100Admin.API.Controllers
{
    /// <summary>
    /// 진료과 조회 관련 API Controller
    /// </summary>
    [Auth]
    [Route("api/departments")]
    public class DepartmentsController : BaseController
    {
        #region FIELD AREA ****************************************
        private readonly ILogger<DepartmentsController> _logger;
        private readonly IMediator _mediator;
        #endregion

        #region CONSTRUCTOR AREA ***********************************
        public DepartmentsController(IMediator mediator, ILogger<DepartmentsController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }
        #endregion

        #region ACTION METHOD AREA *******************************
        /// <summary>
        /// [병원관리자] 병원정보관리 > 병원정보관리 > 전체 진료과목 조회
        /// [전체 관리자] 병원목록 > 병원목록 > 전체 진료과 조회
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<ListResult<GetDepartmentsResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDepartments(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/departments");

            var result = await _mediator.Send(new GetDepartmentsQuery(), cancellationToken);

            return result.ToActionResult(this);
        }
        #endregion
    }
}
