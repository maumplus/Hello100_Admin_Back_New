namespace Hello100Admin.API.Constracts.Seller
{
    public record GetSellerRemitListRequest
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
        /// 검색어
        /// </summary>
        public string? SearchText { get; init; }

        /// <summary>
        /// 시작일자
        /// </summary>
        public required string StartDt { get; init; }

        /// <summary>
        /// 종료일자
        /// </summary>
        public required string EndDt { get; init; }

        /// <summary>
        /// 송금상태
        /// </summary>
        public string? RemitStatus { get; init; }
    }
}
