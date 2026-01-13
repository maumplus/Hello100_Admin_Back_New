using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.AdminUser;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.AdminUser.Commands.UpdatePassword
{
    public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, Result>
    {
        private readonly ILogger<UpdatePasswordCommandHandler> _logger;
        private readonly IAdminUserRepository _adminUserRepository;

        public UpdatePasswordCommandHandler(ILogger<UpdatePasswordCommandHandler> logger, IAdminUserRepository adminUserRepository)
        {
            _logger = logger;
            _adminUserRepository = adminUserRepository;
        }

        public async Task<Result> Handle(UpdatePasswordCommand command, CancellationToken token)
        {
            await Task.CompletedTask;
            return Result.Success();
        }
    }
}
