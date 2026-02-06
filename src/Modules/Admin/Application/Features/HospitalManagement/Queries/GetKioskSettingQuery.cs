using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Common;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Queries
{
    /// <summary>
    /// 키오스크 설정 조회 쿼리
    /// </summary>
    /// <param name="HospNo"></param>
    /// <param name="HospKey"></param>
    /// <param name="EmplNo"></param>
    public record GetKioskSettingQuery(string HospNo, string HospKey, string? EmplNo) : IQuery<Result<GetDeviceSettingResult<KioskRo>>>;

    public class GetKioskSettingQueryValidator : AbstractValidator<GetKioskSettingQuery>
    {
        public GetKioskSettingQueryValidator()
        {
            RuleFor(x => x.HospNo).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("요양기관번호는 필수입니다.");
            RuleFor(x => x.HospKey).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("요양기관키는 필수입니다.");
        }
    }

    public class GetKioskSettingQueryHandler : IRequestHandler<GetKioskSettingQuery, Result<GetDeviceSettingResult<KioskRo>>>
    {
        private readonly ILogger<GetKioskSettingQueryHandler> _logger;
        private readonly IHospitalManagementStore _hospitalStore;
        private readonly ICurrentHospitalProfileProvider _hospitalProfileProvider;
        private readonly IDbSessionRunner _db;

        public GetKioskSettingQueryHandler(
            ILogger<GetKioskSettingQueryHandler> logger,
            ICurrentHospitalProfileProvider hospitalProfileProvider,
            IDbSessionRunner db,
            IHospitalManagementStore hospitalStore)
        {
            _logger = logger;
            _hospitalStore = hospitalStore;
            _hospitalProfileProvider = hospitalProfileProvider;
            _db = db;
        }

        public async Task<Result<GetDeviceSettingResult<KioskRo>>> Handle(GetKioskSettingQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handling GetKioskSettingQuery HospNo:{HospNo}", req.HospNo);

            var hospInfo = await _hospitalProfileProvider.GetCurrentHospitalProfileByHospNoAsync(req.HospNo, ct);

            var result = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _hospitalStore.GetKioskSettingAsync(session, req.HospNo, req.HospKey, req.EmplNo, 1, token),  // SetDeviceType.Kiosk: 1
            ct);

            result.Name = hospInfo.Name;

            var setJsonStr = result.DeviceData.SetJsonStr;
            KioskRo? setJson = string.IsNullOrWhiteSpace(setJsonStr) == false
                             ? setJsonStr.FromJsonNoOptions<KioskRo>()
                             : null;

            var hospNm = result.DeviceData.HospNm ?? hospInfo.Name;
            var infoTxt = result.DeviceData.InfoTxt ?? "";

            var mapped = result.DeviceData.Adapt<DeviceRo<KioskRo>>();

            mapped.HospNm = hospNm;
            mapped.InfoTxt = infoTxt;

            mapped.SetJson = new KioskRo
            {
                ReceptYn = setJson?.ReceptYn ?? "N",
                NewReceiveYn = setJson?.NewReceiveYn ?? "N",
                AddrYn = setJson?.AddrYn ?? "N",
                DetailYn = setJson?.DetailYn ?? "N",
                DeptYn = setJson?.DeptYn ?? "N",
                DeptBreakYn = setJson?.DeptBreakYn ?? "",
                WaitTimeYn = setJson?.WaitTimeYn ?? "N",
                ViewMinTime = setJson?.ViewMinTime ?? "",
                PrtBarcodeYn = setJson?.PrtBarcodeYn ?? "",
                PtntInputType = setJson?.PtntInputType ?? 0,
                PayYn = setJson?.PayYn ?? "Y",
                SimplePayYn = setJson?.SimplePayYn ?? "Y",
                receiptState = setJson?.receiptState ?? "W",
                //ReceiveMainSelect = setJson?.ReceiveMainSelect ?? "D",
                //PopupYn = setJson?.PopupYn ?? "Y",
                //DefaultDeptCD = setJson?.DefaultDeptCD ?? "",
                //DefaultEmplNo = setJson?.DefaultEmplNo ?? "",
                //QrReceiptYn = setJson?.QrReceiptYn ?? "N"
            };

            result.DeviceData = mapped;

            var test = result.ToJsonForStorage();

            return Result.Success(result);
        }
    }
}
