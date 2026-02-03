using DocumentFormat.OpenXml.Drawing;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security.Hash;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.External;
using Hello100Admin.Modules.Admin.Application.Common.Constracts.External.Web.KakaoBiz.Request;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.SendAuthNumber;
using Hello100Admin.Modules.Auth.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.SendAuthNumber
{
    public class SendAuthNumberToSmsCommandHandler : IRequestHandler<SendAuthNumberToSmsCommand, Result<SendAuthNumberResponse>>
    {
        private readonly ILogger<SendAuthNumberToSmsCommandHandler> _logger;
        private readonly IAuthRepository _authRepository;
        private readonly IHasher _sha256Hasher;
        private readonly IBizApiClientService _bizApiClientService;
        private readonly ICryptoService _cryptoService;

        public SendAuthNumberToSmsCommandHandler(ILogger<SendAuthNumberToSmsCommandHandler> logger, IAuthRepository authRepository, IHasher sha256Hasher, IBizApiClientService bizApiClientService, ICryptoService cryptoService)
        {
            _logger = logger;
            _authRepository = authRepository;
            _sha256Hasher = sha256Hasher;
            _bizApiClientService = bizApiClientService;
            _cryptoService = cryptoService;
        }

        private int MakeRandom(int length)
        {
            Random random = new Random();

            int tmp = 0;

            for (int i = 0; i < length; i++)
            {
                tmp += (int)(random.Next(10) * Math.Pow(10, i));
            }

            return tmp;
        }

        private string MakeAuthCode()
        {
            return string.Format("{0:000000}", MakeRandom(6));
        }

        public async Task<Result<SendAuthNumberResponse>> Handle(SendAuthNumberToSmsCommand request, CancellationToken cancellationToken)
        {
            var authNumber = MakeAuthCode();

            var AppAuthNumberInfo = new AppAuthNumberInfoEntity()
            {
                AppCd = request.AppCd,
                Key = _sha256Hasher.HashWithSalt(request.Tel, request.AppCd),
                AuthNumber = _sha256Hasher.HashWithSalt(authNumber, request.AppCd)
            };

            int? authId = null;

            try
            {
                authId = await _authRepository.InsertAsync(AppAuthNumberInfo, cancellationToken);
            }
            catch
            {
                return Result.Success<SendAuthNumberResponse>().WithError(GlobalErrorCode.InvalidVerificationCode.ToError());
            }

            if (authId == null || authId == 0)
            {
                return Result.Success<SendAuthNumberResponse>().WithError(GlobalErrorCode.FailedRequestVerificationCode.ToError());
            }

            var kakaoBizHello100OtpRequest = new KakaoBizLoginOtpRequest
            {
                PhoneNo = request.Tel,
                Otp = authNumber,
                EncKey = _cryptoService.EncryptWithNoVector("clinic2013!" + DateTime.Now.ToString("yyyyMMdd")),
            };

            try
            {
                await _bizApiClientService.LoginOtpAsync(kakaoBizHello100OtpRequest, cancellationToken);
            }
            catch
            { }

            var response = new SendAuthNumberResponse()
            {
                AuthId = authId.Value
            };

            return Result.Success(response);
        }
    }
}
