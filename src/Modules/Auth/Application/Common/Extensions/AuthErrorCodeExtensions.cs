using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Auth.Application.Common.Errors;

namespace Hello100Admin.Modules.Auth.Application.Common.Extensions
{
    public static class AuthErrorCodeExtensions
    {
        public static ErrorInfo ToError(this AuthErrorCode code)
           => new ErrorInfo((int)code, code.ToString(), AuthErrorDescProvider.GetDescription(code));
    }
}
