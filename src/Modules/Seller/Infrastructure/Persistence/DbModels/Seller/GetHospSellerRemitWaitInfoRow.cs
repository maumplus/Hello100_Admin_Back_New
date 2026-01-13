namespace Hello100Admin.Modules.Seller.Infrastructure.Persistence.DbModels.Seller
{
    internal sealed record GetHospSellerRemitWaitInfoRow
    {
        /// <summary>
        /// 고유 ID
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// 판매자 ID (aid(관리자아이디) + emplno(의사번호))
        /// </summary>
        public string SellerId { get; init; } = default!;

        /// <summary>
        /// 계좌 번호 (하이픈 없이)
        /// </summary>
        public string DepositNo { get; init; } = default!;

        /// <summary>
        /// 예금주명
        /// </summary>
        public string Depositor { get; init; } = default!;

        /// <summary>
        /// 활성여부(0:비활성, 1:활성)
        /// </summary>
        public string Enabled { get; init; } = default!;

        /// <summary>
        /// 연동 여부
        /// </summary>
        public string IsSync { get; init; } = default!;

        /// <summary>
        /// 총 송금 금액
        /// </summary>
        public int Amount { get; init; }

        /// <summary>
        /// 입금자명
        /// </summary>
        public string? VaName { get; set; }

        /// <summary>
        /// 송금 상태 [0: 대기(등록 상태), 1:요청,  2: 성공(완료), 3: 재요청, 4: 실패, 5: 취소(삭제)]
        /// </summary>
        public string Status { get; set; } = default!;
    }
}
