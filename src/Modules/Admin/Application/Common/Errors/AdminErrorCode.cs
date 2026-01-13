using System.ComponentModel;

namespace Hello100Admin.Modules.Admin.Application.Common.Errors
{
    /// <summary>
    /// 범위 : 4001 ~ 8000 
    /// 추후 에러코드 관련 정의 필요 [Module당 Number or 별도 Code 세팅]
    /// </summary>
    public enum AdminErrorCode
    {
        [Description("임시 description 입니다.")]
        TempEnum = 4001,
    }
}
