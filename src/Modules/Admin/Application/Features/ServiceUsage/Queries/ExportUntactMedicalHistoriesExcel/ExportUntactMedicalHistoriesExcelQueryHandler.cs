using ClosedXML.Excel;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Exports;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ServiceUsage;
using Hello100Admin.Modules.Admin.Application.Common.Exports;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.ReadModels.ExportUntactMedicalHistoriesExcel;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.ExportUntactMedicalHistoriesExcel
{
    public class ExportUntactMedicalHistoriesExcelQueryHandler : IRequestHandler<ExportUntactMedicalHistoriesExcelQuery, Result<ExcelFile>>
    {
        private readonly IExcelExporter _excelExporter;
        private readonly ILogger<ExportUntactMedicalHistoriesExcelQueryHandler> _logger;
        private readonly IServiceUsageStore _serviceUsageStore;

        public ExportUntactMedicalHistoriesExcelQueryHandler(IExcelExporter excelExporter, 
                                                            ILogger<ExportUntactMedicalHistoriesExcelQueryHandler> logger, 
                                                            IServiceUsageStore serviceUsageStore)
        {
            _excelExporter = excelExporter;
            _logger = logger;
            _serviceUsageStore = serviceUsageStore;
        }

        public async Task<Result<ExcelFile>> Handle(ExportUntactMedicalHistoriesExcelQuery req, CancellationToken token)
        {
            _logger.LogInformation("Process ExportUntactMedicalHistoriesExcelQueryHandler() started.");
            var historyData = await _serviceUsageStore.GetUntactMedicalHistoryForExportAsync(req, token);

            if (historyData.Count > 0)
            {
                var dtos = historyData.Adapt<List<GetUntactMedicalHistoryForExportReadModel>>();

                var columns = new List<ExcelColumn<GetUntactMedicalHistoryForExportReadModel>>
                {
                    new("신청자", x => x.Name, Width: 12),
                    new("진료예약일", x => x.ReqDate),
                    new("진료 유형", x => x.ReceiptType, Width: 12),
                    new("의사명", x => x.DoctNm),
                    new("결제 상태", x => x.ProcessStatus, Width: 5),
                    new("결제 금액", x => x.Amount, Format: "#,##0", Align: XLAlignmentHorizontalValues.Right),
                    new("진료 상태", x => x.PtntState),
                };

                //var content = _excelExporter.ExportWithStyles(dtos, "비대면진료내역", columns);
                var content = _excelExporter.Export(dtos, "비대면진료내역", "비대면 진료 결제 내역", columns);
                return Result.Success(new ExcelFile(content, $"비대면진료내역_{DateTime.Now.ToString("yyyyMMdd")}.xlsx", GlobalConstant.ContentTypes.Xlsx));
            }

            return Result.Success(new ExcelFile()).WithError(GlobalErrorCode.NoDataForExcelExport.ToError());
        }
    }
}
