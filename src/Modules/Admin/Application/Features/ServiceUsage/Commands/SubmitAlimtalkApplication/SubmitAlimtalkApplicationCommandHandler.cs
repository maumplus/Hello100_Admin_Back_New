using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.External;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Common;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ServiceUsage;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Commands.SubmitAlimtalkApplication
{
    public class SubmitAlimtalkApplicationCommandHandler : IRequestHandler<SubmitAlimtalkApplicationCommand, Result>
    {
        private readonly ILogger<SubmitAlimtalkApplicationCommandHandler> _logger;
        private readonly IServiceUsageRepository _serviceUsageRepository;
        private readonly IHospitalInfoProvider _hospitalInfoProvider;
        private readonly IEghisHomeApiClientService _eghisHomeApiClientService;
        private readonly IDbSessionRunner _db;

        public SubmitAlimtalkApplicationCommandHandler(ILogger<SubmitAlimtalkApplicationCommandHandler> logger,
                                                       IServiceUsageRepository serviceUsageRepository,
                                                       IHospitalInfoProvider hospitalInfoProvider,
                                                       IEghisHomeApiClientService eghisHomeApiClientService,
                                                       IDbSessionRunner db)
        {
            _logger = logger;
            _serviceUsageRepository = serviceUsageRepository;
            _hospitalInfoProvider = hospitalInfoProvider;
            _eghisHomeApiClientService = eghisHomeApiClientService;
            _db = db;
        }

        public async Task<Result> Handle(SubmitAlimtalkApplicationCommand command, CancellationToken ct)
        {
            _logger.LogInformation("Process SubmitAlimtalkApplicationCommandHandler() started.");

            await _serviceUsageRepository.SubmitAlimtalkApplicationAsync(command, ct);

            var hospInfo = await _hospitalInfoProvider.GetHospitalInfoByHospNoAsync(command.HospNo, ct);

            if (hospInfo == null)
                return Result.Success().WithError(AdminErrorCode.NotFoundCurrentHospital.ToError());

            try
            {
                await _eghisHomeApiClientService.RequestKakaoAlimTalkServiceAsync(command.HospNo, hospInfo.ChartType, command.TmpType, ct);
            }
            catch (Exception)
            {
                // 요청 실패 시, 알림톡 발송 서비스 신청 정보 삭제
                await _db.RunAsync(DataSource.Hello100,
                    (session, token) => _serviceUsageRepository.DeleteAlimtalkApplicationAsync(session, command.HospNo, command.HospKey, command.TmpType, token),
                ct);

                return Result.Success().WithError(AdminErrorCode.RequestKakaoAlimTalkServiceFailed.ToError());
            }

            return Result.Success();
        }
    }
}
