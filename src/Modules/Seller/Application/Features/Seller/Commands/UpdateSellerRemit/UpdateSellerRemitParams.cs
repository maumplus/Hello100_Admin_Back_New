namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.UpdateSellerRemit
{
    public record UpdateSellerRemitParams
    {
        /// <summary>결과 코드 (0000: 성공)</summary>
        public string ResCd { get; init; } = default!;

        /// <summary>결과 메시지 (한글)</summary>
        public string ResMsg { get; init; } = default!;

        /// <summary>결과 메시지 (영문)</summary>
        public string ResEnMsg { get; init; } = default!;

        /// <summary>거래 일련번호</summary>
        public string TradeSeq { get; init; } = default!;

        /// <summary>거래 일자 (yyyyMMdd)</summary>
        public string TradeDate { get; init; } = default!;

        /// <summary>송금 금액</summary>
        public long Amount { get; init; }

        /// <summary>송금 후 남은 잔액</summary>
        public long BalAmount { get; init; }

        /// <summary>입금 은행 코드</summary>
        public string BankCode { get; init; } = default!;

        /// <summary>입금 은행명</summary>
        public string BankName { get; init; } = default!;

        /// <summary>입금 계좌번호 (마스킹)</summary>
        public string Account { get; init; } = default!;

        /// <summary>예금주명</summary>
        public string Depositor { get; init; } = default!;

        /// <summary>응답 수신 시각</summary>
        public string AppTime { get; init; } = default!;

        /// <summary>VAN 승인 시각</summary>
        public string VanAppTime { get; init; } = default!;
    }
}
