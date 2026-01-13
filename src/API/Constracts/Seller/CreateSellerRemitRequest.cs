namespace Hello100Admin.API.Constracts.Seller
{
    public record CreateSellerRemitRequest
    {
        /// <summary>
        /// 셀러 일련번호
        /// </summary>
        public required int HospSellerId { get; init; }

        /// <summary>
        /// 송금 금액
        /// </summary>
        public required int Amount { get; init; }

        /// <summary>
        /// 비고
        /// </summary>
        public string? Etc { get; init; }
    }
}
