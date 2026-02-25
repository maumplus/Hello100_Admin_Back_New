using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Queries
{
    public record GetSymptomExamKeywordsQuery() : IQuery<Result<List<GetSymptomExamKeywordsResult>>>;

    public class GetSymptomExamKeywordsQueryHandler : IRequestHandler<GetSymptomExamKeywordsQuery, Result<List<GetSymptomExamKeywordsResult>>>
    {
        private readonly ILogger<GetSymptomExamKeywordsQueryHandler> _logger;
        private readonly IHospitalManagementStore _hospitalManagementStore;
        private readonly IDbSessionRunner _db;

        public GetSymptomExamKeywordsQueryHandler(ILogger<GetSymptomExamKeywordsQueryHandler> logger, IHospitalManagementStore hospitalManagementStore, IDbSessionRunner db)
        {
            _logger = logger;
            _hospitalManagementStore = hospitalManagementStore;
            _db = db;
        }

        public async Task<Result<List<GetSymptomExamKeywordsResult>>> Handle(GetSymptomExamKeywordsQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handling GetSymptomExamKeywordsQueryHandler");

            var response = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _hospitalManagementStore.GetSymptomExamKeywordsAsync(session, token),
            ct);

            return Result.Success(response);
        }
    }
}
