using Hello100Admin.Modules.Admin.Application.Features.ApprovalRequest.ReadModels;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ApprovalRequest
{
    public interface IApprovalRequestStore
    {
        /// <summary>
        /// 비대면 진료 승인 요청 목록 조회
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="hospKey"></param>
        /// <param name="apprYn"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<GetUntactMedicalRequestsForApprovalReadModel> GetUntactMedicalRequestsForApprovalAsync(int pageNo, int pageSize, string hospKey, string apprYn, CancellationToken token);

        /// <summary>
        /// 비대면 진료 승인 요청 상세 조회
        /// </summary>
        /// <param name="seq"></param>
        /// <param name="imageUrl"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<GetUntactMedicalRequestDetailForApprovalReadModel> GetUntactMedicalRequestDetailForApprovalAsync(int seq, string imageUrl, CancellationToken token);
    }
}
