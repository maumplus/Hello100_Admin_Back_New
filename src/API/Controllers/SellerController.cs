using Hello100Admin.API.Constracts.Seller;
using Hello100Admin.API.Extensions;
using Hello100Admin.API.Infrastructure.Attributes;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.CreateSeller;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.CreateSellerRemit;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.DeleteSellerRemit;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.UpdateSellerRemit;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.UpdateSellerRemitEnabled;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Queries.GetRemitBalance;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Queries.GetSellerDetail;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Queries.GetSellerList;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Queries.GetSellerRemitList;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Queries.GetSellerRemitWaitList;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.GetRemitBalance;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.GetSellerDetail;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.GetSellerList;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.GetSellerRemitList;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.GetSellerRemitWaitList;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.Shared;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.UpdateSellerRemit;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hello100Admin.API.Controllers
{
    /// <summary>
    /// 송금 API Controller
    /// </summary>
    [Auth]
    public class SellerController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SellerController> _logger;

        public SellerController(
            IMediator mediator,
            ILogger<SellerController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// 셀러 등록
        /// </summary>
        /// <param name="req">셀러 등록 시 요청 정보 <see cref="CreateSellerRequest"/></param>
        /// <returns>응답 결과 <see cref="Result"/></returns>
        /// <returns></returns>
        [HttpPost("add")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateSeller(CreateSellerRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/seller/add [{AId}], [{HospNo}]", base.Aid, req.HospNo);

            var command = req.Adapt<CreateSellerCommand>() with
            { 
                Aid = base.Aid
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// 셀러 리스트
        /// </summary>
        /// <param name="req">셀러 등록 시 요청 정보 <see cref="GetSellerListRequest"/></param>
        /// <returns>응답 리스트가 포함된 결과 <see cref="PagedResult{GetSellerListResponse}"/></returns>
        [HttpPost("list")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<GetSellerListResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSellerList(GetSellerListRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/seller/list [{Aid}]", Aid);

            var query = req.Adapt<GetSellerListQuery>();

            var result = await _mediator.Send(query, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// 셀러 정보 상세
        /// </summary>
        /// <param name="id">셀러 일련번호</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<GetSellerDetailResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSellerDetail(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/seller/{id} [{Aid}]", id, Aid);

            var query = new GetSellerDetailQuery(id);

            var result = await _mediator.Send(query, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// 송금 임시 등록
        /// </summary>
        /// <param name="req">송금 시 요청 정보</param>
        /// <returns></returns>
        [HttpPost("remit-add")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateSellerRemit(CreateSellerRemitRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/seller/remit-add [{SellerId}] [{Aid}]", req.HospSellerId, Aid);

            var command = req.Adapt<CreateSellerRemitCommand>() with
            {
                Aid = base.Aid
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// KCP 송금 요청
        /// </summary>
        /// <param name="req">송금 시 요청 정보</param>
        /// <returns></returns>
        [HttpPatch("remit-request")]
        [ProducesResponseType(typeof(ApiResponse<UpdateSellerRemitResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateSellerRemit(UpdateSellerRemitRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PATCH /api/seller/remit-request [{Id}] [{Aid}]", req.Id, Aid);

            var command = req.Adapt<UpdateSellerRemitCommand>();

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// KCP 잔액조회
        /// </summary>
        /// <returns></returns>
        [HttpPost("remit-balance")]
        [ProducesResponseType(typeof(ApiResponse<GetRemitBalanceResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRemitBalance(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/seller/remit-balance [{Aid}]", Aid);

            var result = await _mediator.Send(new GetRemitBalanceQuery(), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// 송금 리스트 
        /// </summary>  
        /// <returns></returns>
        [HttpPost("remit-list")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<GetSellerRemitListResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSellerRemitList(GetSellerRemitListRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/seller/remit-list [{Aid}]", Aid);

            var query = req.Adapt<GetSellerRemitListQuery>();

            var result = await _mediator.Send(query, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// 송금 예약 리스트
        /// </summary>  
        /// <returns></returns>
        [HttpPost("remit-wait-list")]
        [ProducesResponseType(typeof(ApiResponse<GetSellerRemitWaitListResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSellerRemitWaitList(GetSellerRemitWaitListRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/seller/remit-wait-list [{Aid}]", Aid);

            var query = req.Adapt<GetSellerRemitWaitListQuery>();

            var result = await _mediator.Send(query, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// 송금 삭제 요청(송금 완료 건, 송금 삭제 건은 삭제 불가)
        /// </summary>  
        /// <returns></returns>
        [HttpPatch("remit-delete")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteSellerRemit(DeleteSellerRemitRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PATCH /api/seller/remit-delete [{Aid}]", Aid);

            var command = req.Adapt<DeleteSellerRemitCommand>();

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// 셀러 활성 / 비활성 처리
        /// </summary>  
        /// <returns></returns>
        [HttpPatch("seller-enable")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateSellerRemitEnabled(UpdateSellerRemitEnabledRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PATCH /api/seller/seller-enable [{Aid}]", Aid);

            var command = req.Adapt<UpdateSellerRemitEnabledCommand>();

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }
    }
}
