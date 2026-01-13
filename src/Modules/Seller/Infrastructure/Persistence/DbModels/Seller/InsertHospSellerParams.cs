namespace Hello100Admin.Modules.Seller.Infrastructure.Persistence.DbModels.Seller
{
    /// <summary>
    /// 병원 판매자 계좌 정보 생성 레코드
    /// </summary>
    internal sealed record InsertHospSellerParams
    {
        /// <summary>
        /// 요양기관번호
        /// </summary>
        public string HospNo { get; init; } = default!;

        /// <summary>
        /// 판매자 ID (aid(관리자아이디) + emplno(의사번호))
        /// </summary>
        public string SellerId { get; init; } = default!;

        /// <summary>
        /// 은행 코드
        /// </summary>
        public string BankCd { get; init; } = default!;

        /// <summary>
        /// 계좌 번호 (하이픈 없이)
        /// </summary>
        public string DepositNo { get; init; } = default!;

        /// <summary>
        /// 예금주명
        /// </summary>
        public string Depositor { get; init; } = default!;

        /// <summary>
        /// 비고
        /// </summary>
        public string? Etc { get; init; }

        /// <summary>
        /// 관리자 아이디
        /// </summary>
        public string RegAId { get; init; } = default!;

        /// <summary>
        /// 등록일 (UNIX TIMESTAMP)
        /// </summary>
        public int RegDt { get; init; }
    }
}
