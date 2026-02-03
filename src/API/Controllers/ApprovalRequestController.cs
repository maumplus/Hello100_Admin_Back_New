using Hello100Admin.API.Extensions;
using Hello100Admin.API.Infrastructure.Attributes;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Features.ApprovalRequest.Queries.GetUntactMedicalRequestDetailForApproval;
using Hello100Admin.Modules.Admin.Application.Features.ApprovalRequest.Queries.GetUntactMedicalRequestsForApproval;
using Hello100Admin.Modules.Admin.Application.Features.ApprovalRequest.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hello100Admin.API.Controllers
{
    /// <summary>
    /// 승인 요청 관리 API Controller
    /// </summary>
    [Auth]
    [Route("api/approval-request")]
    public class ApprovalRequestController : BaseController
    {
        private readonly ILogger<ApprovalRequestController> _logger;
        private readonly IMediator _mediator;

        public ApprovalRequestController(IMediator mediator, ILogger<ApprovalRequestController> logger) 
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// 승인요청함 > 비대면진료신청목록 > 조회
        /// </summary>
        [HttpGet("untact-medical-requests")]
        [ProducesResponseType(typeof(ApiResponse<GetUntactMedicalRequestsForApprovalResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUntactMedicalRequestsForApproval(int pageNo, int pageSize, string apprYn, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/approvalrequest/untact-medical-requests [{Aid}]", Aid);

            var query = new GetUntactMedicalRequestsForApprovalQuery
            {
                PageNo = pageNo,
                PageSize = pageSize,
                HospKey = base.HospKey,
                ApprYn = apprYn
            };

            var result = await _mediator.Send(query, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// 승인요청함 > 비대면진료신청목록 > 상세보기 조회
        /// </summary>
        [HttpGet("untact-medical-requests/{seq}")]
        [ProducesResponseType(typeof(ApiResponse<GetUntactMedicalRequestDetailForApprovalResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUntactMedicalRequestDetailForApproval(int seq, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/approvalrequest/untact-medical-requests/{seq} [{Aid}]", seq, Aid);

            var result = await _mediator.Send(new GetUntactMedicalRequestDetailForApprovalQuery(seq), cancellationToken);

            return result.ToActionResult(this);
        }
    }
}
