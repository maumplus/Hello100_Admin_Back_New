namespace Hello100Admin.API.Constracts.Seller
{
    public class GetSellerRemitWaitListRequest
    {
        /// <summary>
        /// 시작일자
        /// </summary>
        public required string StartDt { get; set; }

        /// <summary>
        /// 종료일자
        /// </summary>
        public required string EndDt { get; set; }
    }
}
