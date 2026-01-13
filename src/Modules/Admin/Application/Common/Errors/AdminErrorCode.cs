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
    }
}
