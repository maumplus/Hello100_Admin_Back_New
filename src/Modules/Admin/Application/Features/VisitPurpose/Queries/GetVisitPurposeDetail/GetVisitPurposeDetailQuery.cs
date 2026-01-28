using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Responses.GetVisitPurposeDetail;

namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Queries.GetVisitPurposeDetail
{
    /// <summary>
    /// 내원목적관리 > 내원목적편집 조회
    /// </summary>
    /// <param name="VpCd">내원 키</param>
    /// <param name="HospKey">요양기관 키</param>
    /// <param name="HospNo">요양기관번호</param>
    public record GetVisitPurposeDetailQuery(string VpCd, string HospKey, string HospNo) : IQuery<Result<GetVisitPurposeDetailResponse>>;
}
