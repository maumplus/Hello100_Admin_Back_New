using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.GetRemitBalance;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Queries.GetRemitBalance
{
    public record GetRemitBalanceQuery() : IQuery<Result<GetRemitBalanceResponse>>;
}
