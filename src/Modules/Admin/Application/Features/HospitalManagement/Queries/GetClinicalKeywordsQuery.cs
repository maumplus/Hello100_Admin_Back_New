using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Queries
{
    /// <summary>
    /// 증상/검진 키워드 조회 쿼리
    /// </summary>
    /// <param name="Keyword"></param>
    /// <param name="MasterSeq"></param>
    public record GetClinicalKeywordsQuery(string? Keyword, string? MasterSeq) : IQuery<Result<List<GetClinicalKeywordsResult>>>;

    public class GetClinicalKeywordsQueryHandler : IRequestHandler<GetClinicalKeywordsQuery, Result<List<GetClinicalKeywordsResult>>>
    {
        private readonly ILogger<GetClinicalKeywordsQueryHandler> _logger;
        private readonly IHospitalManagementStore _hospitalStore;
        private readonly IDbSessionRunner _db;

        public GetClinicalKeywordsQueryHandler(
            ILogger<GetClinicalKeywordsQueryHandler> logger,
            IHospitalManagementStore hospitalStore,
            IDbSessionRunner db)
        {
            _logger = logger;
            _hospitalStore = hospitalStore;
            _db = db;
        }

        public async Task<Result<List<GetClinicalKeywordsResult>>> Handle(GetClinicalKeywordsQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handling GetClinicalKeywordsQuery");

            var result = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _hospitalStore.GetClinicalKeywordsAsync(session, req.Keyword, req.MasterSeq, token),
            ct);

            return Result.Success(result);
        }
    }
}
