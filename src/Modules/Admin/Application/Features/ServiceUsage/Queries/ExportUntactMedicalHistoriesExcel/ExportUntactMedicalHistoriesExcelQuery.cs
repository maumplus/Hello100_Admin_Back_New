using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Exports;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.ExportUntactMedicalHistoriesExcel
{
    public record ExportUntactMedicalHistoriesExcelQuery : IQuery<Result<ExcelFile>>
    {
        /// <summary>
        /// 조회 시작일
        /// </summary>
        public string? FromDate { get; init; }

        /// <summary>
        /// 조회 종료일
        /// </summary>
        public string? ToDate { get; init; }

        /// <summary>
        /// 신청자명 검색
        /// </summary>
        public string? SearchKeyword { get; init; }

        /// <summary>
        /// 날짜 기준 타입 [진료예약일/결제요청일]
        /// </summary>
        public string SearchDateType { get; init; } = default!;

        /// <summary>
        /// 요양기관번호
        /// </summary>
        public string HospNo { get; init; } = default!;
    }
}
