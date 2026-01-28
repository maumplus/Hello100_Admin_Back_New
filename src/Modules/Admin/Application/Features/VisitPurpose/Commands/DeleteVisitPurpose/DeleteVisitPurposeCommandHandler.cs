using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.VisitPurpose;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.DeleteVisitPurpose
{
    public class DeleteVisitPurposeCommandHandler : IRequestHandler<DeleteVisitPurposeCommand, Result>
    {
        private readonly ILogger<DeleteVisitPurposeCommandHandler> _logger;
        private readonly IVisitPurposeRepository _visitPurposeRepository;
        public DeleteVisitPurposeCommandHandler(ILogger<DeleteVisitPurposeCommandHandler> logger,
                                                IVisitPurposeRepository visitPurposeRepository)
        {
            _logger = logger;
            _visitPurposeRepository = visitPurposeRepository;
        }

        public async Task<Result> Handle(DeleteVisitPurposeCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Process DeleteVisitPurposeCommandHandler started.");

            await _visitPurposeRepository.DeleteVisitPurposeAsync(req.VpCd, req.HospKey, ct);

            // 현재 운영에서 정상 동작하지 않는 것으로 확인되어 해당 내용 삭제
            // "hello desk update perpose" 라는 PUSH 알림 전송하는 기능
            //if (hospInfo.TabletCnt > 0)
            //    await SendPushDeviceUpdate(userInfo.HospKey, userInfo.HospNo, "purpose");

            return Result.Success();
        }
    }
}
