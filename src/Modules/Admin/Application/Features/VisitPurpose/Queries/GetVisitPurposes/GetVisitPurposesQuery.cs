using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Responses.GetVisitPurposes;

namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Queries.GetVisitPurposes
{
    /// <summary>
    /// 내원 목적 관리 조회 쿼리
    /// </summary>
    /// <param name="HospKey">요양병원 키</param>
    public record GetVisitPurposesQuery(string HospKey) : IQuery<Result<GetVisitPurposesResponse>>;
}
