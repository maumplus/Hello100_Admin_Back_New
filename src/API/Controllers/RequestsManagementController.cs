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
using Hello100Admin.API.Constracts.Admin.Advertisement;
using Hello100Admin.API.Constracts.Admin.RequestsManagement;
using Hello100Admin.Modules.Admin.Application.Features.Advertisement.Commands;
using Hello100Admin.Modules.Admin.Application.Common.Exports;
using Hello100Admin.Modules.Admin.Application.Features.Hospitals.Queries;

namespace Hello100Admin.API.Controllers
{
    /// <summary>
    /// 요청사항관리(전체관리자) API Controller
    /// </summary>
    [Auth]
    [Route("api/requests-management")]
    public class RequestsManagementController : BaseController
    {
        #region FIELD AREA ****************************************
        private readonly ILogger<RequestsManagementController> _logger;
        private readonly IMediator _mediator;
        private readonly string _adminImageUrl;
        #endregion


        #region CONSTRUCTOR AREA ***********************************
        public RequestsManagementController(IConfiguration config, IMediator mediator, ILogger<RequestsManagementController> logger)
        {
            _logger = logger;
            _mediator = mediator;
            _adminImageUrl = config["AdminImageUrl"] ?? string.Empty;
        }
        #endregion

        #region ACTION METHOD AREA *******************************
        /// <summary>
        /// [전체관리자 > 요청사항관리] 잘못된정보 수정요청 > 리스트 조회
        /// </summary>
        [HttpGet("bugs")]
        [ProducesResponseType(typeof(ApiResponse<ListResult<GetRequestBugsResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRequestBugs(int pageNo, int pageSize, bool apprYn, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET api/requests-management/bugs [{Aid}]", Aid);

            var result = await _mediator.Send(new GetRequestBugsQuery(pageNo, pageSize, apprYn), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [전체관리자 > 요청사항관리] 잘못된정보 수정요청 > 상세 조회
        /// </summary>
        [HttpGet("bugs/{hpId}")]
        [ProducesResponseType(typeof(ApiResponse<GetRequestBugResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRequestBug(int hpId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET api/requests-management/bugs/{hpId} [{Aid}]", hpId, Aid);

            var result = await _mediator.Send(new GetRequestBugQuery(hpId), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [전체관리자 > 요청사항관리] 잘못된정보 수정요청 > 관리자확인 업데이트
        /// </summary>
        [HttpPatch("bugs/{hpId}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateRequestBug(int hpId, UpdateRequestBugRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PATCH api/requests-management/bugs/{hpId} [{Aid}]", hpId, Aid);

            var command = req.Adapt<UpdateRequestBugCommand>() with
            {
                HpId = hpId,
                ApprAid = req.ApprAId
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [전체관리자 > 요청사항관리] 비대면진료 신청목록 > 리스트 조회
        /// </summary>
        [HttpGet("untacts")]
        [ProducesResponseType(typeof(ApiResponse<ListResult<GetRequestUntactsResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRequestUntacts([FromQuery] GetRequestUntactsRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET api/requests-management/untacts [{Aid}]", Aid);

            var query = req.Adapt<GetRequestUntactsQuery>();

            var result = await _mediator.Send(query, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [전체관리자 > 요청사항관리] 비대면진료 신청목록 > 엑셀출력
        /// </summary>
        [HttpGet("untacts/export/excel")]
        [ProducesResponseType(typeof(ExcelFile), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportRequestUntactsExcel([FromQuery] GetRequestUntactsRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/requests-management/untacts/export/excel [{Aid}]", Aid);

            var query = req.Adapt<ExportRequestUntactsExcelQuery>();

            var result = await _mediator.Send(query, cancellationToken);

            if (result.ErrorInfo != null)
                return result.ToActionResult(this);

            var file = result?.Data!;
            return File(file.Content, file.ContentType, file.FileName);
        }

        /// <summary>
        /// [전체관리자 > 요청사항관리] 비대면진료 신청목록 > 상세 조회
        /// </summary>
        [HttpGet("Untacts/{seq}")]
        [ProducesResponseType(typeof(ApiResponse<GetRequestUntactResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRequestUntact(int seq, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET api/requests-management/bugs/{seq} [{Aid}]", seq, Aid);

            var rootUrl = _adminImageUrl;

            var result = await _mediator.Send(new GetRequestUntactQuery(seq, rootUrl), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [전체관리자 > 요청사항관리] 비대면진료 신청목록 > 승인/반려 업데이트
        /// </summary>
        [HttpPatch("Untacts/{seq}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateRequestUntact(int seq, UpdateRequestUntactRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PATCH api/requests-management/untact/{seq} [{Aid}]", seq, Aid);

            var command = req.Adapt<UpdateRequestUntactCommand>() with
            {
                Seq = seq,
                JoinState = req.JoinState,
                ReturnReason = req.ReturnReason
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }
        #endregion

    }
}
