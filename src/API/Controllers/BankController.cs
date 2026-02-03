using Hello100Admin.API.Extensions;
using Hello100Admin.API.Infrastructure.Attributes;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Seller.Application.Features.Bank.Queries.GetBankList;
using Hello100Admin.Modules.Seller.Application.Features.Bank.Responses.GetBankList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hello100Admin.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Auth]
    [Route("api/bank")]
    public class BankController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BankController> _logger;

        public BankController(
            IMediator mediator,
            ILogger<BankController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// 송금 임시 등록
        /// </summary>
        /// <param name="req">송금 시 요청 정보</param>
        /// <returns></returns>
        //[AllowAnonymous]
        [HttpGet("list")]
        [ProducesResponseType(typeof(Result<GetBankListResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBankList(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/bank/list [{Aid}]", Aid);

            var result = await _mediator.Send(new GetBankListQuery(), cancellationToken);

            return result.ToActionResult(this);
        }
    }
}
