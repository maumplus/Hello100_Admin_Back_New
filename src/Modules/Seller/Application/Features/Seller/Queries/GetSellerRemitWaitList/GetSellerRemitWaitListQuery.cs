using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.GetSellerRemitWaitList;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Queries.GetSellerRemitWaitList
{
    /// <summary>
    /// 판매자 정산 대기 목록 조회 쿼리
    /// </summary>
    /// <param name="StartDt">시작 일자</param>
    /// <param name="EndDt">종료 일자</param>
    public record GetSellerRemitWaitListQuery(string StartDt, string EndDt) : IQuery<Result<GetSellerRemitWaitListResponse>>;
}
