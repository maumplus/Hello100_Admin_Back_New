using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Responses.GetUntactMedicalPaymentDetail;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.GetUntactMedicalPaymentDetail
{
    /// <summary>
    /// 비대면 진료 결제 내역 상세 조회 쿼리
    /// </summary>
    /// <param name="PaymentId">NHN KCP 결제 ID</param>
    public record GetUntactMedicalPaymentDetailQuery(string PaymentId) : IQuery<Result<GetUntactMedicalPaymentDetailResponse>>;
}
