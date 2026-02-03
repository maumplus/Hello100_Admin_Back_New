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

        /// <summary>
        /// [전체관리자] 회원목록 > 상세 조회 (사용자 상세 정보)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<GetHospitalUserProfileResult> GetHospitalUserProfileAsync(string userId, CancellationToken token);

        /// <summary>
        /// [전체관리자] 회원목록 > 상세 조회 (가족 프로필 정보)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<List<GetHospitalUserProfileResultFamilyItem>> GetHospitalUserFamilyProfileAsync(string userId, CancellationToken token);

        /// <summary>
        /// [전체관리자] 회원목록 > 상세 조회 (사용자 및 가족 서비스 이용 현황)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<List<GetHospitalUserProfileResultServiceUsageItem>> GetHospitalUserAndFamilyServiceUsages(string userId, CancellationToken token);
    }
}
