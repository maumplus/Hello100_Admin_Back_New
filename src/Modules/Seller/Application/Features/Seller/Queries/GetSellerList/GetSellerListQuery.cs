using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.GetSellerList;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.Shared;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Queries.GetSellerList
{
    public record GetSellerListQuery
    (
        int PageNo, 
        int PageSize, 
        string? SearchText, 
        string? IsSync, 
        bool Enabled
    ) : IQuery<Result<PagedResult<GetSellerListResponse>>>;
}
