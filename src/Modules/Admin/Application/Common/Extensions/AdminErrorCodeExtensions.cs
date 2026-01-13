using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Errors;

namespace Hello100Admin.Modules.Admin.Application.Common.Extensions
{
    public static class AdminErrorCodeExtensions
    {
        public static ErrorInfo ToError(this AdminErrorCode code)
            => new ErrorInfo((int)code, code.ToString(), AdminErrorDescProvider.GetDescription(code));
    }
}
