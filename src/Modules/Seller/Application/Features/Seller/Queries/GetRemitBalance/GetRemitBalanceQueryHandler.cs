using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Seller.Application.Common.Abstractions.External;
using Hello100Admin.Modules.Seller.Application.Common.Abstractions.Persistence.Seller;
using Hello100Admin.Modules.Seller.Application.Common.Constracts.External.Web.Seller.Request;
using Hello100Admin.Modules.Seller.Application.Common.Errors;
using Hello100Admin.Modules.Seller.Application.Common.Extensions;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.GetRemitBalance;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Queries.GetRemitBalance
{
    public class GetRemitBalanceQueryHandler : IRequestHandler<GetRemitBalanceQuery, Result<GetRemitBalanceResponse>>
    {
        private readonly ILogger<GetRemitBalanceQueryHandler> _logger;
        private readonly ISellerStore _sellerStore;
        private readonly IKcpRemitService _kcpRemitService;

        public GetRemitBalanceQueryHandler(ILogger<GetRemitBalanceQueryHandler> logger, ISellerStore sellerStore, IKcpRemitService kcpRemitService)
        {
            _logger = logger;
            _sellerStore = sellerStore;
            _kcpRemitService = kcpRemitService;
        }

        public async Task<Result<GetRemitBalanceResponse>> Handle(GetRemitBalanceQuery req, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Processing get remit balance");

            var kcpReq = new AccountBalanceRequest
            {
                PayMethod = "VCNT",
                Currency = "410",
                VaTxtype = "48100000"
            };

            var kcpResult = await _kcpRemitService.GetAccountBalanceAsync(kcpReq);

            if (kcpResult == null || kcpResult.ResCd != "0000")
            {
                return Result.SuccessWithError<GetRemitBalanceResponse>(SellerErrorCode.KcpBalanceInquiryFailed.ToError());
            }

            long.TryParse(kcpResult.CanAmount, out var canAmount);

            var result = kcpResult.Adapt<GetRemitBalanceResponse>() with { CanAmount = canAmount };

            return Result.Success(result);
        }
    }
}
