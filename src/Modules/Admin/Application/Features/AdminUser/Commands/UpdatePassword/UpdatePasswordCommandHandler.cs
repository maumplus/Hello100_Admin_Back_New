using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security.Hash;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.AdminUser;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.AdminUser.Commands.UpdatePassword
{
    public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, Result>
    {
        private readonly ILogger<UpdatePasswordCommandHandler> _logger;
        private readonly IAdminUserRepository _adminUserRepository;
        private readonly IHasher _hash;

        public UpdatePasswordCommandHandler(ILogger<UpdatePasswordCommandHandler> logger, IAdminUserRepository adminUserRepository, IHasher hash)
        {
            _logger = logger;
            _adminUserRepository = adminUserRepository;
            _hash = hash;
        }

        public async Task<Result> Handle(UpdatePasswordCommand command, CancellationToken token)
        {
            _logger.LogInformation("Processing update password. [{id}]", command.Aid);

            var encryptedPwd = _hash.HashWithSalt(command.NewPassword, command.Aid);

            var updateCount = await _adminUserRepository.UpdatePassword(command.Aid, encryptedPwd);

            if (updateCount <= 0)
                return Result.Success().WithError(AdminErrorCode.PasswordChangeFailed.ToError());

            return Result.Success();
        }
    }
}
