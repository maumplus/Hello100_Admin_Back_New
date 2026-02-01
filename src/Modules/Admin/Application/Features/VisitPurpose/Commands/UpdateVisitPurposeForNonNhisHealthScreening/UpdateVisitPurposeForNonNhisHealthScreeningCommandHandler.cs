using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.VisitPurpose;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using static Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.UpdateVisitPurposeForNonNhisHealthScreening.UpdateVisitPurposeForNonNhisHealthScreeningParams;

namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.UpdateVisitPurposeForNonNhisHealthScreening
{
    public class UpdateVisitPurposeForNonNhisHealthScreeningCommandHandler : IRequestHandler<UpdateVisitPurposeForNonNhisHealthScreeningCommand, Result>
    {
        private readonly ILogger<UpdateVisitPurposeForNonNhisHealthScreeningCommandHandler> _logger;
        private readonly IVisitPurposeRepository _visitPurposeRepository;
        private readonly IVisitPurposeStore _visitPurposeStore;
        private readonly IDbSessionRunner _db;
        private readonly ICryptoService _cryptoService;

        public UpdateVisitPurposeForNonNhisHealthScreeningCommandHandler(
            ILogger<UpdateVisitPurposeForNonNhisHealthScreeningCommandHandler> logger,
            IVisitPurposeRepository visitPurposeRepository,
            IVisitPurposeStore visitPurposeStore,
            IDbSessionRunner db,
            ICryptoService cryptoService)
        {
            _logger = logger;
            _visitPurposeRepository = visitPurposeRepository;
            _visitPurposeStore = visitPurposeStore;
            _db = db;
            _cryptoService = cryptoService;
        }

        public async Task<Result> Handle(UpdateVisitPurposeForNonNhisHealthScreeningCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Process UpdateVisitPurposeForNonNhisHealthScreeningCommandHandler started.");

            if (req.ShowYn == "N")
            {
                var visitPurpose = await _visitPurposeStore.GetVisitPurposesAsync(req.HospKey, ct);

                int purposeViewCount = visitPurpose.DetailList.Count(x => x.ShowYn == "Y");

                if (purposeViewCount == 1)
                {
                    return Result.Success().WithError(AdminErrorCode.VisitPurposeExposureRequired.ToError());
                }
            }

            var data = new UpdateVisitPurposeForNonNhisHealthScreeningParams();

            data.Purpose = req.Adapt<UpdateVisitPurposeBizParams>();
            data.Purpose.TranId = _cryptoService.EncryptToBase64WithDesEcbPkcs7(req.VpCd);

            if (req.DetailYn == "Y" && req.Details != null)
            {
                req.Details.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x))
                    {
                        data.Details.Add(new UpdateVisitPurposeBizDetailsParams
                        {
                            Name = x
                        });
                    }
                });
            }

            await _db.RunInTransactionAsync(DataSource.Hello100, async session =>
            {
                // 승인요청정보 테이블에 저장
                var createdApprId = await _visitPurposeRepository.CreateVisitPurposeApprovalAsync(session, req.HospKey, "HR", data.ToJsonForStorage(), req.AId, ct);

                // 이지스병원내원목적정보 테이블에 저장
                await _visitPurposeRepository.UpdateVisitPurposeForNonNhisHealthScreeningAsync(session, req, createdApprId, ct);
            }, ct);


            // 현재 운영에서 정상 동작하지 않는 것으로 확인되어 해당 내용 삭제
            // "hello desk update perpose" 라는 PUSH 알림 전송하는 기능
            //if (hospInfo.TabletCnt > 0)
            //    await SendPushDeviceUpdate(userInfo.HospKey, userInfo.HospNo, "purpose");

            _logger.LogInformation("Visit purpose for non-NHIS health screening updated successfully.");
            return Result.Success();
        }
    }
}
