namespace Hello100Admin.Modules.Seller.Application.Features.Seller.ReadModels.GetSellerRemitList
{
    public class GetHospSellerRemitListReadModel
    {
        /// <summary>
        /// 총 건수
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 행 번호
        /// </summary>
        public int RowNum { get; set; }

        /// <summary>
        /// 요양 기관 번호
        /// </summary>
        public string HospNo { get; set; } = default!;

        /// <summary>
        /// 병원명
        /// </summary>
        public string HospName { get; set; } = default!;

        /// <summary>
        /// 은행 로고 이미지 경로
        /// </summary>
        public string? BankImgPath { get; set; }

        /// <summary>
        /// 고유 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 판매자 일련번호
        /// </summary>
        public int HospSellerId { get; set; }

        /// <summary>
        /// 총 송금 금액
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 송금 요청 금액
        /// </summary>
        public int VaMny { get; set; }

        /// <summary>
        /// 입금자명
        /// </summary>
        public string? VaName { get; set; }

        /// <summary>
        /// 요청자 IP
        /// </summary>
        public string? CustIp { get; set; }

        /// <summary>
        /// 송금 상태 (0: 대기(등록 상태), 1:요청,  2: 성공(완료), 3: 재요청, 4: 실패, 5: 취소(삭제) )
        /// </summary>
        public string Status { get; set; } = null!;

        /// <summary>
        /// 요청 여부 (0: 송금 요청 전, 1: 송금 요청완료)
        /// </summary>
        public string Requested { get; set; } = null!;

        /// <summary>
        /// 거래 일련번호
        /// </summary>
        public string? TradeSeq { get; set; }

        /// <summary>
        /// 송금 일자
        /// </summary>
        public string? TradeDate { get; set; }

        /// <summary>
        /// 모계좌 잔액
        /// </summary>
        public int? BalAmount { get; set; }

        /// <summary>
        /// 은행 코드
        /// </summary>
        public string? BankCd { get; set; }

        /// <summary>
        /// 요청 시각
        /// </summary>
        public string? AppTime { get; set; }

        /// <summary>
        /// 기관 처리 시각
        /// </summary>
        public string? VanApptime { get; set; }

        /// <summary>
        /// 은행명
        /// </summary>
        public string? BankName { get; set; }

        /// <summary>
        /// 계좌번호
        /// </summary>
        public string? Account { get; set; }

        /// <summary>
        /// 모계좌 예금주명
        /// </summary>
        public string? Depositor { get; set; }

        /// <summary>
        /// 결과 코드
        /// </summary>
        public string? ResCd { get; set; }

        /// <summary>
        /// 결과 메시지
        /// </summary>
        public string? ResMsg { get; set; }

        /// <summary>
        /// 영문 메시지
        /// </summary>
        public string? ResEnMsg { get; set; }

        /// <summary>
        /// 관리자 아이디
        /// </summary>
        public string RegAid { get; set; } = null!;

        /// <summary>
        /// 요청 등록일
        /// </summary>
        public int RegDt { get; set; }

        /// <summary>
        /// 응답 등록일
        /// </summary>
        public int? ResDt { get; set; }

        /// <summary>
        /// 갱신 등록일
        /// </summary>
        public int? RefreshDt { get; set; }

        /// <summary>
        /// 비고
        /// </summary>
        public string? Etc { get; set; }
    }
}
