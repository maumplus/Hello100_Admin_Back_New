using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Exports;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.ExportExaminationResultAlimtalkHistoriesExcel
{
    public record ExportExaminationResultAlimtalkHistoriesExcelQuery : IQuery<Result<ExcelFile>>
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
        /// 날짜 기준 타입 [0: 당일, 1: 기간 설정]
        /// </summary>
        public string DateRangeType { get; init; } = default!;

        /// <summary>
        /// 날짜 기준 타입 [1: 결과 발송일, 2: 진단 검사일]
        /// </summary>
        public int SearchDateType { get; init; }

        /// <summary>
        /// 검색 키워드
        /// </summary>
        public string? SearchKeyword { get; init; }

        /// <summary>
        /// 발송 상태 [0: 전체, 1: 발송성공, 2: 발송실패]
        /// </summary>
        public int SendStatus { get; init; }

        /// <summary>
        /// 요양기관 번호
        /// </summary>
        public string HospNo { get; init; } = default!;
    }
}
