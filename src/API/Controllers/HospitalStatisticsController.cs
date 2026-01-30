using Hello100Admin.API.Extensions;
using Hello100Admin.API.Infrastructure.Attributes;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Features.HospitalStatistics.Queries;
using Hello100Admin.Modules.Admin.Application.Features.HospitalStatistics.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hello100Admin.API.Controllers
{
    /// <summary>
    /// 통계 관리 API
    /// </summary>
    [Auth]
    [Route("api/hospital-statistics")]
    public class HospitalStatisticsController : BaseController
    {
        private readonly ILogger<HospitalStatisticsController> _logger;
        private readonly IMediator _mediator;

        public HospitalStatisticsController(IMediator mediator, ILogger<HospitalStatisticsController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// 환자/월별 통계 > 접수구분통계 (접수방법별 접수 현황)
        /// stats 오타 아님
        /// </summary>
        [HttpGet("me/patient-statistics/registrations/by-method/{year}")]
        [ProducesResponseType(typeof(ApiResponse<GetRegistrationStatsByMethodResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRegistrationStatsByMethod(string year, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/hospital-statistics/me/patient-statistics/registrations/by-method/{year} [{AId}]", year, AId);
            
            var result = await _mediator.Send(new GetRegistrationStatsByMethodQuery(base.HospNo, year), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// 환자/월별 통계 > 환자구분통계 (접수/취소 현황)
        /// </summary>
        [HttpGet("me/patient-statistics/registrations/status-summary/{year}")]
        [ProducesResponseType(typeof(ApiResponse<GetRegistrationStatusSummaryResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRegistrationStatusSummary(string year, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/hospital-statistics/me/patient-statistics/registrations/status-summary/{year} [{AId}]", year, AId);

            var result = await _mediator.Send(new GetRegistrationStatusSummaryQuery(base.HospNo, year), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// 내원목적별 통계 > 조회 (내원목적별 접수/취소 월별 통계)
        /// </summary>
        [HttpGet("me/patient-statistics/registrations/by-visit-purpose/{yearMonth}")]
        [ProducesResponseType(typeof(ApiResponse<GetRegistrationStatsByVisitPurposeResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRegistrationStatsByVisitPurpose(string yearMonth, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/hospital-statistics/me/patient-statistics/registrations/by-visit-purpose/{yearMonth} [{AId}]", yearMonth, AId);

            var result = await _mediator.Send(new GetRegistrationStatsByVisitPurposeQuery(base.HospNo, yearMonth), cancellationToken);

            return result.ToActionResult(this);
        }
    }
}
