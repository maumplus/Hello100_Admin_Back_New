namespace Hello100Admin.API.Constracts.Seller
{
    public record CreateSellerRequest
    {
        /// <summary>
        /// 요양기관번호
        /// </summary>
        public required string HospNo { get; init; }

        /// <summary>
        /// 은행 코드
        /// </summary>
        public required string BankCd { get; init; }

        /// <summary>
        /// 계좌 번호 (하이픈 없이)
        /// </summary>
        public required string DepositNo { get; init; }

        /// <summary>
        /// 예금주명
        /// </summary>
        public required string Depositor { get; init; }

        /// <summary>
        /// 비고
        /// </summary>
        public string? Etc { get; init; }
    }
}
