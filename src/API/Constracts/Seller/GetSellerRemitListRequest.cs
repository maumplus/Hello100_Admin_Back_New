namespace Hello100Admin.API.Constracts.Seller
{
    public class GetSellerRemitListRequest
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
        /// 시작일자
        /// </summary>
        public required string StartDt { get; set; }

        /// <summary>
        /// 종료일자
        /// </summary>
        public required string EndDt { get; set; }

        /// <summary>
        /// 송금상태
        /// </summary>
        public string? RemitStatus { get; set; }
    }
}
