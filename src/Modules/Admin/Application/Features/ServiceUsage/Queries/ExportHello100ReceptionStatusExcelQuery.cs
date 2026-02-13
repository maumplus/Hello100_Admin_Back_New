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
    public record ExportHello100ReceptionStatusExcelQuery(string FromDate, string ToDate) : IQuery<Result<ExcelFile>>;

    public class ExportHello100ReceptionStatusExcelQueryValidator : AbstractValidator<ExportHello100ReceptionStatusExcelQuery>
    {
        public ExportHello100ReceptionStatusExcelQueryValidator() 
        {
            RuleFor(x => x.FromDate)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("조회 시작일이 비어있습니다. 확인 후 다시 시도해주세요.");
            RuleFor(x => x.ToDate)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("조회 종료일이 비어있습니다. 확인 후 다시 시도해주세요.");
        }
    }

    public class ExportHello100ReceptionStatusExcelQueryHandler : IRequestHandler<ExportHello100ReceptionStatusExcelQuery, Result<ExcelFile>>
    {
        private readonly ILogger<ExportHello100ReceptionStatusExcelQueryHandler> _logger;
        private readonly IServiceUsageStore _serviceUsageStore;
        private readonly IExcelExporter _excelExporter;
        private readonly IDbSessionRunner _db;

        public ExportHello100ReceptionStatusExcelQueryHandler(
            ILogger<ExportHello100ReceptionStatusExcelQueryHandler> logger,
            IServiceUsageStore serviceUsageStore,
            IExcelExporter excelExporter,
            IDbSessionRunner db)
        {
            _logger = logger;
            _serviceUsageStore = serviceUsageStore;
            _excelExporter = excelExporter;
            _db = db;
        }

        public async Task<Result<ExcelFile>> Handle(ExportHello100ReceptionStatusExcelQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handle ExportHello100ReceptionStatusExcelQueryHandler");

            var historyData = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _serviceUsageStore.ExportHello100ReceptionStatusExcelAsync(
                    session, req.FromDate, req.ToDate, token),
            ct);

            var yesterdayDtos = historyData.YesterdayItems.Adapt<List<ExportHello100ReceptionStatusExcelResultItem>>();
            var periodDtos = historyData.PeriodItems.Adapt<List<ExportHello100ReceptionStatusExcelResultItem>>();

            var columns = new List<ExcelColumn<ExportHello100ReceptionStatusExcelResultItem>>
            {
                new("요양기관번호", x => x.HospNo, Width: 15),
                new("병원명", x => x.HospName, Width: 20),
                new("총계", x => x.TotalCount, Width: 18, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("예약대기", x => x.ReservationWaitingCount, Width: 15, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("대기", x => x.WaitingCount, Width: 15, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("접수", x => x.ReceptionCount, Width: 15, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("취소", x => x.CanceledCount, Width: 15, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("완료", x => x.CompletedCount, Width: 15, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("실패", x => x.FailedCount, Width: 15, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("QR접수_예약대기", x => x.QrReservationWaitingCount, Width: 18, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("QR접수_대기", x => x.QrWaitingCount, Width: 15, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("QR접수_접수", x => x.QrReceptionCount, Width: 15, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("QR접수_취소", x => x.QrCanceledCount, Width: 15, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("QR접수_완료", x => x.QrCompletedCount, Width: 15, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("오늘접수_예약대기", x => x.TodayReservationWaitingCount, Width: 18, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("오늘접수_대기", x => x.TodayWaitingCount, Width: 15, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("오늘접수_접수", x => x.TodayReceptionCount, Width: 15, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("오늘접수_취소", x => x.TodayCanceledCount, Width: 15, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("오늘접수_완료", x => x.TodayCompletedCount, Width: 15, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("진료예약_예약대기", x => x.ReservationReservationWaitingCount, Width: 18, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("진료예약_대기", x => x.ReservationWaitingCountByType, Width: 15, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("진료예약_접수", x => x.ReservationReceptionCount, Width: 15, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("진료예약_취소", x => x.ReservationCanceledCount, Width: 15, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("진료예약_완료", x => x.ReservationCompletedCount, Width: 15, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("비대면접수_예약대기", x => x.NonFaceToFaceReservationWaitingCount, Width: 20, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("비대면접수_대기", x => x.NonFaceToFaceWaitingCount, Width: 17, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("비대면접수_접수", x => x.NonFaceToFaceReceptionCount, Width: 17, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("비대면접수_취소", x => x.NonFaceToFaceCanceledCount, Width: 17, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                new("비대면접수_완료", x => x.NonFaceToFaceCompletedCount, Width: 17, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
            };

            var period = $"{req.FromDate} ~ {req.ToDate}";

            using var wb = new XLWorkbook();
            _excelExporter.AddSheet(wb, yesterdayDtos, $"헬로100접수현황(전일)", $"조회기간: {period}", columns);
            _excelExporter.AddSheet(wb, periodDtos, $"헬로100접수현황(금일, 9시 기점)", $"조회기간: {period}", columns);
            
            using var ms = new MemoryStream();
            wb.SaveAs(ms);

            var content = ms.ToArray();

            return Result.Success(new ExcelFile(content, $"헬로100접수현황_{DateTime.Now:yyyyMMdd}.xlsx", GlobalConstant.ContentTypes.Xlsx));
        }
    }
}
