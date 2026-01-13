namespace Hello100Admin.Modules.Seller.Application.Features.Seller.ReadModels.GetSellerRemitWaitList
{
    public class GetSellerRemitWaitListReadModel
    {
        /// <summary>
        /// 행 번호
        /// </summary>
        public int RowNum { get; set; }

        /// <summary>
        /// 고유 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 요양기관번호
        /// </summary>
        public string HospNo { get; set; } = default!;

        /// <summary>
        /// 병원명
        /// </summary>
        public string HospName { get; set; } = default!;

        /// <summary>
        /// 은행 코드
        /// </summary>
        public string BankCd { get; set; } = default!;

        /// <summary>
        /// 은행명
        /// </summary>
        public string BankName { get; set; } = default!;

        /// <summary>
        /// 은행 로고 이미지 경로
        /// </summary>
        public string? BankImgPath { get; set; }

        /// <summary>
        /// 계좌 번호 (하이픈 없이)
        /// </summary>
        public string DepositNo { get; set; } = default!;

        /// <summary>
        /// 예금주명
        /// </summary>
        public string Depositor { get; set; } = default!;

        /// <summary>
        /// 총 송금 금액
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 송금 상태 (0: 대기(등록 상태), 1:요청,  2: 성공(완료), 3: 재요청, 4: 실패, 5: 취소(삭제) )
        /// </summary>
        public string Status { get; set; } = null!;

        /// <summary>
        /// 비고
        /// </summary>
        public string? Etc { get; set; }
    }
}
