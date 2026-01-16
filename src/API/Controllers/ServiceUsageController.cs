using Hello100Admin.API.Constracts.Admin.ServiceUsage;
using Hello100Admin.API.Extensions;
using Hello100Admin.API.Infrastructure.Attributes;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.GetUntactMedicalHistory;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Responses.GetUntactMedicalHistory;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hello100Admin.API.Controllers
{
    /// <summary>
    /// 서비스 이용 관리 API Controller
    /// </summary>
    [Auth]
    public class ServiceUsageController : BaseController
    {
        private readonly ILogger<ServiceUsageController> _logger;
        private readonly IMediator _mediator;

        public ServiceUsageController(IMediator mediator, ILogger<ServiceUsageController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// 비대면 진료 내역 리스트
        /// </summary>
        /// <param name="req">요청 정보 <see cref="GetUntactMedicalHistoryRequest"/></param>
        /// <returns>응답 리스트가 포함된 결과 <see cref="GetUntactMedicalHistoryResponse"/></returns>
        [HttpPost("list")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUntactMedicalHistory(GetUntactMedicalHistoryRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/serviceusage/list [{AId}]", AId);

            var query = req.Adapt<GetUntactMedicalHistoryQuery>() with { HospNo = base.HospNo };

            var result = await _mediator.Send(query, cancellationToken);

            return result.ToActionResult(this);
        }

        // 결제 내역 상세 조회
    }
}
