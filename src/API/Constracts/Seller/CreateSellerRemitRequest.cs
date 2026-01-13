namespace Hello100Admin.API.Constracts.Seller
{
    public class CreateSellerRemitRequest
    {
        /// <summary>
        /// 셀러 일련번호
        /// </summary>
        public required int HospSellerId { get; set; }

        /// <summary>
        /// 송금 금액
        /// </summary>
        public required int Amount { get; set; }

        /// <summary>
        /// 비고
        /// </summary>
        public string? Etc { get; set; }
    }
}
