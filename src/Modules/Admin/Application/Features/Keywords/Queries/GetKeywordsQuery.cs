using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Features.Keywords.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.Keywords.Queries
{
    /// <summary>
    /// 증상/검진 키워드 조회 쿼리
    /// </summary>
    /// <param name="Keyword"></param>
    /// <param name="MasterSeq"></param>
    public record GetKeywordsQuery(string? Keyword, string? MasterSeq) : IQuery<Result<List<GetKeywordsResult>>>;

    public class GetClinicalKeywordsQueryHandler : IRequestHandler<GetKeywordsQuery, Result<List<GetKeywordsResult>>>
    {
        private readonly ILogger<GetClinicalKeywordsQueryHandler> _logger;
        private readonly IKeywordsStore _keywordsStore;
        private readonly IDbSessionRunner _db;

        public GetClinicalKeywordsQueryHandler(
            ILogger<GetClinicalKeywordsQueryHandler> logger,
            IKeywordsStore keywordsStore,
            IDbSessionRunner db)
        {
            _logger = logger;
            _keywordsStore = keywordsStore;
            _db = db;
        }

        public async Task<Result<List<GetKeywordsResult>>> Handle(GetKeywordsQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handling GetClinicalKeywordsQuery");

            var result = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _keywordsStore.GetKeywordsAsync(session, req.Keyword, req.MasterSeq, token),
            ct);

            return Result.Success(result);
        }
    }
}
