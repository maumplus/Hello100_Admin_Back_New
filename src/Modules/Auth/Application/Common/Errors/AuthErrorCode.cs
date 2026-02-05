using System.ComponentModel;

namespace Hello100Admin.Modules.Auth.Application.Common.Errors
{
    /// <summary>
    /// 범위 : 8001 ~ 9000
    /// 추후 에러코드 관련 정의 필요 [Module당 Number or 별도 Code 세팅]
    /// </summary>
    public enum AuthErrorCode
    {
        [Description("로그인을 위해서는 전화번호 혹은 이메일 둘 중 하나는 필수입니다. 확인 후 다시 시도해주세요.")]
        NotFoundPhoneAndEmail = 8001,
    }
}
