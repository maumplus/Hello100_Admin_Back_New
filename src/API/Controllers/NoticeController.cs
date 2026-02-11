using Hello100Admin.API.Constracts.Admin.Notice;
using Hello100Admin.API.Extensions;
using Hello100Admin.API.Infrastructure.Attributes;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.Notice.Commands;
using Hello100Admin.Modules.Admin.Application.Features.Notice.Queries;
using Hello100Admin.Modules.Admin.Application.Features.Notice.Results;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hello100Admin.API.Controllers
{
    /// <summary>
    /// 공지사항 API
    /// </summary>
    [Auth]
    [Route("api/notice")]
    public class NoticeController : BaseController
    {
        #region FIELD AREA ****************************************
        private readonly ILogger<NoticeController> _logger;
        private readonly IMediator _mediator;
        #endregion

        #region CONSTRUCTOR AREA ***********************************
        public NoticeController(IMediator mediator, ILogger<NoticeController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }
        #endregion

        #region ACTION METHOD AREA *******************************
        /// <summary>
        /// [전체 관리자] 공지사항 > 공지목록 > 조회
        /// </summary>
        [HttpGet("notices")]
        [ProducesResponseType(typeof(ApiResponse<ListResult<GetNoticesResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNotices(int pageNo, int pageSize, string? searchKeyword, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/notice/notices [{AId}]", Aid);

            var result = await _mediator.Send(new GetNoticesQuery(pageNo, pageSize, searchKeyword), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [전체 관리자] 공지사항 > 공지목록 > 신규등록
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("notices")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateNotice(CreateNoticeRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/notice/notices [{AId}]", Aid);

            var command = req.Adapt<CreateNoticeCommand>();

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [전체 관리자] 공지사항 > 공지목록 > 공지사항편집 > 조회
        /// </summary>
        [HttpGet("notices/{notiId}")]
        [ProducesResponseType(typeof(ApiResponse<GetNoticeResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNotice(int notiId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/notice/notices/{notiId} [{AId}]", notiId, Aid);

            var result = await _mediator.Send(new GetNoticeQuery(notiId), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [전체 관리자] 공지사항 > 공지목록 > 공지사항편집 > 저장
        /// </summary>
        [HttpPatch("notices/{notiId}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateNotice(int notiId, UpdateNoticeRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PATCH /api/notice/notices/{notiId} [{AId}]", notiId, Aid);

            var command = req.Adapt<UpdateNoticeCommand>() with { NotiId = notiId };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [전체 관리자] 공지사항 > 공지목록 > 공지사항편집 > 삭제
        /// </summary>
        [HttpDelete("notices/{notiId}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteNotice(int notiId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("DELETE /api/notice/notices/{notiId} [{AId}]", notiId, Aid);

            var result = await _mediator.Send(new DeleteNoticeCommand(notiId), cancellationToken);

            return result.ToActionResult(this);
        }
        #endregion
    }
}
