using Hello100Admin.API.Constracts.Admin.VisitPurpose;
using Hello100Admin.API.Extensions;
using Hello100Admin.API.Infrastructure.Attributes;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.BulkUpdateCertificates;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.BulkUpdateVisitPurposes;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.CreateVisitPurpose;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.DeleteVisitPurpose;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.UpdateVisitPurposeForNhisHealthScreening;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.UpdateVisitPurposeForNonNhisHealthScreening;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Queries;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Queries.GetCertificates;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Queries.GetVisitPurposeDetail;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Queries.GetVisitPurposes;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Responses.GetCertificates;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Responses.GetVisitPurposeDetail;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Responses.GetVisitPurposes;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hello100Admin.API.Controllers
{
    /// <summary>
    /// 내원목적관리 API
    /// </summary>
    [Auth]
    [Route("api/visit-purpose")]
    public class VisitPurposeController : BaseController
    {
        private readonly ILogger<VisitPurposeController> _logger;
        private readonly IMediator _mediator;

        public VisitPurposeController(IMediator mediator, ILogger<VisitPurposeController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// 내원목적관리 조회
        /// </summary>
        [HttpGet("visit-purposes")]
        [ProducesResponseType(typeof(ApiResponse<GetVisitPurposesResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVisitPurposes(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/visitpurpose/visit-purposes [{Aid}]", Aid);
            
            var result = await _mediator.Send(new GetVisitPurposesQuery(base.HospKey), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// 내원목적관리 > 내원목적편집 조회
        /// </summary>
        [HttpGet("visit-purposes/{vpCd}")]
        [ProducesResponseType(typeof(ApiResponse<GetVisitPurposeDetailResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVisitPurposeDetail(string vpCd, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/visitpurpose/visit-purposes/{vpCd} [{Aid}]", vpCd, Aid);
            
            var result = await _mediator.Send(new GetVisitPurposeDetailQuery(vpCd, base.HospKey, base.HospNo), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// 내원목적관리 > 목록편집 > 저장
        /// </summary>
        [HttpPut("visit-purposes/bulk")]
        [ProducesResponseType(typeof(ApiResponse<GetVisitPurposesResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> BulkUpdateVisitPurposes(List<BulkUpdateVisitPurposesRequest> req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PUT /api/visitpurpose/visit-purposes/bulk [{Aid}]", Aid);

            if (req != null && req.Count <= 0)
                return Result.Success(GlobalErrorCode.EmptyRequestBody.ToError()).ToActionResult(this);

            var tempList = req.Adapt<List<BulkUpdateVisitPurposeCommandItem>>();

            var command = new BulkUpdateVisitPurposesCommand()
            {
                HospKey = base.HospKey,
                Items = tempList
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// 내원목적관리 > 신규등록 > 등록요청
        /// </summary>
        [HttpPost("visit-purposes/add")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateVisitPurpose(CreateVisitPurposeRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/visitpurpose/visit-purposes/add [{Aid}]", Aid);

            var command = req.Adapt<CreateVisitPurposeCommand>() with
            {
                HospNo = base.HospNo,
                AId = base.Aid,
                HospKey = base.HospKey,
                InpuiryUrl = "https://paper.hello100.kr/papercheck/index/",
                InpuiryIdx = req.InquiryIdx,
                InpuirySkipYn = "Y",
                DelYn = "N",
                Role = this.SetRole(req.Roles)
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// 내원목적관리 > 내원목적편집 > 저장 (공단 검진)
        /// National Health Insurance Service (국민건강보험공단), Health Screening (건강 검진) 
        /// </summary>
        [HttpPut("visit-purposes/nhis-health-screening")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateVisitPurposeForNhisHealthScreening(UpdateVisitPurposeForNhisHealthScreeningRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PUT /api/visitpurpose/visit-purposes/nhis-health-screening [{Aid}]", Aid);

            var command = req.Adapt<UpdateVisitPurposeForNhisHealthScreeningCommand>() with
            {
                HospKey = base.HospKey,
                Role = this.SetRole(req.Roles),
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// 내원목적관리 > 내원목적편집 > 저장 (공단 검진 외 나머지)
        /// National Health Insurance Service (국민건강보험공단), Health Screening (건강 검진) 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut("visit-purposes/non-nhis-health-screening")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateVisitPurposeForNonNhisHealthScreening(UpdateVisitPurposeForNonNhisHealthScreeningRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PUT /api/visitpurpose/visit-purposes/non-nhis-health-screening [{Aid}]", Aid);

            // Request 체크 필요
            var command = req.Adapt<UpdateVisitPurposeForNonNhisHealthScreeningCommand>() with
            {
                HospNo = base.HospNo,
                AId = base.Aid,
                HospKey = base.HospKey,
                InpuiryUrl = "https://paper.hello100.kr/papercheck/index/",
                InpuiryIdx = req.InquiryIdx,
                InpuirySkipYn = null, // 이거 맞음?
                DelYn = "N", // 이거 맞음?
                Role = this.SetRole(req.Roles)
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// 내원목적관리 > 내원목적편집 > 삭제 (공단 검진 외 나머지)
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPatch("visit-purposes/{vpCd}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteVisitPurpose(string vpCd, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PATCH /api/visitpurpose/visit-purposes/{vpCd} [{Aid}]", vpCd, Aid);

            var command = new DeleteVisitPurposeCommand()
            {
                VpCd = vpCd,
                HospKey = base.HospKey,
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// 내원목적관리 > 내원목적편집(공단검진 제외), 신규등록 시 문진표 조회
        /// </summary>
        [HttpPatch("visit-purposes/questionnaires")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetQuestionnaires(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PATCH /api/visitpurpose/visit-purposes/questionnaires [{Aid}]", Aid);

            var result = await _mediator.Send(new GetQuestionnairesQuery(base.HospNo), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// 제증명문서관리 조회
        /// </summary>
        [HttpGet("certificates")]
        [ProducesResponseType(typeof(ApiResponse<GetCertificatesResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCertificates(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/visitpurpose/certificates [{Aid}]", Aid);
            
            var result = await _mediator.Send(new GetCertificatesQuery(base.HospKey), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// 제증명문서관리 > 목록편집 > 저장
        /// </summary>
        [HttpPut("certificates/bulk")]
        [ProducesResponseType(typeof(ApiResponse<GetVisitPurposesResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> BulkUpdateCertificates(List<BulkUpdateCertificatesRequest> req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PUT /api/visitpurpose/certificates/bulk [{Aid}]", Aid);

            if (req != null && req.Count <= 0)
                return Result.Success(GlobalErrorCode.EmptyRequestBody.ToError()).ToActionResult(this);

            var tempList = req.Adapt<List<BulkUpdateCertificatesCommandItem>>();

            var command = new BulkUpdateCertificatesCommand()
            {
                HospKey = base.HospKey,
                Items = tempList
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        #region INTERNAL METHOD AREA ********************************************
        private int SetRole(List<int>? roleList)
        {
            if (roleList == null || roleList.Count <= 0)
                return 0;

            int role = 0;

            foreach (var item in roleList)
            {
                role |= item;
            }

            return role;
        }
        #endregion
    }
}
