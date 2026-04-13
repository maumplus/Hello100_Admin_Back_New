using Hello100Admin.Modules.Auth.Domain.Entities;

namespace Hello100Admin.Modules.Auth.Application.Common.Views
{
    /// <summary>
    /// TB_ADMIN 기준 확장 Model
    /// 추가 상속 금지, Response 사용 금지
    /// </summary>
    public class TbAdminView : TbAdminEntity
    {
        /// <summary>
        /// 요양기관키
        /// </summary>
        public string? HospKey { get; set; }
    }
}
