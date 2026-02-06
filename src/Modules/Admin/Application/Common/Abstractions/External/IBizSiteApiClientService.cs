using Hello100Admin.Modules.Admin.Application.Common.Constracts.External.Web.BizSite;
using Hello100Admin.Modules.Admin.Application.Common.Models;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.External
{
    public interface IBizSiteApiClientService
    {
        /// <summary>
        /// 카카오 알림톡 정보 조회
        /// </summary>
        /// <param name="hospNo"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<BizSiteApiResult<KakaoMsgResultData>> PostKakaoMessageInfoAsync(string hospNo, CancellationToken ct);

        /// <summary>
        /// 카카오 검진 알림톡 정보 조회
        /// </summary>
        /// <param name="hospNo"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<KakaoMsgExaminationResultData> PostKakaoMessageExamiantionInfoAsync(string hospNo, CancellationToken ct);
    }
}
