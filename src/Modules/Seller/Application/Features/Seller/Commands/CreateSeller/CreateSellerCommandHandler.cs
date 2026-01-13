using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Seller.Application.Common.Abstractions.External;
using Hello100Admin.Modules.Seller.Application.Common.Abstractions.Persistence.Seller;
using Hello100Admin.Modules.Seller.Application.Common.Constracts;
using Hello100Admin.Modules.Seller.Application.Common.Constracts.External.Web.Seller.Request;
using Hello100Admin.Modules.Seller.Application.Common.Errors;
using Hello100Admin.Modules.Seller.Application.Common.Extensions;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.CreateSeller
{
    /// <summary>
    /// 송금 등록 커맨드 핸들러
    /// </summary>
    public class CreateSellerCommandHandler : IRequestHandler<CreateSellerCommand, Result>
    {
        private readonly ILogger<CreateSellerCommandHandler> _logger;
        private readonly ISellerRepository _sellerRepository;
        private readonly ISellerStore _sellerStore;
        private readonly ICryptoService _cryptoService;
        private readonly IKcpRemitService _kcpRemitService;

        public CreateSellerCommandHandler(
            ILogger<CreateSellerCommandHandler> logger,
            ISellerRepository sellerRepository,
            ISellerStore sellerStore,
            ICryptoService cryptoService,
            IKcpRemitService kcpRemitService)
        {
            _logger = logger;
            _sellerRepository = sellerRepository;
            _sellerStore = sellerStore;
            _cryptoService = cryptoService;
            _kcpRemitService = kcpRemitService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Result> Handle(CreateSellerCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Processing create seller AId: {aId}", command.AId);

            // 병원 조회
            var hospInfo = await _sellerStore.GetApprovedUntactHospitalInfoAsync(command.HospNo, cancellationToken);

            if (hospInfo == null)
            {
                return Result.SuccessWithError(SellerErrorCode.HospitalNotFound.ToError());
            }

            // 관리자 셀러 등록 된 병원 수 조회
            long sellerCount = await _sellerStore.GetHospitalSellerCountAsync(command.HospNo, cancellationToken);

            string sellerId = _cryptoService.Encrypt($"{command.HospNo}||{command.AId}||{sellerCount}", CryptoKeyType.Seller);

            // 송금 DB 등록
            var hospSellerId = await _sellerRepository.InsertTbHospSellerAsync(command, sellerId, cancellationToken);

            if (hospSellerId <= 0)
            {
                return Result.SuccessWithError(SellerErrorCode.HospitalSellerInsertError.ToError());
            }

            // Set Request Info
            var sellerReq = new SellerRegisterRequest
            {
                SellerId = sellerId,
                SellerName = hospInfo.HospName,
                OwnName = command.DepositNo,
                Address = hospInfo.HospAddr,
                TelNo = hospInfo.HospTel,
                SellerLevel = "CT01",//hospInfo.BusinessLevel,
                TaxNo = "1234123412", //hospInfo.BusinessNo,
                BankCd = command.BankCd,
                DepositNo = command.DepositNo,
                Depositor = command.Depositor,
            };

            // KCP 송금 요청
            var sellerResult = await _kcpRemitService.RegisterSellerAsync(sellerReq);

            if (sellerResult?.ResCd != "0000")
            {
                return Result.SuccessWithError(SellerErrorCode.KcpSellerSyncError.ToError());
            }
            else
            {
                // KCP 연동 상태 업데이트
                int primaryId = Convert.ToInt32(hospSellerId);

                int updateResult = await _sellerRepository.UpdateTbHospSellerIsSyncByIdAsync(primaryId, cancellationToken);

                if (updateResult < 0)
                {
                    return Result.SuccessWithError(SellerErrorCode.KcpSellerSyncUpdateError.ToError());
                }
            }

            _logger.LogInformation("Seller created successfully for AId: {aId}", command.AId);

            return Result.Success(); //Success : 1
        }
    }
}
