using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Errors;

namespace Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions
{
    public static class GlobalErrorCodeExtensions
    {
        public static ErrorInfo ToError(this GlobalErrorCode code)
            => new ErrorInfo((int)code, code.ToString(), GlobalErrorCodeService.GetDescription(code));
    }
}
