using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Features.ApprovalRequest.Responses;

namespace Hello100Admin.Modules.Admin.Application.Features.ApprovalRequest.Queries.GetUntactMedicalRequestDetailForApproval
{
    /// <summary>
    /// 비대면 진료 승인 요청 상세 조회 쿼리
    /// </summary>
    /// <param name="Seq"></param>
    public record GetUntactMedicalRequestDetailForApprovalQuery(int Seq) : IQuery<Result<GetUntactMedicalRequestDetailForApprovalResponse>>;
}
