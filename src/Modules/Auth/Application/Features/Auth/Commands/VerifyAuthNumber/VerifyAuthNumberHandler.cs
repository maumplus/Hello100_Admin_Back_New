using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security.Hash;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.External;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.VerifyAuthNumber
{
    public class VerifyAuthNumberCommandHandler : IRequestHandler<VerifyAuthNumberCommand, Result>
    {
        private readonly ILogger<VerifyAuthNumberCommandHandler> _logger;
        private readonly IAuthStore _authStore;
        private readonly IHasher _sha256Hasher;

        public VerifyAuthNumberCommandHandler(ILogger<VerifyAuthNumberCommandHandler> logger, IAuthStore authStore, IHasher sha256Hasher)
        {
            _logger = logger;
            _authStore = authStore;
            _sha256Hasher = sha256Hasher;
        }

        public async Task<Result> Handle(VerifyAuthNumberCommand request, CancellationToken cancellationToken)
        {
            var authNumberEnc = _sha256Hasher.HashWithSalt(request.AuthNumber, request.AppCd);

            var appAuthNumberInfo = await _authStore.GetAppAuthNumberInfoAsync(request.AuthId);

            if (appAuthNumberInfo == null)
            {
                return Result.Success().WithError(GlobalErrorCode.InvalidVerificationCode.ToError());
            }
            else if (appAuthNumberInfo.AuthNumber != authNumberEnc)
            {
                return Result.Success().WithError(GlobalErrorCode.InvalidVerificationCode.ToError());
            }
            else
            {
                var regDt = appAuthNumberInfo.RegDt.ToDateTime("yyyy-MM-dd HH:mm:ss");

                if (regDt == null || DateTime.Now > regDt.Value.AddMinutes(3))
                {
                    return Result.Success().WithError(GlobalErrorCode.ExpiredVerificationCode.ToError());
                }
            }

            return Result.Success();
        }
    }
}
