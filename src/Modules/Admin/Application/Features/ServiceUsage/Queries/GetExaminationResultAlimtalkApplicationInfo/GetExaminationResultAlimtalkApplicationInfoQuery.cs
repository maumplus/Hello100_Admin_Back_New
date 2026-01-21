using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Responses.GetExaminationResultAlimtalkApplicationInfo;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.GetExaminationResultAlimtalkApplicationInfo
{
    /// <summary>
    /// 검사결과 알림톡 신청 정보 조회 Query
    /// </summary>
    /// <param name="HospNo">요양기관번호</param>
    public record GetExaminationResultAlimtalkApplicationInfoQuery(string HospNo) : IQuery<Result<GetExaminationResultAlimtalkApplicationInfoResponse>>;
}
