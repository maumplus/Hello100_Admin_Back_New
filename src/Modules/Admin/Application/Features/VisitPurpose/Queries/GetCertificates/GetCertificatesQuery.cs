using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Responses.GetCertificates;

namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Queries.GetCertificates
{
    /// <summary>
    /// 제증명문서관리 조회 쿼리
    /// </summary>
    /// <param name="HospKey">요양기관 키</param>
    public record GetCertificatesQuery(string HospKey) : IQuery<Result<GetCertificatesResponse>>;
}
