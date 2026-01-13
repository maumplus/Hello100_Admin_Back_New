namespace Hello100Admin.Modules.Seller.Infrastructure.Persistence.DbModels.Seller
{
    internal sealed record GetSellerRemitInfoByIdRow
    {
        /// <summary>
        /// 결과 코드 (0000: 성공)
        /// </summary>
        public string ResCd { get; init; } = default!;

        /// <summary>
        /// 결과 메시지 (한글)
        /// </summary>
        public string ResMsg { get; init; } = default!;

        /// <summary>
        /// 결과 메시지 (영문)
        /// </summary>
        public string ResEnMsg { get; init; } = default!;

        /// <summary>
        /// 거래 일련번호
        /// </summary>
        public string TradeSeq { get; init; } = default!;

        /// <summary>
        /// 거래 일자 (yyyyMMdd)
        /// </summary>
        public string TradeDate { get; init; } = default!;

        /// <summary>
        /// 입금 은행 코드
        /// </summary>
        public string BankCode { get; init; } = default!;

        /// <summary>
        /// 입금 은행명
        /// </summary>
        public string BankName { get; init; } = default!;

        /// <summary>
        /// 입금 계좌번호 (마스킹)
        /// </summary>
        public string Account { get; init; } = default!;

        /// <summary>
        /// 예금주명
        /// </summary>
        public string Depositor { get; init; } = default!;

        /// <summary>
        /// 응답 수신 시각
        /// </summary>
        public string AppTime { get; init; } = default!;

        /// <summary>
        /// VAN 승인 시각
        /// </summary>
        public string VanAppTime { get; init; } = default!;

        /// <summary>
        /// 비고
        /// </summary>
        public string? Etc { get; init; }

        /// <summary>
        /// 요청 여부 (0: 송금 요청 전, 1: 송금 요청완료)
        /// </summary>
        public string Requested { get; set; } = default!;

        /// <summary>
        /// 응답 등록일
        /// </summary>
        public int? ResDt { get; set; }
    }
}
