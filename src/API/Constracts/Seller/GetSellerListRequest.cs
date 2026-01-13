namespace Hello100Admin.API.Constracts.Seller
{
    public record GetSellerListRequest
    {
        /// <summary>
        /// 페이지 번호
        /// </summary>
        public required int PageNo { get; set; } = 1;

        /// <summary>
        /// 페이지 사이즈
        /// </summary>
        public required int PageSize { get; set; } = 20;

        /// <summary>
        /// 검색어
        /// </summary>
        public string? SearchText { get; set; }

        /// <summary>
        /// 연동여부
        /// </summary>
        public string? IsSync { get; set; } = "";

        /// <summary>
        /// 활성여부
        /// </summary>
        public bool Enabled { get; set; } = true;
    }
}
