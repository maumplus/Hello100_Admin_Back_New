using Hello100Admin.BuildingBlocks.Common.Application;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.CreateSellerRemit
{
    public record CreateSellerRemitCommand(string AId, int HospSellerId, int Amount, string? Etc) : ICommand<Result>;
}
