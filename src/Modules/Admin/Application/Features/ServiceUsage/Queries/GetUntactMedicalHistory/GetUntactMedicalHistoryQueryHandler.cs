using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ServiceUsage;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Responses.GetUntactMedicalHistory;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.GetUntactMedicalHistory
{
    public class GetUntactMedicalHistoryQueryHandler : IRequestHandler<GetUntactMedicalHistoryQuery, Result<GetUntactMedicalHistoryResponse?>>
    {
        private readonly ILogger<GetUntactMedicalHistoryQueryHandler> _logger;
        private readonly IServiceUsageStore _serviceUsageStore;
        private readonly ICryptoService _cryptoService;
        
        public GetUntactMedicalHistoryQueryHandler(ILogger<GetUntactMedicalHistoryQueryHandler> logger, IServiceUsageStore serviceUsageStore, ICryptoService cryptoService)
        {
            _logger = logger;
            _serviceUsageStore = serviceUsageStore;
            _cryptoService = cryptoService;
        }

        public async Task<Result<GetUntactMedicalHistoryResponse?>> Handle(GetUntactMedicalHistoryQuery req, CancellationToken token)
        {
            _logger.LogInformation("Process GetUntactMedicalHistoryQuery() started.");

            var historyInfo = await _serviceUsageStore.GetUntactMedicalHistoryAsync(req, token);

            historyInfo?.DetailList.ForEach(s => s.Name = _cryptoService.DecryptWithNoVector(s.Name, CryptoKeyType.Name));

            var result = historyInfo?.Adapt<GetUntactMedicalHistoryResponse>();

            return Result.Success(result);
        }
    }
}
