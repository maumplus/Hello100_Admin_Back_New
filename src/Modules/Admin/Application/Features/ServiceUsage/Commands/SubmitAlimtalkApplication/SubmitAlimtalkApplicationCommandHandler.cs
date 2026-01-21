using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.External;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Common;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ServiceUsage;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Commands.SubmitAlimtalkApplication
{
    public class SubmitAlimtalkApplicationCommandHandler : IRequestHandler<SubmitAlimtalkApplicationCommand, Result>
    {
        private readonly ILogger<SubmitAlimtalkApplicationCommandHandler> _logger;
        private readonly IServiceUsageRepository _serviceUsageRepository;
        private readonly ICurrentHospitalProfileProvider _currentHospitalProfileProvider;
        private readonly IEghisHomeApiClientService _eghisHomeApiClientService;

        public SubmitAlimtalkApplicationCommandHandler(ILogger<SubmitAlimtalkApplicationCommandHandler> logger,
                                                       IServiceUsageRepository serviceUsageRepository,
                                                       ICurrentHospitalProfileProvider currentHospitalProfileProvider,
                                                       IEghisHomeApiClientService eghisHomeApiClientService)
        {
            _logger = logger;
            _serviceUsageRepository = serviceUsageRepository;
            _currentHospitalProfileProvider = currentHospitalProfileProvider;
            _eghisHomeApiClientService = eghisHomeApiClientService;
        }

        public async Task<Result> Handle(SubmitAlimtalkApplicationCommand command, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Process SubmitAlimtalkApplicationCommandHandler() started.");

            await _serviceUsageRepository.SubmitAlimtalkApplicationAsync(command, cancellationToken);

            var hospInfo = await _currentHospitalProfileProvider.GetCurrentHospitalProfileByHospNoAsync(command.HospNo, cancellationToken);

            await _eghisHomeApiClientService.RequestKakaoAlimTalkServiceAsync(command.HospNo, hospInfo.ChartType, command.TmpType, cancellationToken);

            return Result.Success();
        }
    }
}
