using ClosedXML.Excel;
using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Exports;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ServiceUsage;
using Hello100Admin.Modules.Admin.Application.Common.Exports;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Results;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries
{
    public record ExportUntactMedicalUsageStatusExcelQuery : IQuery<Result<ExcelFile>>
    {
        /// <summary>
        /// 조회 시작일 (yyyy-MM-dd)
        /// </summary>
        public string FromDate { get; init; } = default!;
        /// <summary>
        /// 조회 종료일 (yyyy-MM-dd)
        /// </summary>
        public string ToDate { get; init; } = default!;
        /// <summary>
        /// 조회 날짜 유형 [0: 당일, 1: 기간설정]
        /// </summary>
        public int SearchDateType { get; set; }
        /// <summary>
        /// 검색 유형 [1: 병원명, 2: 요양기관번호, 3: 회원명, 4: 주문번호]
        /// </summary>
        public int SearchType { get; init; }
        /// <summary>
        /// 검색 키워드
        /// </summary>
        public string? SearchKeyword { get; init; }
        /// <summary>
        /// 검색 상태 유형 (복수 선택 가능) [전체: "total", 접수완료: "recept", 진료완료: "end", 접수취소: "cancel"]
        /// </summary>
        public List<string> SearchStateTypes { get; init; } = default!;
        /// <summary>
        /// 검색 결제 유형 (복수 선택 가능) [전체: "total", 결제완료: "success", 결제실패: "fail"]
        /// </summary>
        public List<string> SearchPaymentTypes { get; init; } = default!;
    }

    public class ExportUntactMedicalUsageStatusExcelQueryValidator : AbstractValidator<ExportUntactMedicalUsageStatusExcelQuery>
    {
        public ExportUntactMedicalUsageStatusExcelQueryValidator()
        {
            RuleFor(x => x.SearchType).NotNull().GreaterThan(0).WithMessage("검색 유형은 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.SearchStateTypes)
                .NotEmpty().WithMessage("검색 상태 유형은 최소 하나 이상 선택해야 합니다.")
                .Must(types => types.All(t => new[] { "total", "recept", "end", "cancel" }.Contains(t)))
                .WithMessage("검색 상태 유형은 'total', 'recept', 'end', 'cancel' 중 하나여야 합니다.");
            RuleFor(x => x.SearchPaymentTypes)
                .NotEmpty().WithMessage("검색 결제 유형은 최소 하나 이상 선택해야 합니다.")
                .Must(types => types.All(t => new[] { "total", "success", "fail" }.Contains(t)))
                .WithMessage("검색 결제 유형은 'total', 'success', 'fail' 중 하나여야 합니다.");
            RuleFor(x => x.FromDate)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("조회 시작일은 필수입니다.");
            RuleFor(x => x.ToDate)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("조회 종료일은 필수입니다.");
        }
    }

    public class ExportUntactMedicalUsageStatusExcelQueryHandler : IRequestHandler<ExportUntactMedicalUsageStatusExcelQuery, Result<ExcelFile>>
    {
        private readonly IServiceUsageStore _serviceUsageStore;
        private readonly ILogger<ExportUntactMedicalUsageStatusExcelQueryHandler> _logger;
        private readonly IExcelExporter _excelExporter;
        private readonly IDbSessionRunner _db;

        public ExportUntactMedicalUsageStatusExcelQueryHandler(
            IServiceUsageStore serviceUsageStore,
            ILogger<ExportUntactMedicalUsageStatusExcelQueryHandler> logger,
            IExcelExporter excelExporter,
            IDbSessionRunner db)
        {
            _serviceUsageStore = serviceUsageStore;
            _logger = logger;
            _excelExporter = excelExporter;
            _db = db;
        }

        public async Task<Result<ExcelFile>> Handle(ExportUntactMedicalUsageStatusExcelQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handling ExportUntactMedicalUsageStatusExcelQuery");

            var historyData = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _serviceUsageStore.ExportUntactMedicalUsageStatusExcelAsync(
                    session, req.FromDate, req.ToDate, req.SearchDateType, req.SearchType, req.SearchKeyword, req.SearchStateTypes,
                    req.SearchPaymentTypes, token),
            ct);

            if (historyData.Count > 0)
            {
                var dtos = historyData.Adapt<List<ExportUntactMedicalUsageStatusExcelResult>>();

                var columns = new List<ExcelColumn<ExportUntactMedicalUsageStatusExcelResult>>
                {
                    new("NO", x => x.RowNum, Width: 8),
                    new("진료일자", x => x.ReqDate, Width: 13),
                    new("진료시간", x => x.ReqTime, Width: 13),
                    new("접수번호", x => x.ReceptNo, Width: 13),
                    new("병원명", x => x.HospName, Width: 17),
                    new("의사명", x => x.DoctorName, Width: 15),
                    new("환자명", x => x.PtntName, Width: 15),
                    new("진료상태", x => x.PtntState, Width: 14),
                    new("결제금액", x => x.PaymentAmt, Width: 15, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                    new("결제상태", x => x.PaymentStatus, Width: 15),
                    new("주문번호", x => x.OrdrIdxx, Width: 23),
                    new("팩스발송회수", x => x.FaxCount, Width: 16, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right)
                };

                var content = _excelExporter.Export(dtos, $"비대면진료현황_{DateTime.Now:yyyyMMdd}", "비대면진료현황", columns);
                return Result.Success(new ExcelFile(content, $"비대면진료현황_{DateTime.Now:yyyyMMdd}.xlsx", GlobalConstant.ContentTypes.Xlsx));
            }

            return Result.Success(new ExcelFile()).WithError(GlobalErrorCode.NoDataForExcelExport.ToError());
        }
    }
}