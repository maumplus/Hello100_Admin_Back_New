using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;
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
    /// <summary>
    /// 헬로데스크 설정 조회 쿼리
    /// </summary>
    /// <param name="HospNo"></param>
    /// <param name="HospKey"></param>
    /// <param name="EmplNo"></param>
    public record GetHelloDeskSettingQuery(string HospNo, string HospKey, string? EmplNo) : IQuery<Result<GetDeviceSettingResult<TabletRo>>>;

    public class GetHelloDeskSettingQueryValidator : AbstractValidator<GetHelloDeskSettingQuery>
    {
        public GetHelloDeskSettingQueryValidator()
        {
            RuleFor(x => x.HospNo).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("요양기관번호는 필수입니다.");
            RuleFor(x => x.HospKey).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("요양기관키는 필수입니다.");
        }
    }

    public class GetHelloDeskSettingQueryHandler : IRequestHandler<GetHelloDeskSettingQuery, Result<GetDeviceSettingResult<TabletRo>>>
    {
        private readonly ILogger<GetHelloDeskSettingQueryHandler> _logger;
        private readonly IHospitalManagementStore _hospitalStore;
        private readonly ICryptoService _cryptoService;
        private readonly ICurrentHospitalProfileProvider _hospitalProfileProvider;
        private readonly IDbSessionRunner _db;

        public GetHelloDeskSettingQueryHandler(
            ILogger<GetHelloDeskSettingQueryHandler> logger,
            IHospitalManagementStore hospitalStore,
            ICryptoService cryptoService,
            ICurrentHospitalProfileProvider hospitalProfileProvider,
            IDbSessionRunner db)
        {
            _logger = logger;
            _hospitalStore = hospitalStore;
            _cryptoService = cryptoService;
            _hospitalProfileProvider = hospitalProfileProvider;
            _db = db;
        }

        public async Task<Result<GetDeviceSettingResult<TabletRo>>> Handle(GetHelloDeskSettingQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handling GetHelloDeskSettingQuery HospNo:{HospNo}", req.HospNo);

            var hospInfo = await _hospitalProfileProvider.GetCurrentHospitalProfileByHospNoAsync(req.HospNo, ct);

            if (hospInfo.TabletCnt <= 0)
                return Result.Success<GetDeviceSettingResult<TabletRo>>().WithError(AdminErrorCode.TabletNotRegistered.ToError());

            var result = await _db.RunAsync(DataSource.Hello100, 
                (session, token) => _hospitalStore.GetHelloDeskSettingAsync(session, req.HospNo, req.HospKey, req.EmplNo, 2, token),  // SetDeviceType.Tablet: 2
            ct);

            result.Name = hospInfo.Name;

            var setJsonStr = result.DeviceData.SetJsonStr;
            TabletRo? setJson = string.IsNullOrWhiteSpace(setJsonStr) == false
                              ? setJsonStr.FromJsonNoOptions<TabletRo>()
                              : null;

            var hospNm = result.DeviceData.HospNm ?? hospInfo.Name;
            var infoTxt = result.DeviceData.InfoTxt ?? "";

            var mapped = result.DeviceData.Adapt<DeviceRo<TabletRo>>();

            mapped.HospNm = hospNm;
            mapped.InfoTxt = infoTxt;

            mapped.SetJson = new TabletRo
            {
                ReceptYn = setJson?.ReceptYn ?? "N",
                NewReceiveYn = setJson?.NewReceiveYn ?? "N",
                AddrYn = setJson?.AddrYn ?? "N",
                DetailYn = setJson?.DetailYn ?? "N",
                DeptYn = setJson?.DeptYn ?? "N",
                DeptBreakYn = setJson?.DeptBreakYn ?? "N",
                WaitTimeYn = setJson?.WaitTimeYn ?? "N",
                ViewMinTime = setJson?.ViewMinTime ?? "",
                PrtBarcodeYn = setJson?.PrtBarcodeYn ?? "",
                PtntInputType = setJson?.PtntInputType ?? 0,
                PayYn = setJson?.PayYn ?? "Y",
                SimplePayYn = setJson?.SimplePayYn ?? "Y",
                ReceiveMainSelect = setJson?.ReceiveMainSelect ?? "D",
                PopupYn = setJson?.PopupYn ?? "Y",
                DefaultDeptCD = setJson?.DefaultDeptCD ?? "",
                DefaultEmplNo = setJson?.DefaultEmplNo ?? "",
                receiptState = setJson?.receiptState ?? "W",
                QrReceiptYn = setJson?.QrReceiptYn ?? "N"
            };

            result.DeviceData = mapped;

            var doctorInfos = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _hospitalStore.GetDoctorsAsync(session, req.HospNo, 1, 50, token),
            ct);

            foreach (var item in doctorInfos.Items)
            {
                item.DoctNoDesc = _cryptoService.DecryptWithNoVector(item.DoctNo);
            }

            result.DocList = doctorInfos;

            // 신환접수를 N으로 했을때 초기화면이 기본이 되지 않고 휴대폰으로 되도록 변경
            if (result.DeviceData.SetJson.NewReceiveYn == "N" && result.DeviceData.SetJson.ReceiveMainSelect == "D")
                result.DeviceData.SetJson.ReceiveMainSelect = "P";

            var test = result.ToJsonForStorage();

            return Result.Success(result);
        }
    }
}
