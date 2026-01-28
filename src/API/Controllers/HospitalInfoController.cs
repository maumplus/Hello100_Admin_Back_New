using Hello100Admin.API.Infrastructure.Attributes;
using MediatR;
using Hello100Admin.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Hello100Admin.Modules.Admin.Application.Features.Hospital.Responses;
using Hello100Admin.Modules.Admin.Application.Features.Hospital.Queries.GetDoctorList;
using Hello100Admin.Modules.Admin.Application.Features.Hospital.Queries.GetHospital;

namespace Hello100Admin.API.Controllers
{
    /// <summary>
    /// 병원정보관리 API Controller
    /// </summary>
    public class HospitalInfoController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<HospitalInfoController> _logger;

        public HospitalInfoController(
        IMediator mediator,
        ILogger<HospitalInfoController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// [병원정보관리 > 병원정보관리]병원정보 API
        /// </summary>
        [HttpGet("/api/admin/hospital")]
        [ProducesResponseType(typeof(GetHospitalResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHospital(CancellationToken cancellationToken = default)
        {
            /*var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }*/

            _logger.LogInformation("GET /api/admin/hospital");

            var query = new GetHospitalQuery()
            {
                HospNo = "10350072"
            };
            var result = await _mediator.Send(query, cancellationToken);

            // 중앙화된 매퍼로 Result -> IActionResult 변환
            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원정보관리 > 의료진관리]의료진 목록 API
        /// </summary>
        [HttpGet("/api/admin/doctor/list")]
        [ProducesResponseType(typeof(List<GetDoctorListResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDoctorList(CancellationToken cancellationToken = default)
        {
            /*var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }*/

            _logger.LogInformation("GET /api/admin/doctor/list");

            var query = new GetDoctorListQuery()
            {
                HospNo = "10350072"
            };
            var result = await _mediator.Send(query, cancellationToken);

            // 중앙화된 매퍼로 Result -> IActionResult 변환
            return result.ToActionResult(this);
        }
    }
}
