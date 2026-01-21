namespace Hello100Admin.API.Constracts.Admin.ServiceUsage
{
    public record SearchUntactMedicalHistorysRequest()
    {
        /// <summary>
        /// 페이지 번호
        /// </summary>
        public required int PageNo { get; init; }

        /// <summary>
        /// 페이지 사이즈
        /// </summary>
        public required int PageSize { get; init; }

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
