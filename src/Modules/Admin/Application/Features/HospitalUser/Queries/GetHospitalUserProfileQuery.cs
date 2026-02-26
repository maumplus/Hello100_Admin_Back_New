using System.Globalization;
using System.Text.RegularExpressions;
using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Application.Features.HospitalUser.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalUser.Queries
{
    public record GetHospitalUserProfileQuery(string UserId) : IQuery<Result<GetHospitalUserProfileResult>>;

    public class GetHospitalUserProfileQueryValidator : AbstractValidator<GetHospitalUserProfileQuery>
    {
        public GetHospitalUserProfileQueryValidator()
        {
            RuleFor(x => x.UserId)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("병원 사용자 ID는 필수입니다.");
        }
    }

    public class GetHospitalUserProfileHandler : IRequestHandler<GetHospitalUserProfileQuery, Result<GetHospitalUserProfileResult>>
    {
        private readonly ILogger<GetHospitalUserProfileHandler> _logger;
        private readonly IHospitalUserStore _hospitalUserStore;
        private readonly ICryptoService _cryptoService;

        public GetHospitalUserProfileHandler(
            IHospitalUserStore hospitalUserStore,
            ILogger<GetHospitalUserProfileHandler> logger,
            ICryptoService cryptoService)
        {
            _hospitalUserStore = hospitalUserStore;
            _logger = logger;
            _cryptoService = cryptoService;
        }

        public async Task<Result<GetHospitalUserProfileResult>> Handle(GetHospitalUserProfileQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handling GetHospitalUserProfileQuery for UserId: {UserId}", req.UserId);

            var userProfile = await _hospitalUserStore.GetHospitalUserProfileAsync(req.UserId, ct);
            var familyProfileList = await _hospitalUserStore.GetHospitalUserFamilyProfileAsync(req.UserId, ct);
            var serviceUsageList = await _hospitalUserStore.GetHospitalUserAndFamilyServiceUsages(req.UserId, ct);

            #region SET USER PROFILE
            var name = _cryptoService.DecryptWithNoVector(userProfile.Name, CryptoKeyType.Email);
            var phone = _cryptoService.DecryptWithNoVector(userProfile.Phone, CryptoKeyType.Mobile);
            phone = phone.Replace("-", "");
            userProfile.Name = name.Substring(0, 1).PadRight(name.Length, '*');
            userProfile.Email = _cryptoService.DecryptWithNoVector(userProfile.Email, CryptoKeyType.Email);
            userProfile.Phone = phone.Length == 11 ? phone.Substring(0, 3) + "-" + phone.Substring(3, 2) + "**" + "-" + phone.Substring(7, 2) + "**" : "";
            userProfile.Email = userProfile.Email == "null" ? "" : userProfile.Email;
            userProfile.Phone = new Regex(@"(\d{3})(\d{4})(\*{4})").Replace(userProfile.Phone, "$1-$2-$3");
            #endregion

            #region SET FAMILY PROFILE
            if (familyProfileList.Count > 0)
            {
                foreach (var family in familyProfileList)
                {
                    var birthYear = _cryptoService.DecryptWithNoVector(family.BirthDay, CryptoKeyType.Mobile).Substring(0, 4) + "년 ";
                    var birthMonth = _cryptoService.DecryptWithNoVector(family.BirthDay, CryptoKeyType.Mobile).Substring(4, 2) + "월 ";
                    var birthDate = _cryptoService.DecryptWithNoVector(family.BirthDay, CryptoKeyType.Mobile).Substring(6, 2) + "일";

                    family.MemberNm = _cryptoService.DecryptWithNoVector(family.MemberNm, CryptoKeyType.Email);
                    family.BirthDay = birthYear + birthMonth + birthDate;
                    family.Sex = _cryptoService.DecryptWithNoVector(family.Sex, CryptoKeyType.Mobile);
                }
            }
            #endregion

            #region SET SERVICE USAGE
            if (serviceUsageList.Count > 0)
            {
                var rownum = serviceUsageList.Count;

                foreach (var serviceUsage in serviceUsageList)
                {
                    serviceUsage.RowNum = rownum--;

                    if (!string.IsNullOrWhiteSpace(serviceUsage.ReqDate) &&
                        DateTime.TryParseExact(serviceUsage.ReqDate, "yyyyMMdd", null, DateTimeStyles.None, out var reqDt))
                    {
                        serviceUsage.ReqDate = reqDt.ToString("yyyy.MM.dd");
                    }

                    serviceUsage.Name = string.IsNullOrWhiteSpace(serviceUsage.Name) ? "" : $"{serviceUsage.Name[0]}{new string('*', serviceUsage.Name.Length - 1)}";
                }
            }
            #endregion

            userProfile.Family.TotalCount = familyProfileList.Count;
            userProfile.Family.Items = familyProfileList;
            userProfile.ServiceUsages.TotalCount = serviceUsageList.Count;
            userProfile.ServiceUsages.Items = serviceUsageList;

            return Result.Success(userProfile);
        }
    }
}
