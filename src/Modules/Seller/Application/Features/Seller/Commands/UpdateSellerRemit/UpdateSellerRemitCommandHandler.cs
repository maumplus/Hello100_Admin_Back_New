using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.Modules.Seller.Application.Common.Abstractions.External;
using Hello100Admin.Modules.Seller.Application.Common.Abstractions.Persistence.Seller;
using Hello100Admin.Modules.Seller.Application.Common.Constracts.External.Web.Seller.Request;
using Hello100Admin.Modules.Seller.Application.Common.Errors;
using Hello100Admin.Modules.Seller.Application.Common.Extensions;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.UpdateSellerRemit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.UpdateSellerRemit
{
    public class UpdateSellerRemitCommandHandler : IRequestHandler<UpdateSellerRemitCommand, Result<UpdateSellerRemitResponse>>
    {
        private readonly ILogger<UpdateSellerRemitCommandHandler> logger;
        private readonly ISellerRepository _sellerRepository;
        private readonly ISellerStore _sellerStore;
        private readonly IKcpRemitService _kcpRemitService;

        public UpdateSellerRemitCommandHandler(ILogger<UpdateSellerRemitCommandHandler> logger, 
                                               ISellerRepository sellerRepository, 
                                               ISellerStore sellerStore,
                                               IKcpRemitService kcpRemitService)
        {
            this.logger = logger;
            _sellerRepository = sellerRepository;
            _sellerStore = sellerStore;
            _kcpRemitService = kcpRemitService;
        }

        public async Task<Result<UpdateSellerRemitResponse>> Handle(UpdateSellerRemitCommand command, CancellationToken cancellationToken)
        {
            logger.LogDebug("Processing update seller remit [{Id}]", command.Id);

            var remitWaitInfo = await _sellerStore.GetHospSellerRemitWaitInfoAsync(command.Id, cancellationToken);

            if (remitWaitInfo == null || remitWaitInfo.Status != "0" || remitWaitInfo.IsSync != "1" || remitWaitInfo.Enabled != "1")
            {
                var failedResult = new UpdateSellerRemitResponse
                {
                    ResCd = "9001",
                    ResMsg = "요청 불가한 상태입니다.",
                    ResEnMsg = "Invalid request state"
                };

                return Result<UpdateSellerRemitResponse>.Success(failedResult).WithError(SellerErrorCode.InvalidStateForRequest.ToError());
            }

            // KCP 송금 요청
            var kcpReq = new RemitRequest
            {
                PayMethod = "VCNT",
                RemitType = "S",
                Currency = "410",
                SellerId = remitWaitInfo.SellerId,
                Amount = remitWaitInfo.Amount.ToString(),
                VaTxtype = "48200000",
                VaName = "（주）이지스헬스케어",
                VaMny = remitWaitInfo.Amount.ToString()
            };

            var kcpResult = await _kcpRemitService.SendRemitAsync(kcpReq);

            if (kcpResult == null)
            {
                var failedResult = new UpdateSellerRemitResponse
                {
                    ResCd = "9999",
                    ResMsg = "KCP 응답 없음",
                    ResEnMsg = "No response from KCP"
                };

                return Result<UpdateSellerRemitResponse>.Success(failedResult).WithError(SellerErrorCode.KcpNoResponse.ToError());
            }

            var updateParams = new UpdateSellerRemitParams
            {
                ResCd = kcpResult.ResCd,
                ResMsg = kcpResult.ResMsg,
                ResEnMsg = kcpResult.ResEnMsg,
                TradeSeq = kcpResult.TradeSeq,
                TradeDate = kcpResult.TradeDate,
                Amount = kcpResult.Amount,
                BalAmount = kcpResult.BalAmount,
                BankCode = kcpResult.BankCode,
                BankName = kcpResult.BankName,
                Account = kcpResult.Account,
                AppTime = kcpResult.AppTime,
                VanAppTime = kcpResult.VanApptime,
            };

            var updateCount = await _sellerRepository.UpdateSellerRemitAsync(updateParams, command.Id, command.Etc, cancellationToken);

            var result = new UpdateSellerRemitResponse
            {
                ResCd = updateCount > 0 ? kcpResult.ResCd : "9002",
                ResMsg = updateCount > 0 ? kcpResult.ResMsg : "DB 저장 실패",
                ResEnMsg = updateCount > 0 ? kcpResult.ResEnMsg : "DB update failed",
                TradeSeq = kcpResult.TradeSeq,
                TradeDate = kcpResult.TradeDate,
                Amount = kcpResult.Amount,
                BalAmount = kcpResult.BalAmount,
                BankCode = kcpResult.BankCode,
                BankName = kcpResult.BankName,
                Account = kcpResult.Account,
                AppTime = kcpResult.AppTime,
                VanAppTime = kcpResult.VanApptime,
            };

            return Result<UpdateSellerRemitResponse>.Success(result);
        }
    }
}
