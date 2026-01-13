using Hello100Admin.BuildingBlocks.Common.Application;

namespace Hello100Admin.Modules.Admin.Application.Features.AdminUser.Commands.UpdatePassword
{
    public record UpdatePasswordCommand(string UserId, string NewPassword) : ICommand<Result>;
}
