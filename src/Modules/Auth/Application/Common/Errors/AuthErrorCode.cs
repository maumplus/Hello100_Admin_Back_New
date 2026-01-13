using System.ComponentModel;

namespace Hello100Admin.Modules.Auth.Application.Common.Errors
{
    /// <summary>
    /// 범위 : 8001 ~ 9000
    /// 추후 에러코드 관련 정의 필요 [Module당 Number or 별도 Code 세팅]
    /// </summary>
    public enum AuthErrorCode
    {
        [Description("임시 description 입니다.")]
        TempEnum = 8001,
    }
}
