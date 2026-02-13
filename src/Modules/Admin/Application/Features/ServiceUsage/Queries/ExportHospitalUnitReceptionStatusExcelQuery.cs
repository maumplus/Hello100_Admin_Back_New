using ClosedXML.Excel;
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
    public record ExportHospitalUnitReceptionStatusExcelQuery : IQuery<Result<ExcelFile>>
    {
        /// <summary>
        /// 조회 시작일
        /// </summary>
        public string FromDate { get; init; } = default!;
        /// <summary>
        /// 조회 종료일
        /// </summary>
        public string ToDate { get; init; } = default!;
        /// <summary>
        /// 검색 타입 [1: 병원명, 2: 요양기관번호]
        /// </summary>
        public int SearchType { get; init; }
        /// <summary>
        /// 검색 키워드
        /// </summary>
        public string? SearchKeyword { get; init; }
        /// <summary>
        /// QR 접수 체크 여부
        /// </summary>
        public string QrCheckInYn { get; init; } = default!;
        /// <summary>
        /// 오늘 접수 체크 여부
        /// </summary>
        public string TodayRegistrationYn { get; init; } = default!;
        /// <summary>
        /// 진료 예약 체크 여부
        /// </summary>
        public string AppointmentYn { get; init; } = default!;
        /// <summary>
        /// 비대면 진료 체크 여부
        /// </summary>
        public string TelemedicineYn { get; init; } = default!;
        /// <summary>
        /// 테스트병원 제외 여부
        /// </summary>
        public string ExcludeTestHospitalsYn { get; init; } = default!;
    }

    public class ExportHospitalUnitReceptionStatusExcelQueryHandler : IRequestHandler<ExportHospitalUnitReceptionStatusExcelQuery, Result<ExcelFile>>
    {
        private readonly ILogger<ExportHospitalUnitReceptionStatusExcelQueryHandler> _logger;
        private readonly IServiceUsageStore _serviceUsageStore;
        private readonly IExcelExporter _excelExporter;
        private readonly IDbSessionRunner _db;

        public ExportHospitalUnitReceptionStatusExcelQueryHandler(
            ILogger<ExportHospitalUnitReceptionStatusExcelQueryHandler> logger,
            IServiceUsageStore serviceUsageStore,
            IExcelExporter excelExporter,
            IDbSessionRunner db)
        {
            _logger = logger;
            _serviceUsageStore = serviceUsageStore;
            _excelExporter = excelExporter;
            _db = db;
        }

        public async Task<Result<ExcelFile>> Handle(ExportHospitalUnitReceptionStatusExcelQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handle ExportHospitalUnitReceptionStatusExcelQueryHandler");

            var historyData = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _serviceUsageStore.ExportHospitalUnitReceptionStatusExcelAsync(
                    session, req.FromDate, req.ToDate, req.SearchType, req.SearchKeyword, req.QrCheckInYn,
                    req.TodayRegistrationYn, req.AppointmentYn, req.TelemedicineYn, req.ExcludeTestHospitalsYn, token),
            ct);

            if (historyData.Count > 0)
            {
                var dtos = historyData.Adapt<List<GetHospitalServiceUsageStatusResultItemByHospitalUnit>>();

                var columns = new List<ExcelColumn<GetHospitalServiceUsageStatusResultItemByHospitalUnit>>
                {
                    new("요양기관번호", x => x.HospNo, Width: 15),
                    new("병원명", x => x.HospName, Width: 20),
                    new("접수대기(예약완료)", x => x.WaitingCount, Width: 18, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                    new("접수완료", x => x.ReceptionCount, Width: 15, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                    new("접수취소", x => x.ReceptionCanceledCount, Width: 15, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                    new("접수실패", x => x.ReceptionFailedCount, Width: 15, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                    new("진료완료", x => x.TreatmentCompletedCount, Width: 15, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right)
                };

                var content = _excelExporter.Export(dtos, $"병원별 서비스 이용현황(하단)_{DateTime.Now:yyyyMMdd}", "병원별 서비스 이용현황(하단)", columns);
                return Result.Success(new ExcelFile(content, $"병원별서비스이용현황(하단)_{DateTime.Now:yyyyMMdd}.xlsx", GlobalConstant.ContentTypes.Xlsx));
            }

            return Result.Success(new ExcelFile()).WithError(GlobalErrorCode.NoDataForExcelExport.ToError());
        }
    }
}
