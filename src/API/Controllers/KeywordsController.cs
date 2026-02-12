using Hello100Admin.API.Extensions;
using Hello100Admin.API.Infrastructure.Attributes;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Features.Keywords.Queries;
using Hello100Admin.Modules.Admin.Application.Features.Keywords.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hello100Admin.API.Controllers
{
    /// <summary>
    /// 키워드 조회 관련 API Controller
    /// </summary>
    [Auth]
    [Route("api/keywords")]
    public class KeywordsController : BaseController
    {
        #region FIELD AREA ****************************************
        private readonly ILogger<KeywordsController> _logger;
        private readonly IMediator _mediator;
        #endregion

        #region CONSTRUCTOR AREA ***********************************
        public KeywordsController(IMediator mediator, ILogger<KeywordsController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }
        #endregion

        #region ACTION METHOD AREA *******************************
        /// <summary>
        /// [병원관리자] 병원정보관리 > 병원정보관리 > 증상/키워드 조회(모달)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<GetKeywordsResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetKeywords(string? keyword, string? masterSeq, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/keywords");

            var result = await _mediator.Send(new GetKeywordsQuery(keyword, masterSeq), cancellationToken);

            return result.ToActionResult(this);
        }
        #endregion
    }
}
