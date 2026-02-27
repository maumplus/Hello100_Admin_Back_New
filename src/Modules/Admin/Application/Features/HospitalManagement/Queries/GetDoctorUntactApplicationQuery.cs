using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Account;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Common;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Queries
{
    public record GetDoctorUntactApplicationQuery(string EmplNo, string HospNo) : IQuery<Result<GetDoctorUntactApplicationResult>>;

    public class GetDoctorUntactApplicationQueryValidator : AbstractValidator<GetDoctorUntactApplicationQuery>
    {
        public GetDoctorUntactApplicationQueryValidator()
        {
            RuleFor(x => x.EmplNo).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("의사 사번은 필수입니다.");
            RuleFor(x => x.HospNo).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("요양기관번호는 필수입니다.");
        }
    }

    public class GetDoctorUntactApplicationQueryHandler : IRequestHandler<GetDoctorUntactApplicationQuery, Result<GetDoctorUntactApplicationResult>>
    {
        private readonly ILogger<GetDoctorUntactApplicationQueryHandler> _logger;
        private readonly IHospitalManagementStore _hospitalManagementStore;
        private readonly IAccountStore _accountStore;
        private readonly IHospitalInfoProvider _hospitalInfoProvider;
        private readonly ICryptoService _cryptoService;
        private readonly IDbSessionRunner _db;

        public GetDoctorUntactApplicationQueryHandler(
            ILogger<GetDoctorUntactApplicationQueryHandler> logger, 
            IHospitalManagementStore hospitalManagementStore,
            IAccountStore accountStore,
            IHospitalInfoProvider hospitalInfoProvider,
            ICryptoService cryptoService,
            IDbSessionRunner db)
        {
            _logger = logger;
            _hospitalManagementStore = hospitalManagementStore;
            _accountStore = accountStore;
            _hospitalInfoProvider = hospitalInfoProvider;
            _cryptoService = cryptoService;
            _db = db;
        }

        public async Task<Result<GetDoctorUntactApplicationResult>> Handle(GetDoctorUntactApplicationQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handle GetDoctorUntactApplicationQueryHandler. EmplNo={EmplNo}, HospNo={HospNo}", req.EmplNo, req.HospNo);

            var hospInfo = await _hospitalInfoProvider.GetHospitalInfoByHospNoAsync(req.HospNo, ct);

            if (hospInfo == null)
            {
                return Result.Success<GetDoctorUntactApplicationResult>().WithError(AdminErrorCode.NotFoundCurrentHospital.ToError());
            }

            if (string.IsNullOrEmpty(hospInfo.PostCd))
            {
                return Result.Success<GetDoctorUntactApplicationResult>().WithError(AdminErrorCode.NotFoundHospitalPostCd.ToError());
            }

            if (string.IsNullOrEmpty(hospInfo.Addr))
            {
                return Result.Success<GetDoctorUntactApplicationResult>().WithError(AdminErrorCode.NotFoundHospitalAddr.ToError());
            }

            if (string.IsNullOrEmpty(hospInfo.Tel))
            {
                return Result.Success<GetDoctorUntactApplicationResult>().WithError(AdminErrorCode.NotFoundHospitalTel.ToError());
            }

            var licenseTypes = await _accountStore.GetCommonList("23", ct);

            var doctorInfo = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _hospitalManagementStore.GetDoctorInfoAsync(session, req.HospNo, req.EmplNo, token),
            ct);

            var response = hospInfo.Adapt<GetDoctorUntactApplicationResult>();

            response.HospName = hospInfo.Name;
            response.LicenseTypes = licenseTypes.Adapt<List<GetDoctorUntactApplicationResultLicenseTypeItem>>();
            response.DoctorInfo = doctorInfo.Adapt<GetDoctorUntactApplicationResultDoctorInfoItem>();
            response.DoctorInfo.DoctNo = _cryptoService.DecryptWithNoVector(response.DoctorInfo.DoctNo, CryptoKeyType.Default);

            return Result.Success(response);
        }
    }
}
