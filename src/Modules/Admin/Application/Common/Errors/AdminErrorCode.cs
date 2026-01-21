using System.ComponentModel;

namespace Hello100Admin.Modules.Admin.Application.Common.Errors
{
    /// <summary>
    /// 범위 : 4001 ~ 8000 
    /// 추후 에러코드 관련 정의 필요 [Module당 Number or 별도 Code 세팅]
    /// </summary>
    public enum AdminErrorCode
    {
        [Description("비밀번호 변경에 실패하였습니다. 다시 시도해주세요.")]
        PasswordChangeFailed = 4001,
        [Description("비대면 진료 결제 내역을 찾을 수 없습니다.")]
        NotFoundUntactMedicalPayment = 4002,
        [Description("카카오 비즈 데이터 요청 간 에러가 발생하였습니다. 다시 시도해주세요.")]
        KakaoBizDataRequestFailed = 4003,
        [Description("병원 정보를 찾지 못했습니다. 다시 로그인해주세요.")]
        NotFoundCurrentHospital = 4004,
        [Description("비즈사이트 카카오 알림톡 서비스 신청에 실패하였습니다. 다시 시도해주세요.")]
        RequestKakaoAlimTalkServiceFailed = 4005,
    }
}
