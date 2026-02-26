

using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.Asset.Results;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence
{
    public interface IAssetStore
    {
        public Task<ListResult<GetUsageListResult>> GetUsageListAsync(
            int pageSize, int pageNo, int searchType, int searchDateType, string? fromDate, string? toDate, string? fromDay, string? toDay, string? searchKeyword
            , bool isExcel, CancellationToken token);
    }
}
