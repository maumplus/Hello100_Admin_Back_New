using Hello100Admin.Modules.Admin.Domain.Entities;

namespace Hello100Admin.Modules.Admin.Application.Common.ReadModels
{
    /// <summary>
    /// TB_ADMIN 기준 확장 Model
    /// 추가 상속 금지, Response 사용 금지
    /// </summary>
    public sealed class GetAdminInfoReadModel : TbAdminEntity
    {
        public string HospKey { get; set; } = default!;
    }
}
