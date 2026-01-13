using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Seller.Application.Common.Errors;

namespace Hello100Admin.Modules.Seller.Application.Common.Extensions
{
    public static class SellerErrorCodeExtensions
    {
        public static ErrorInfo ToError(this SellerErrorCode code)
            => new ErrorInfo((int)code, code.ToString(), SellerErrorDescProvider.GetDescription(code));
    }
}
