using Hello100Admin.API.Constracts.Admin;
using Hello100Admin.API.Constracts.Admin.RequestsManagement;
using Hello100Admin.API.Extensions;
using Hello100Admin.API.Infrastructure.Attributes;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Exports;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.Asset.Queries;
using Hello100Admin.Modules.Admin.Application.Features.Asset.Results;
using Hello100Admin.Modules.Admin.Application.Features.RequestsManagement.Queries;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hello100Admin.API.Controllers
{
    /// <summary>
    /// 자산관리(전체관리자) API Controller
    /// </summary>
    [Auth]
    [Route("api/asset")]
    public class AssetController : BaseController
    {
        #region FIELD AREA ****************************************
        private readonly ILogger<AssetController> _logger;
        private readonly IMediator _mediator;
        private readonly string _adminImageUrl;
        #endregion

        #region CONSTRUCTOR AREA ***********************************
        public AssetController(IConfiguration config, IMediator mediator, ILogger<AssetController> logger)
        {
            _logger = logger;
            _mediator = mediator;
            _adminImageUrl = config["AdminImageUrl"] ?? string.Empty;
        }
        #endregion

        #region ACTION METHOD AREA *******************************
        /// <summary>
        /// [전체관리자 > 자산관리] 사용이력 > 리스트 조회
        /// </summary>
        [HttpGet("usage-list")]
        [ProducesResponseType(typeof(ApiResponse<ListResult<GetUsageListResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsageList([FromQuery] GetUsageListRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET api/asset/usage-list [{Aid}]", Aid);

            var query = req.Adapt<GetUsageListQuery>();

            var result = await _mediator.Send(query, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [전체관리자 > 자산관리] 사용이력 > 리스트 조회 > 엑셀출력
        /// </summary>
        [HttpGet("usage-list/export/excel")]
        [ProducesResponseType(typeof(ExcelFile), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportRequestUntactsExcel([FromQuery] GetUsageListRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET api/asset/usage-list/export/excel [{Aid}]", Aid);

            var query = req.Adapt<ExportUsageListExcelQuery>();

            var result = await _mediator.Send(query, cancellationToken);

            if (result.ErrorInfo != null)
                return result.ToActionResult(this);

            var file = result?.Data!;
            return File(file.Content, file.ContentType, file.FileName);
        }
        #endregion
    }
}
