using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security.Hash;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.AdminUser;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.AdminUser.Commands
{
    public record UpdateHospitalAdminPasswordCommand(string AId, string NewPassword) : IQuery<Result>;

    public class UpdateHospitalAdminPasswordCommandHandler : IRequestHandler<UpdateHospitalAdminPasswordCommand, Result>
    {
        private readonly ILogger<UpdateHospitalAdminPasswordCommandHandler> _logger;
        private readonly IAdminUserRepository _adminUserRepository;
        private readonly IHasher _hash;

        public UpdateHospitalAdminPasswordCommandHandler(
            ILogger<UpdateHospitalAdminPasswordCommandHandler> logger,
            IAdminUserRepository adminUserRepository,
            IHasher hash)
        {
            _logger = logger;
            _adminUserRepository = adminUserRepository;
            _hash = hash;
        }

        public async Task<Result> Handle(UpdateHospitalAdminPasswordCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Handling UpdateHospitalAdminPasswordCommandHandler");

            var encryptedPwd = _hash.HashWithSalt(req.NewPassword, req.AId);

            var updateCount = await _adminUserRepository.UpdatePassword(req.AId, encryptedPwd);

            if (updateCount <= 0)
                return Result.Success().WithError(AdminErrorCode.PasswordChangeFailed.ToError());

            return Result.Success();
        }
    }
}
