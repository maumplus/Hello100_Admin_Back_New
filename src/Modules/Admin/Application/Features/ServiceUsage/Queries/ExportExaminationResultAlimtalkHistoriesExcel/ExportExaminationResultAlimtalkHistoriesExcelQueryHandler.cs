using ClosedXML.Excel;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Exports;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ServiceUsage;
using Hello100Admin.Modules.Admin.Application.Common.Exports;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.ReadModels.ExportUntactMedicalHistoriesExcel;
using MediatR;
using Microsoft.Extensions.Logging;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.ReadModels.ExportExaminationResultAlimtalkHistoriesExcel;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.External;
using Hello100Admin.Modules.Admin.Application.Common.Constracts.External.Web.KakaoBiz.Request;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.ExportExaminationResultAlimtalkHistoriesExcel
{
    public class ExportExaminationResultAlimtalkHistoriesExcelQueryHandler : IRequestHandler<ExportExaminationResultAlimtalkHistoriesExcelQuery, Result<ExcelFile>>
    {
        private readonly ILogger<ExportExaminationResultAlimtalkHistoriesExcelQueryHandler> _logger;
        private readonly IExcelExporter _excelExporter;
        private readonly IServiceUsageStore _serviceUsageStore;
        private readonly ICryptoService _cryptoService;
        private readonly IBizApiClientService _bizApiClientService;

        public ExportExaminationResultAlimtalkHistoriesExcelQueryHandler(ILogger<ExportExaminationResultAlimtalkHistoriesExcelQueryHandler> logger,
                                                                         IExcelExporter excelExporter,
                                                                         IServiceUsageStore serviceUsageStore,
                                                                         ICryptoService cryptoService,
                                                                         IBizApiClientService bizApiClientService)
        {
            _logger = logger;
            _excelExporter = excelExporter;
            _serviceUsageStore = serviceUsageStore;
            _cryptoService = cryptoService;
            _bizApiClientService = bizApiClientService;
        }

        public async Task<Result<ExcelFile>> Handle(ExportExaminationResultAlimtalkHistoriesExcelQuery req, CancellationToken token)
        {
            _logger.LogInformation("Process ExportExaminationResultAlimtalkHistoriesExcelQueryHandler() started.");
            var resultList = await _serviceUsageStore.GetExaminationResultAlimtalkHistoryForExportAsync(req, token);

            if (resultList.Count > 0)
            {
                var kakaoBizRequest = new KakaoBizSendHistoryRequest
                {
                    HospNo = req.HospNo,
                    FromDate = Convert.ToDateTime(req.FromDate).ToString("yyyyMMdd"),
                    ToDate = Convert.ToDateTime(req.ToDate).ToString("yyyyMMdd"),
                    EncKey = _cryptoService.Encrypt("clinic2013!" + DateTime.Now.ToString("yyyyMMdd"))
                };

                var bizResult = await _bizApiClientService.SendHistoryAsync(kakaoBizRequest, token);

                if (bizResult != null && bizResult.ResultCd == 0 && bizResult.ResultData.ListCount > 0)
                {
                    var joinedItems = resultList.Join(
                        bizResult.ResultData.List,
                        a => a.NotificationId?.ToUpper(),
                        b => b.HcResult?.ToUpper(),
                        (a, b) => new { ItemA = a, ItemB = b }
                        );

                    foreach (var pair in joinedItems)
                    {
                        if (pair.ItemB.ResultCd == "K000")
                        {
                            pair.ItemA.SendStatus = "발송성공";
                        }
                        else
                        {
                            pair.ItemA.SendStatus = "발송실패";
                            pair.ItemA.Message = $"{pair.ItemB.ResultCd} {pair.ItemB.ResultMsg}";
                        }
                    }
                }

                if (req.SendStatus == 1)
                {
                    resultList.RemoveAll(a => a.SendStatus != "발송성공");
                }
                else if (req.SendStatus == 2)
                {
                    resultList.RemoveAll(a => a.SendStatus != "발송실패");
                }

                if (resultList.Count > 0)
                {
                    var columns = new List<ExcelColumn<GetExaminationResultAlimtalkHistoryForExportReadModel>>
                    {
                        new("No", x => x.RowNum),
                        new("환자명", x => x.PtntName),
                        new("진단검사일자", x => x.ReqDate, Width: 12),
                        new("결과발송일자", x => x.SendDate, Width: 18),
                        new("발송상태", x => x.SendStatus),
                        new("실패메세지", x => x.Message, Width: 20),
                        new("발송방식", x => x.SendType),
                    };

                    var content = _excelExporter.Export(resultList, "진단검사결과 알림톡 발송 내역", "진단검사결과 알림톡 발송 내역", columns);
                    return Result.Success(new ExcelFile(content, $"진단검사결과 알림톡 발송 내역_{DateTime.Now.ToString("yyyyMMdd")}.xlsx", GlobalConstant.ContentTypes.Xlsx));
                }
            }

            return Result.Success(new ExcelFile()).WithError(GlobalErrorCode.NoDataForExcelExport.ToError());
        }
    }
}
