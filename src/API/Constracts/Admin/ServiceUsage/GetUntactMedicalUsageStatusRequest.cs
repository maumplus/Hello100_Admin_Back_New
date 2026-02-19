namespace Hello100Admin.API.Constracts.Admin.ServiceUsage
{
    public sealed record GetUntactMedicalUsageStatusRequest
    {
        /// <summary>
        /// 페이지 번호
        /// </summary>
        public required int PageNo { get; init; }
        /// <summary>
        /// 페이지당 항목 수
        /// </summary>
        public required int PageSize { get; init; }
        /// <summary>
        /// 조회 시작일 (yyyy-MM-dd)
        /// </summary>
        public required string FromDate { get; init; }
        /// <summary>
        /// 조회 종료일 (yyyy-MM-dd)
        /// </summary>
        public required string ToDate { get; init; }
        /// <summary>
        /// 조회 날짜 유형 [0: 당일, 1: 기간설정]
        /// </summary>
        public required int SearchDateType { get; set; }
        /// <summary>
        /// 검색 유형 [1: 병원명, 2: 요양기관번호, 3: 회원명, 4: 주문번호]
        /// </summary>
        public required int SearchType { get; init; }
        /// <summary>
        /// 검색 키워드
        /// </summary>
        public string? SearchKeyword { get; init; }
        /// <summary>
        /// 검색 상태 유형 (복수 선택 가능) [전체: "total", 접수완료: "recept", 진료완료: "end", 접수취소: "cancel"]
        /// </summary>
        public required List<string> SearchStateTypes { get; init; } = default!;
        /// <summary>
        /// 검색 결제 유형 (복수 선택 가능) [전체: "total", 결제완료: "success", 결제실패: "fail"]
        /// </summary>
        public required List<string> SearchPaymentTypes { get; init; } = default!;
    }
}
