using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Exports;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Application.Common.Exports;
using Hello100Admin.Modules.Admin.Application.Features.Hospitals.Results;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.Hospitals.Queries
{
    public record ExportHospitalsExcelQuery(int SearchType, string? SearchKeyword) : IQuery<Result<ExcelFile>>;

    public class ExportHospitalsExcelQueryHandler : IRequestHandler<ExportHospitalsExcelQuery, Result<ExcelFile>>
    {
        private readonly ILogger<ExportHospitalsExcelQueryHandler> _logger;
        private readonly IExcelExporter _excelExporter;
        private readonly IHospitalsStore _hospitalsStore;
        private readonly IDbSessionRunner _db;

        public ExportHospitalsExcelQueryHandler(
            ILogger<ExportHospitalsExcelQueryHandler> logger, 
            IExcelExporter excelExporter, 
            IHospitalsStore hospitalsStore, 
            IDbSessionRunner db)
        {
            _logger = logger;
            _excelExporter = excelExporter;
            _hospitalsStore = hospitalsStore;
            _db = db;
        }

        public async Task<Result<ExcelFile>> Handle(ExportHospitalsExcelQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handle ExportHospitalsExcelQueryHandler");

            var historyData = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _hospitalsStore.ExportHospitalsExcelAsync(session, req.SearchType, req.SearchKeyword, token),
            ct);

            if (historyData.Count > 0)
            {
                var dtos = historyData.Adapt<List<ExportHospitalsExcelResult>>();

                var columns = new List<ExcelColumn<ExportHospitalsExcelResult>>
                {
                    new("HospKey", x => x.HospKey, Width: 30),
                    new("요양기관명", x => x.Name, Width: 18),
                    new("종별코드", x => x.HospClsCd, Width: 12),
                    new("종별코드명", x => x.CmName, Width: 12),
                    new("우편번호", x => x.PostCd, Width: 12),
                    new("주소", x => x.Addr, Width: 25),
                    new("전화번호", x => x.Tel, Width: 15),
                    new("병원URL", x => x.Site, Width: 18),
                    new("개설일자", x => x.RegDt, Width: 12),
                    new("X좌표", x => x.Lat, Width: 18, Format: "0.###############################"),
                    new("Y좌표", x => x.Lng, Width: 18, Format: "0.###############################")
                };

                var content = _excelExporter.Export(dtos, "병원목록", "병원 목록", columns);
                return Result.Success(new ExcelFile(content, $"병원목록_{DateTime.Now.ToString("yyyyMMdd")}.xlsx", GlobalConstant.ContentTypes.Xlsx));
            }

            return Result.Success(new ExcelFile()).WithError(GlobalErrorCode.NoDataForExcelExport.ToError());
        }
    }
}
