using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.UpdateSellerRemit;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.UpdateSellerRemit
{
    public record UpdateSellerRemitCommand(int Id, string? Etc) : ICommand<Result<UpdateSellerRemitResponse>>;
}
