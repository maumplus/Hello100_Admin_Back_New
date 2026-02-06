using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.RequestsManagement.Results;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence
{
    public interface IRequestsManagementStore
    {
        #region 전체관리자 > 요청사항관리 > 잘못된정보 수정요청
        public Task<ListResult<GetRequestsResult>> GetRequestsAsync(string hospKey, int pageSize, int pageNum, string? apprYn, CancellationToken token);

        public Task<ListResult<GetRequestBugsResult>> GetRequestBugsAsync(int pageSize, int pageNum, bool apprYn, CancellationToken token);

        public Task<GetRequestBugResult> GetRequestBugAsync(long hpId, CancellationToken token);
        #endregion
    }
}
