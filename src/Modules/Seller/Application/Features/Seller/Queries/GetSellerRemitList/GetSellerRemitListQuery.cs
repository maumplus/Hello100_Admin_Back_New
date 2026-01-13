using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.GetSellerRemitList;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.Shared;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Queries.GetSellerRemitList
{
    /// <summary>
    /// 판매자 송금 내역 조회 쿼리
    /// </summary>
    /// <param name="PageNo">페이지 번호</param>
    /// <param name="PageSize">페이지 사이즈</param>
    /// <param name="SearchText">검색어</param>
    /// <param name="StartDt">시작일자</param>
    /// <param name="EndDt">종료일자</param>
    /// <param name="RemitStatus">송금상태</param>
    public record GetSellerRemitListQuery(
        int PageNo,
        int PageSize,
        string? SearchText,
        string StartDt,
        string EndDt,
        string? RemitStatus
    ) : IQuery<Result<PagedResult<GetSellerRemitListResponse>>>;
}
