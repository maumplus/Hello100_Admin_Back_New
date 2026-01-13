namespace Hello100Admin.API.Constracts.Seller
{
    public record GetSellerRemitWaitListRequest
    {
        /// <summary>
        /// 시작일자
        /// </summary>
        public required string StartDt { get; init; }

        /// <summary>
        /// 종료일자
        /// </summary>
        public required string EndDt { get; init; }
    }
}
