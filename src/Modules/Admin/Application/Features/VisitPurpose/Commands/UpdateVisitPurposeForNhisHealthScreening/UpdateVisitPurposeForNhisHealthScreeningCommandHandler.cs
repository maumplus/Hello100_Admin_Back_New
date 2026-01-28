using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.VisitPurpose;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.UpdateVisitPurposeForNhisHealthScreening
{
    public class UpdateVisitPurposeForNhisHealthScreeningCommandHandler : IRequestHandler<UpdateVisitPurposeForNhisHealthScreeningCommand, Result>
    {
        private readonly ILogger<UpdateVisitPurposeForNhisHealthScreeningCommandHandler> _logger;
        private readonly IVisitPurposeRepository _visitPurposeRepository;
        private readonly IVisitPurposeStore _visitPurposeStore;

        public UpdateVisitPurposeForNhisHealthScreeningCommandHandler(
            ILogger<UpdateVisitPurposeForNhisHealthScreeningCommandHandler> logger,
            IVisitPurposeRepository visitPurposeRepository,
            IVisitPurposeStore visitPurposeStore)
        {
            _logger = logger;
            _visitPurposeRepository = visitPurposeRepository;
            _visitPurposeStore = visitPurposeStore;
        }

        public async Task<Result> Handle(UpdateVisitPurposeForNhisHealthScreeningCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Process UpdateVisitPurposeForNhisHealthScreeningCommandHandler started. HospKey: {HospKey}", req.HospKey);

            if (req.ShowYn == "N")
            {
                var visitPurpose = await _visitPurposeStore.GetVisitPurposesAsync(req.HospKey, ct);

                int purposeViewCount = visitPurpose.DetailList.Count(x => x.ShowYn == "Y");

                if (purposeViewCount == 1)
                {
                    return Result.Success().WithError(AdminErrorCode.VisitPurposeExposureRequired.ToError());
                }
            }

            await _visitPurposeRepository.UpdateVisitPurposeForNhisHealthScreeningAsync(req.HospKey, req.ShowYn, req.Role, req.DetailShowYn, ct);

            // 현재 운영에서 정상 동작하지 않는 것으로 확인되어 해당 내용 삭제
            // "hello desk update perpose" 라는 PUSH 알림 전송하는 기능
            //if (hospInfo.TabletCnt > 0)
            //    await SendPushDeviceUpdate(userInfo.HospKey, userInfo.HospNo, "purpose");

            _logger.LogInformation("Visit purpose for NHIS health screening updated successfully for HospKey: {HospKey}", req.HospKey);

            return Result.Success();
        }
    }
}
