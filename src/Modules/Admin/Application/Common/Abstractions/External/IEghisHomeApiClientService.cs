namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.External
{
    public interface IEghisHomeApiClientService
    {
        /// <summary>
        /// 비즈 홈페이지로 카카오 알림톡 서비스 신청
        /// </summary>
        /// <param name="hospNo">요양기관번호</param>
        /// <param name="chartType">차트 유형</param>
        /// <param name="tmpType">신청 유형 ["": 알림톡 발송 서비스 신청(진료접수), "KakaoJoinTestResult": 알림톡 발송 서비스 신청(검사결과)]</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task RequestKakaoAlimTalkServiceAsync(string hospNo, string chartType, string tmpType, CancellationToken ct);
    }
}
