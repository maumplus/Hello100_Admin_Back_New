using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Features.ApprovalRequest.Responses;

namespace Hello100Admin.Modules.Admin.Application.Features.ApprovalRequest.Queries.GetUntactMedicalRequestsForApproval
{
    public record GetUntactMedicalRequestsForApprovalQuery : IQuery<Result<GetUntactMedicalRequestsForApprovalResponse>>
    {
        /// <summary>
        /// 페이지 번호
        /// </summary>
        public int PageNo { get; init; }

        /// <summary>
        /// 페이지 크기
        /// </summary>
        public int PageSize { get; init; }

        /// <summary>
        /// 요양기관 키
        /// </summary>
        public string HospKey { get; init; } = default!;

        /// <summary>
        /// 승인 여부
        /// </summary>
        public string ApprYn { get; init; } = default!;
    }
}
