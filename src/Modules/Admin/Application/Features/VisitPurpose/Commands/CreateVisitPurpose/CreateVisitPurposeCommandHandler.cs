using System.Text.Encodings.Web;
using System.Text.Json;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.VisitPurpose;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.CreateVisitPurpose
{
    public class CreateVisitPurposeCommandHandler : IRequestHandler<CreateVisitPurposeCommand, Result>
    {
        private readonly ILogger<CreateVisitPurposeCommandHandler> _logger;
        private readonly IVisitPurposeRepository _visitPurposeRepository;
        private readonly ICryptoService _cryptoService;
        private readonly IDbSessionRunner _db;

        public CreateVisitPurposeCommandHandler(ILogger<CreateVisitPurposeCommandHandler> logger,
                                                IVisitPurposeRepository visitPurposeRepository,
                                                ICryptoService cryptoService,
                                                IDbSessionRunner db)
        {
            _logger = logger;
            _visitPurposeRepository = visitPurposeRepository;
            _cryptoService = cryptoService;
            _db = db;
        }

        public async Task<Result> Handle(CreateVisitPurposeCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Process CreateVisitPurposeCommandHandler started.");

            var data = new CreateVisitPurposeParams();

            data.Purpose = req.Adapt<CreateVisitPurposeBizParams>();

            if (req.DetailYn == "Y" && req.Details != null)
            {
                req.Details.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x))
                    {
                        data.Details.Add(new CreateVisitPurposeBizDetailsParams
                        {
                            Name = x
                        });
                    }
                });
            }

            await _db.RunInTransactionAsync(DataSource.Hello100, 
                async (session, token) =>
                {
                    // 승인요청정보 테이블에 저장
                    var createdApprId = await _visitPurposeRepository.CreateVisitPurposeApprovalAsync(session, req.HospKey, "HR", data.ToJsonForStorage(), req.AId, token);

                    // 이지스병원내원목적정보 테이블에 저장
                    await _visitPurposeRepository.CreateVisitPurposeAsync(session, req, createdApprId, token);
                },
            ct);

            // 현재 운영에서 정상 동작하지 않는 것으로 확인되어 해당 내용 삭제
            // "hello desk update perpose" 라는 PUSH 알림 전송하는 기능
            //if (hospInfo.TabletCnt > 0)
            //    await SendPushDeviceUpdate(userInfo.HospKey, userInfo.HospNo, "purpose");

            return Result.Success();
        }
    }
}
