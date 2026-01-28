using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ServiceUsage;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Responses.SearchUntactMedicalHistories;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.SearchUntactMedicalHistories
{
    public class SearchUntactMedicalHistoriesQueryHandler : IRequestHandler<SearchUntactMedicalHistoriesQuery, Result<SearchUntactMedicalHistoriesResponse?>>
    {
        private readonly ILogger<SearchUntactMedicalHistoriesQueryHandler> _logger;
        private readonly IServiceUsageStore _serviceUsageStore;
        private readonly ICryptoService _cryptoService;
        
        public SearchUntactMedicalHistoriesQueryHandler(ILogger<SearchUntactMedicalHistoriesQueryHandler> logger, IServiceUsageStore serviceUsageStore, ICryptoService cryptoService)
        {
            _logger = logger;
            _serviceUsageStore = serviceUsageStore;
            _cryptoService = cryptoService;
        }

        public async Task<Result<SearchUntactMedicalHistoriesResponse?>> Handle(SearchUntactMedicalHistoriesQuery req, CancellationToken token)
        {
            _logger.LogInformation("Process GetUntactMedicalHistoryQuery() started.");

            var historyInfo = await _serviceUsageStore.SearchUntactMedicalHistoriesAsync(req, token);

            historyInfo.DetailList.ForEach(s => s.Name = _cryptoService.DecryptWithNoVector(s.Name, CryptoKeyType.Name));

            var result = historyInfo?.Adapt<SearchUntactMedicalHistoriesResponse>();

            return Result.Success(result);
        }
    }
}
