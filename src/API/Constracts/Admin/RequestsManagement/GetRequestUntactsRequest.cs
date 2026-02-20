namespace Hello100Admin.API.Constracts.Admin.RequestsManagement
{
    public sealed record GetRequestUntactsRequest
    {
        /// <summary>
        /// 페이지 번호
        /// </summary>
        public required int PageNo { get; set; }

        /// <summary>
        /// 페이지 사이즈
        /// </summary>
        public required int PageSize { get; set; }

        /// <summary>
        /// 조회 시작일
        /// </summary>
        public string? FromDate { get; set; }

        /// <summary>
        /// 조회 종료일
        /// </summary>
        public string? ToDate { get; set; }

        /// <summary>
        /// 검색 키워드 조회 타입 [병원명: 1, 의사명: 2, 요양기관번호: 3]
        /// </summary>
        public required int SearchType { get; set; }

        /// <summary>
        /// 검색 기간 타입 [전체: 0, 기간: 1]
        /// </summary>
        public required int SearchDateType { get; set; }

        /// <summary>
        /// 검색 키워드
        /// </summary>
        public string? SearchKeyword { get; set; }

        /// <summary>
        /// 처리상태 [신청: 01, 승인: 02, 반려: 03]
        /// </summary>
        public List<string>? JoinState { get; set; }
    }
}
