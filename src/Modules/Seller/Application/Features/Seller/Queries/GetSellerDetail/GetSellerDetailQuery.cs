using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.GetSellerDetail;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Queries.GetSellerDetail
{
    public record GetSellerDetailQuery
    (
        int Id
    ) : IQuery<Result<GetSellerDetailResponse>>;
}
