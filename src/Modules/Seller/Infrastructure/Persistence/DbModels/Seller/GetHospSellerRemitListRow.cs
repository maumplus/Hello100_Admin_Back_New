namespace Hello100Admin.Modules.Seller.Infrastructure.Persistence.DbModels.Seller
{
    internal sealed record GetHospSellerRemitListRow
    {
        /// <summary>
        /// 총 건수
        /// </summary>
        public int TotalCount { get; init; }

        /// <summary>
        /// 행 번호
        /// </summary>
        public int RowNum { get; init; }
        
        /// <summary>
        /// 요양 기관 번호
        /// </summary>
        public string HospNo { get; init; } = default!;

        /// <summary>
        /// 병원명
        /// </summary>
        public string HospName { get; init; } = default!;

        /// <summary>
        /// 은행 로고 이미지 경로
        /// </summary>
        public string? BankImgPath { get; init; }

        /// <summary>
        /// 고유 ID
        /// </summary>
        public int Id { get; init; }

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
        public string? VaName { get; init; }

        /// <summary>
        /// 요청자 IP
        /// </summary>
        public string? CustIp { get; init; }

        /// <summary>
        /// 송금 상태 (0: 대기(등록 상태), 1:요청,  2: 성공(완료), 3: 재요청, 4: 실패, 5: 취소(삭제) )
        /// </summary>
        public string Status { get; init; } = null!;

        /// <summary>
        /// 요청 여부 (0: 송금 요청 전, 1: 송금 요청완료)
        /// </summary>
        public string Requested { get; init; } = null!;

        /// <summary>
        /// 거래 일련번호
        /// </summary>
        public string? TradeSeq { get; init; }

        /// <summary>
        /// 송금 일자
        /// </summary>
        public string? TradeDate { get; init; }

        /// <summary>
        /// 모계좌 잔액
        /// </summary>
        public int? BalAmount { get; init; }

        /// <summary>
        /// 은행 코드
        /// </summary>
        public string? BankCd { get; init; }

        /// <summary>
        /// 요청 시각
        /// </summary>
        public string? AppTime { get; init; }

        /// <summary>
        /// 기관 처리 시각
        /// </summary>
        public string? VanApptime { get; init; }

        /// <summary>
        /// 은행명
        /// </summary>
        public string? BankName { get; init; }

        /// <summary>
        /// 계좌번호
        /// </summary>
        public string? Account { get; init; }

        /// <summary>
        /// 모계좌 예금주명
        /// </summary>
        public string? Depositor { get; init; }

        /// <summary>
        /// 결과 코드
        /// </summary>
        public string? ResCd { get; init; }

        /// <summary>
        /// 결과 메시지
        /// </summary>
        public string? ResMsg { get; init; }

        /// <summary>
        /// 영문 메시지
        /// </summary>
        public string? ResEnMsg { get; init; }

        /// <summary>
        /// 관리자 아이디
        /// </summary>
        public string RegAid { get; init; } = null!;

        /// <summary>
        /// 요청 등록일
        /// </summary>
        public int RegDt { get; init; }

        /// <summary>
        /// 응답 등록일
        /// </summary>
        public int? ResDt { get; init; }

        /// <summary>
        /// 갱신 등록일
        /// </summary>
        public int? RefreshDt { get; init; }

        /// <summary>
        /// 비고
        /// </summary>
        public string? Etc { get; init; }
    }
}
