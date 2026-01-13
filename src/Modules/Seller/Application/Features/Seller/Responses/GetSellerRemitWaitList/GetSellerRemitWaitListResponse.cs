namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.GetSellerRemitWaitList
{
    public record GetSellerRemitWaitListResponse
    {
        public IList<GetSellerRemitWaitListInfo> List { get; set; } = new List<GetSellerRemitWaitListInfo>();
    }

    public record GetSellerRemitWaitListInfo
    {
        /// <summary>
        /// 순번
        /// </summary>
        public int RowNum { get; init; }

        /// <summary>
        /// 일련번호
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// 병원명
        /// </summary>
        public string HospName { get; init; }

        /// <summary>
        /// 병원 고유 번호
        /// </summary>
        public string HospNo { get; init; }

        /// <summary>
        /// 은행 코드
        /// </summary>
        public string BankCd { get; init; }

        /// <summary>
        /// 은행명
        /// </summary>
        public string BankName { get; init; }

        /// <summary>
        /// 은행 이미지 경로
        /// </summary>
        public string? BankImgPath { get; init; }

        /// <summary>
        /// 예금 계좌번호
        /// </summary>
        public string DepositNo { get; init; }

        /// <summary>
        /// 예금주명
        /// </summary>
        public string Depositor { get; init; }

        /// <summary>
        /// 송금 금액
        /// </summary>
        public int Amount { get; init; }

        /// <summary>
        /// 송금 상태 (0: 대기, 1: 요청, 2: 완료 등)
        /// </summary>
        public string Status { get; init; }

        /// <summary>
        /// 비고
        /// </summary>
        public string Etc { get; init; }
    }
}
