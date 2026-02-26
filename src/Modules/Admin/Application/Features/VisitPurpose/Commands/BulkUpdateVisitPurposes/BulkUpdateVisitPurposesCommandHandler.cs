using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.VisitPurpose;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.BulkUpdateVisitPurposes
{
    public class BulkUpdateVisitPurposesCommandHandler : IRequestHandler<BulkUpdateVisitPurposesCommand, Result>
    {
        private readonly ILogger<BulkUpdateVisitPurposesCommandHandler> _logger;
        private readonly IVisitPurposeRepository _visitPurposeRespository;

        public BulkUpdateVisitPurposesCommandHandler(ILogger<BulkUpdateVisitPurposesCommandHandler> logger, 
                                                     IVisitPurposeRepository visitPurposeRepository)
        {
            _logger = logger;
            _visitPurposeRespository = visitPurposeRepository;
        }

        public async Task<Result> Handle(BulkUpdateVisitPurposesCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Process BulkUpdateVisitPurposesCommandHandler started.");

            var showYItems = req.Items.Where(x => x.ShowYn == "Y").ToList();

            if (showYItems.Count <= 0)
                return Result.Success().WithError(AdminErrorCode.VisitPurposeExposureRequired.ToError());

            await _visitPurposeRespository.BulkUpdateVisitPurposesAsync(req, ct);

            // 현재 운영에서 정상 동작하지 않는 것으로 확인되어 해당 내용 삭제
            // "hello desk update perpose" 라는 PUSH 알림 전송하는 기능
            //if (hospInfo.TabletCnt > 0)
            //    await SendPushDeviceUpdate(userInfo.HospKey, userInfo.HospNo, "purpose");

            return Result.Success();
        }
    }
}
