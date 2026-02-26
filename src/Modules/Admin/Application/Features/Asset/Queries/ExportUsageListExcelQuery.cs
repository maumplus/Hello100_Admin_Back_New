using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Exports;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Application.Common.Exports;
using Hello100Admin.Modules.Admin.Application.Features.Asset.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.Asset.Queries
{

    public record ExportUsageListExcelQuery : IQuery<Result<ExcelFile>>
    {
        /// <summary>
        /// 페이지 번호
        /// </summary>
        public int PageNo { get; init; }

        /// <summary>
        /// 페이지 사이즈
        /// </summary>
        public int PageSize { get; init; }

        /// <summary>
        /// 조회 시작일
        /// </summary>
        public string? FromDate { get; init; }

        /// <summary>
        /// 조회 종료일
        /// </summary>
        public string? ToDate { get; init; }

        /// <summary>
        /// 미사용 경과일 시작일
        /// </summary>
        public string? FromDay { get; init; }

        /// <summary>
        /// 미사용 경과일 종료일
        /// </summary>
        public string? ToDay { get; init; }
        /// <summary>
        /// 조회 타입 (1: 병원명, 2: 의사명, 3: 요양기관번호)
        /// </summary>
        public int SearchType { get; init; }
        /// <summary>
        /// 조회일 타입 (1: 전체, 2: 기간)
        /// </summary>
        public int SearchDateType { get; init; }
        /// <summary>
        /// 검색어
        /// </summary>
        public string? SearchKeyword { get; init; }
    }

    public class ExportUsageListExcelQueryValidator : AbstractValidator<ExportUsageListExcelQuery>
    {
        public ExportUsageListExcelQueryValidator()
        {
            RuleFor(x => x.PageNo).NotNull().GreaterThan(0).WithMessage("페이지 번호는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.PageSize).NotNull().GreaterThan(0).WithMessage("페이지 사이즈는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.SearchDateType).InclusiveBetween(1, 2).WithMessage("검색 날짜 조회 타입이 범위를 벗어났습니다.");
            RuleFor(x => x.SearchType).InclusiveBetween(1, 3).WithMessage("검색 키워드 조회 타입이 범위를 벗어났습니다.");
        }
    }

    public class ExportUsageListExcelQueryHandler : IRequestHandler<ExportUsageListExcelQuery, Result<ExcelFile>>
    {
        private readonly ILogger<ExportUsageListExcelQueryHandler> _logger;
        private readonly IAssetStore _assetStore;
        private readonly ICryptoService _cryptoService;
        private readonly IExcelExporter _excelExporter;

        public ExportUsageListExcelQueryHandler(
            ILogger<ExportUsageListExcelQueryHandler> logger,
            IAssetStore assetStore,
            ICryptoService cryptoService,
            IExcelExporter excelExporter)
        {
            _logger = logger;
            _assetStore = assetStore;
            _cryptoService = cryptoService;
            _excelExporter = excelExporter;
        }

        public async Task<Result<ExcelFile>> Handle(ExportUsageListExcelQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handling ExportUsageListExcelQuery started.");

            var UsageList = await _assetStore.GetUsageListAsync(
                req.PageSize, req.PageNo, req.SearchType, req.SearchDateType, req.FromDate, req.ToDate, req.FromDay, req.ToDay, req.SearchKeyword, false, ct);

            var exportExcel = UsageList.Items.ToList();

            if (exportExcel.Count > 0)
            {
                var columns = new List<ExcelColumn<GetUsageListResult>>
                {
                    new("No", x => x.RowNum, Width: 10),
                    new("S/N", x => x.SerialKey, Width: 25),
                    new("요양기관번호", x => x.HospNo, Width: 18),
                    new("병원명", x => x.HospName, Width: 25),
                    new("마지막 사용일", x => x.AccessDt, Width: 18),
                    new("최근 사용일", x => x.BeforeAccessDt, Width: 18),
                    new("등록일", x => x.RegDt, Width: 18),
                    new("미사용 경과일", x => x.AccessDiffDay, Width: 12),
                    new("자격모듈설치여부", x => x.QrCheck, Width: 12),
                    new("LoginType", x => x.LoginType, Width: 15),
                    new("LicenseCd", x => x.LicenseCd, Width: 15),
                    new("PushToken", x => x.PushToken, Width: 15),
                    new("EmplNo", x => x.EmplNo, Width: 15),
                    new("OsType", x => x.OsType, Width: 15),
                    new("OsVer", x => x.OsVer, Width: 15),
                    new("AppVer", x => x.AppVer, Width: 15),
                    new("AgentVer", x => x.AgentVer, Width: 15),
                    new("AgentIp", x => x.AgentIp, Width: 15),
                    new("DeviceInfo", x => x.DeviceInfo, Width: 15),
                };

                var content = _excelExporter.Export(exportExcel, "헬로데스크 사용이력", "헬로데스크 사용이력", columns);
                return Result.Success(new ExcelFile(content, $"헬로데스크_사용이력_{DateTime.Now.ToString("yyyyMMdd")}.xlsx", GlobalConstant.ContentTypes.Xlsx));
            }

            return Result.Success(new ExcelFile()).WithError(GlobalErrorCode.NoDataForExcelExport.ToError());
        }
    }

}
