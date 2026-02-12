using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Features.Keywords.Results;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence
{
    public interface IKeywordsStore
    {
        /// <summary>
        /// 증상/검진 키워드 조회
        /// </summary>
        /// <param name="db"></param>
        /// <param name="keyword"></param>
        /// <param name="masterSeq"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<List<GetKeywordsResult>> GetKeywordsAsync(DbSession db, string? keyword, string? masterSeq, CancellationToken ct = default);
    }
}
