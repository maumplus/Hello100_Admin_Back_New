

using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Exports;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Application.Common.Exports;
using Hello100Admin.Modules.Admin.Application.Features.RequestsManagement.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.RequestsManagement.Queries
{
    public record ExportRequestUntactsExcelQuery : IQuery<Result<ExcelFile>>
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
        /// <summary>
        /// 처리상태 (01: 신청, 02: 승인, 03: 반려)
        /// </summary>
        public List<string> JoinState { get; init; } = null!;
    }

    public class ExportRequestUntactsExcelQueryValidator : AbstractValidator<ExportRequestUntactsExcelQuery>
    {
        public ExportRequestUntactsExcelQueryValidator()
        {
            RuleFor(x => x.PageNo).NotNull().GreaterThan(0).WithMessage("페이지 번호는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.PageSize).NotNull().GreaterThan(0).WithMessage("페이지 사이즈는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.SearchDateType).InclusiveBetween(1, 2).WithMessage("검색 날짜 조회 타입이 범위를 벗어났습니다.");
            RuleFor(x => x.SearchType).InclusiveBetween(1, 3).WithMessage("검색 키워드 조회 타입이 범위를 벗어났습니다.");
            RuleFor(x => x.JoinState).NotNull().WithMessage("처리 상태는 필수입니다.");
        }
    }

    public class ExportRequestUntactsExcelQueryHandler : IRequestHandler<ExportRequestUntactsExcelQuery, Result<ExcelFile>>
    {
        private readonly ILogger<ExportRequestUntactsExcelQueryHandler> _logger;
        private readonly IRequestsManagementStore _requestsManagementStore;
        private readonly ICryptoService _cryptoService;
        private readonly IExcelExporter _excelExporter;

        public ExportRequestUntactsExcelQueryHandler(
            ILogger<ExportRequestUntactsExcelQueryHandler> logger, 
            IRequestsManagementStore requestsManagementStore, 
            ICryptoService cryptoService,
            IExcelExporter excelExporter)
        {
            _logger = logger;
            _requestsManagementStore = requestsManagementStore;
            _cryptoService = cryptoService;
            _excelExporter = excelExporter;
        }

        public async Task<Result<ExcelFile>> Handle(ExportRequestUntactsExcelQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handling ExportRequestUntactsExcelQuery started.");

            var requestUntactsList = await _requestsManagementStore.GetRequestUntactsAsync(
                req.PageSize, req.PageNo, req.SearchType, req.SearchDateType, req.FromDate, req.ToDate, req.SearchKeyword, req.JoinState, true, ct);

            var exportExcel = requestUntactsList.Items.ToList();

            if (exportExcel.Count > 0)
            {
                var columns = new List<ExcelColumn<GetRequestUntactsResult>>
                {
                    new("No", x => x.RowNum, Width: 10),
                    new("신청ID", x => x.Seq, Width: 10),
                    new("요양기관번호", x => x.HospNo, Width: 18),
                    new("암호화된요양기관번호", x => x.HospKey, Width: 25),
                    new("병원명", x => x.HospNm, Width: 25),
                    new("의사명", x => x.DoctNm, Width: 15),
                    new("핸드폰", x => x.DoctTel, Width: 18),
                    new("요청일시", x => x.RegDt, Width: 18),
                    new("처리일시", x => x.ModDt, Width: 18),
                    new("처리여부", x => x.JoinStateNm, Width: 12),
                    new("처리여부코드", x => x.JoinState, Width: 12)
                };

                var content = _excelExporter.Export(exportExcel, "비대면진료 신청목록", "비대면진료 신청목록", columns);
                return Result.Success(new ExcelFile(content, $"비대면진료_신청목록_{DateTime.Now.ToString("yyyyMMdd")}.xlsx", GlobalConstant.ContentTypes.Xlsx));
            }

            return Result.Success(new ExcelFile()).WithError(GlobalErrorCode.NoDataForExcelExport.ToError());
        }
    }

}
