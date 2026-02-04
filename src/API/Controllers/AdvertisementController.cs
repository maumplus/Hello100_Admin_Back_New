using Hello100Admin.API.Constracts.Admin.Advertisement;
using Hello100Admin.API.Extensions;
using Hello100Admin.API.Infrastructure.Attributes;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.Advertisement.Commands;
using Hello100Admin.Modules.Admin.Application.Features.Advertisement.Queries;
using Hello100Admin.Modules.Admin.Application.Features.Advertisement.Results;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hello100Admin.API.Controllers
{
    /// <summary>
    /// 광고 API
    /// </summary>
    [Auth]
    [Route("api/advertisement")]
    public class AdvertisementController : BaseController
    {
        #region FIELD AREA ****************************************
        private readonly ILogger<AdvertisementController> _logger;
        private readonly IMediator _mediator;
        #endregion

        #region CONSTRUCTOR AREA ***********************************
        public AdvertisementController(IMediator mediator, ILogger<AdvertisementController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }
        #endregion

        #region ACTION METHOD AREA *******************************
        /// <summary>
        /// [전체 관리자] 광고관리 > 팝업관리 > 조회
        /// </summary>
        [HttpGet("popups")]
        [ProducesResponseType(typeof(ApiResponse<ListResult<GetPopupsResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPopups(int pageNo, int pageSize, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/advertisement/popups [{AId}]", Aid);
            
            var result = await _mediator.Send(new GetPopupsQuery(pageNo, pageSize), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [전체 관리자] 광고관리 > 팝업관리 > 신규등록 > 등록
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("popups")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreatePopup([FromForm] CreatePopupRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/advertisement/popups [{AId}]", Aid);

            var payload = GetImagePayload(req.Image);

            var command = req.Adapt<CreatePopupCommand>() with
            {
                ImagePayload = payload
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [전체 관리자] 광고관리 > 팝업관리 > 팝업광고편집 > 조회
        /// </summary>
        [HttpGet("popups/{popupId}")]
        [ProducesResponseType(typeof(ApiResponse<GetPopupResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPopup(int popupId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/advertisement/popups/{popupId} [{AId}]", popupId, Aid);

            var result = await _mediator.Send(new GetPopupQuery(popupId), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [전체 관리자] 광고관리 > 팝업관리 > 팝업광고편집 > 저장
        /// </summary>
        [HttpPatch("popups/{popupId}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePopup(int popupId, [FromForm] UpdatePopupRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PATCH /api/advertisement/popups/{popupId} [{AId}]", popupId, Aid);

            var payload = GetImagePayload(req.Image);

            var command = req.Adapt<UpdatePopupCommand>() with
            {
                AdId = popupId,
                ImagePayload = payload
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [전체 관리자] 광고관리 > 팝업관리 > 팝업광고편집 > 삭제
        /// </summary>
        [HttpDelete("popups/{popupId}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeletePopup(int popupId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("DELETE /api/advertisement/popups/{popupId} [{AId}]", popupId, Aid);

            var result = await _mediator.Send(new DeletePopupCommand(popupId), cancellationToken);

            return result.ToActionResult(this);
        }
        #endregion

        #region INTERNAL METHOD AREA **********************************************
        private FileUploadPayload? GetImagePayload(IFormFile? image)
        {
            if (image == null)
                return null;
            
            var payload = new FileUploadPayload(
                image.FileName,
                image.ContentType,
                image.Length,
                () => image.OpenReadStream()
            );

            return payload;
        }
        #endregion
    }
}
