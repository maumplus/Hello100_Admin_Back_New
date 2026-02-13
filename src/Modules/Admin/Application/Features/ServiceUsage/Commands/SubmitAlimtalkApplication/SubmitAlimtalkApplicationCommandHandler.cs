using Hello100Admin.BuildingBlocks.Common.Application;
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

        public SubmitAlimtalkApplicationCommandHandler(ILogger<SubmitAlimtalkApplicationCommandHandler> logger,
                                                       IServiceUsageRepository serviceUsageRepository,
                                                       IHospitalInfoProvider hospitalInfoProvider,
                                                       IEghisHomeApiClientService eghisHomeApiClientService)
        {
            _logger = logger;
            _serviceUsageRepository = serviceUsageRepository;
            _hospitalInfoProvider = hospitalInfoProvider;
            _eghisHomeApiClientService = eghisHomeApiClientService;
        }

        public async Task<Result> Handle(SubmitAlimtalkApplicationCommand command, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Process SubmitAlimtalkApplicationCommandHandler() started.");

            await _serviceUsageRepository.SubmitAlimtalkApplicationAsync(command, cancellationToken);

            var hospInfo = await _hospitalInfoProvider.GetHospitalInfoByHospNoAsync(command.HospNo, cancellationToken);

            if (hospInfo == null)
                return Result.Success().WithError(AdminErrorCode.NotFoundCurrentHospital.ToError());

            await _eghisHomeApiClientService.RequestKakaoAlimTalkServiceAsync(command.HospNo, hospInfo.ChartType, command.TmpType, cancellationToken);

            return Result.Success();
        }
    }
}
