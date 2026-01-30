using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.HospitalUser.Results;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence
{
    public interface IHospitalUserStore
    {
        /// <summary>
        /// [전체관리자] 회원목록 > 조회
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="fromDt"></param>
        /// <param name="toDt"></param>
        /// <param name="keywordSearchType"></param>
        /// <param name="searchKeyword"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<ListResult<SearchHospitalUsersResult>> SearchHospitalUsersAsync(
            int pageNo, int pageSize, string? fromDt, string? toDt, int keywordSearchType, string? searchKeyword, CancellationToken token);
    }
}
