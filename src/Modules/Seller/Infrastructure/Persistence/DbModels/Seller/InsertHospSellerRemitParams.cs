namespace Hello100Admin.Modules.Seller.Infrastructure.Persistence.DbModels.Seller
{
    /// <summary>
    /// 병원 판매자 송금 내역 생성 레코드
    /// </summary>
    internal sealed record InsertHospSellerRemitParams
    {
        /// <summary>
        /// 판매자 일련번호
        /// </summary>
        public int HospSellerId { get; init; }

        /// <summary>
        /// 총 송금 금액
        /// </summary>
        public int Amount { get; init; }

        /// <summary>
        /// 송금 요청 금액
        /// </summary>
        public int VaMny { get; init; }

        /// <summary>
        /// 입금자명
        /// </summary>
        public string VaName { get; init; } = default!;

        /// <summary>
        /// 모계좌 예금주명
        /// </summary>
        public string? Depositor { get; init; }

        /// <summary>
        /// 비고
        /// </summary>
        public string? Etc { get; init; }

        /// <summary>
        /// 관리자 아이디
        /// </summary>
        public string RegAId { get; init; } = default!;

        /// <summary>
        /// 요청 등록일
        /// </summary>
        public int RegDt { get; init; }
    }
}
