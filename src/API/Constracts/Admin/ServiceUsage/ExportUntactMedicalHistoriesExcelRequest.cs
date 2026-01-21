namespace Hello100Admin.API.Constracts.Admin.ServiceUsage
{
    public record ExportUntactMedicalHistoriesExcelRequest
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
        public required string SearchDateType { get; init; }
    }
}
