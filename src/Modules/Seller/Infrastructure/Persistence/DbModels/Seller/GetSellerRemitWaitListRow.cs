namespace Hello100Admin.Modules.Seller.Infrastructure.Persistence.DbModels.Seller
{
    internal sealed record GetSellerRemitWaitListRow
    {
        /// <summary>
        /// 행 번호
        /// </summary>
        public int RowNum { get; init; }

        /// <summary>
        /// 고유 ID
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// 요양기관번호
        /// </summary>
        public string HospNo { get; init; } = default!;

        /// <summary>
        /// 병원명
        /// </summary>
        public string HospName { get; init; } = default!;

        /// <summary>
        /// 은행 코드
        /// </summary>
        public string BankCd { get; init; } = default!;

        /// <summary>
        /// 은행명
        /// </summary>
        public string BankName { get; init; } = default!;

        /// <summary>
        /// 은행 로고 이미지 경로
        /// </summary>
        public string? BankImgPath { get; init; }

        /// <summary>
        /// 계좌 번호 (하이픈 없이)
        /// </summary>
        public string DepositNo { get; init; } = default!;

        /// <summary>
        /// 예금주명
        /// </summary>
        public string Depositor { get; init; } = default!;

        /// <summary>
        /// 총 송금 금액
        /// </summary>
        public int Amount { get; init; }

        /// <summary>
        /// 송금 상태 (0: 대기(등록 상태), 1:요청,  2: 성공(완료), 3: 재요청, 4: 실패, 5: 취소(삭제) )
        /// </summary>
        public string Status { get; init; } = null!;

        /// <summary>
        /// 비고
        /// </summary>
        public string? Etc { get; init; }
    }
}
