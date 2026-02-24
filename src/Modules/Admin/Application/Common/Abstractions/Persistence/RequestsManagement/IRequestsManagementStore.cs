using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.RequestsManagement.Results;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence
{
    public interface IRequestsManagementStore
    {
        #region 전체관리자 > 요청사항관리 > 잘못된정보 수정요청

        public Task<ListResult<GetRequestBugsResult>> GetRequestBugsAsync(int pageSize, int pageNum, bool apprYn, CancellationToken token);

        public Task<GetRequestBugResult> GetRequestBugAsync(long hpId, CancellationToken token);

        public Task<ListResult<GetRequestUntactsResult>> GetRequestUntactsAsync(
            int pageSize, int pageNum, int searchType, int searchDateType, string? fromDate, string? toDate, string? searchKeyword, List<string> joinState, bool isExcel, CancellationToken token);

        public Task<GetRequestUntactResult> GetRequestUntactAsync(int seq, string rootUrl, CancellationToken token);
        #endregion
    }
}
