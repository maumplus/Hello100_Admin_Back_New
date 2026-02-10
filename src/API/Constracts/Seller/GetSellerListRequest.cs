namespace Hello100Admin.API.Constracts.Seller
{
    public record GetSellerListRequest
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
        /// 연동여부
        /// </summary>
        public string? IsSync { get; init; }
    }
}
