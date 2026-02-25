namespace Hello100Admin.API.Constracts.Admin.ServiceUsage
{
    public record ExportExaminationResultAlimtalkHistoriesExcelRequest
    {
        /// <summary>
        /// 요양기관번호
        /// </summary>
        public required string HospNo { get; init; }
        /// <summary>
        /// 조회 시작일
        /// </summary>
        public required string FromDate { get; init; }

        /// <summary>
        /// 조회 종료일
        /// </summary>
        public required string ToDate { get; init; }

        /// <summary>
        /// 날짜 기준 타입 [0: 당일, 1: 기간 설정]
        /// </summary>
        public required string DateRangeType { get; init; }

        /// <summary>
        /// 날짜 기준 타입 [1: 결과 발송일, 2: 진단 검사일]
        /// </summary>
        public required int SearchDateType { get; init; }

        /// <summary>
        /// 검색 키워드
        /// </summary>
        public string? SearchKeyword { get; init; }

        /// <summary>
        /// 발송 상태 [0: 전체, 1: 발송성공, 2: 발송실패]
        /// </summary>
        public required int SendStatus { get; init; }
    }

    public record ExportMyExaminationResultAlimtalkHistoriesExcelRequest
    {
        /// <summary>
        /// 조회 시작일
        /// </summary>
        public required string FromDate { get; init; }

        /// <summary>
        /// 조회 종료일
        /// </summary>
        public required string ToDate { get; init; }

        /// <summary>
        /// 날짜 기준 타입 [0: 당일, 1: 기간 설정]
        /// </summary>
        public required string DateRangeType { get; init; }

        /// <summary>
        /// 날짜 기준 타입 [1: 결과 발송일, 2: 진단 검사일]
        /// </summary>
        public required int SearchDateType { get; init; }

        /// <summary>
        /// 검색 키워드
        /// </summary>
        public string? SearchKeyword { get; init; }

        /// <summary>
        /// 발송 상태 [0: 전체, 1: 발송성공, 2: 발송실패]
        /// </summary>
        public required int SendStatus { get; init; }
    }
}
