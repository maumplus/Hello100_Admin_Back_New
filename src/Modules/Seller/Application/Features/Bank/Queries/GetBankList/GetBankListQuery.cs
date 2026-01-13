using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Seller.Application.Features.Bank.Responses.GetBankList;

namespace Hello100Admin.Modules.Seller.Application.Features.Bank.Queries.GetBankList
{
    public record GetBankListQuery() : IQuery<Result<GetBankListResponse>>;
}
