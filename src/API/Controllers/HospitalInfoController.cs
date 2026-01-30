using Hello100Admin.API.Infrastructure.Attributes;
using MediatR;
using Hello100Admin.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Hello100Admin.Modules.Admin.Application.Features.Hospital.Responses;
using Hello100Admin.Modules.Admin.Application.Features.Hospital.Queries.GetDoctorList;
using Hello100Admin.Modules.Admin.Application.Features.Hospital.Queries.GetHospital;
using Hello100Admin.Modules.Admin.Application.Features.Hospital.Queries.GetHospitalSetting;
using Hello100Admin.Modules.Admin.Application.Features.Hospital.Queries.GetHospitalList;

namespace Hello100Admin.API.Controllers
{
    /// <summary>
    /// 병원정보관리 API Controller
    /// </summary>
    [Route("api/hospital-info")]
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
        /// [병원정보관리 > 병원정보관리]병원 목록 API
        /// </summary>
        [HttpGet("/api/hospital-info/hospitals")]
        [ProducesResponseType(typeof(GetHospitalResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHospitalList(CancellationToken cancellationToken = default)
        {
            /*var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }*/

            _logger.LogInformation("GET /api/hospital-info/hospitals");

            var query = new GetHospitalListQuery()
            {
                ChartType = "%",
                SearchType = 0,
                Keyword = "",
                PageNo = 0,
                PageSize = 100
            };

            var result = await _mediator.Send(query, cancellationToken);

            // 중앙화된 매퍼로 Result -> IActionResult 변환
            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원정보관리 > 병원정보관리]병원정보 API
        /// </summary>
        [HttpGet("/api/hospital-info/hospital")]
        [ProducesResponseType(typeof(GetHospitalResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHospital(CancellationToken cancellationToken = default)
        {
            /*var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }*/

            _logger.LogInformation("GET /api/hospital-info/hospital");

            var query = new GetHospitalQuery()
            {
                HospNo = "10350072"
            };
            var result = await _mediator.Send(query, cancellationToken);

            // 중앙화된 매퍼로 Result -> IActionResult 변환
            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원정보관리 > Hello100 설정]Hello100 설정정보 API
        /// </summary>
        [HttpGet("/api/hospital-info/hospital/setting")]
        [ProducesResponseType(typeof(GetHospitalResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHospitalSetting(CancellationToken cancellationToken = default)
        {
            /*var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }*/

            _logger.LogInformation("GET /api/hospital-info/hospital/setting");

            var query = new GetHospitalSettingQuery()
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
        [HttpGet("/api/hospital-info/doctors")]
        [ProducesResponseType(typeof(List<GetDoctorListResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDoctorList(CancellationToken cancellationToken = default)
        {
            /*var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }*/

            _logger.LogInformation("GET /api/hospital-info/doctors");

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
