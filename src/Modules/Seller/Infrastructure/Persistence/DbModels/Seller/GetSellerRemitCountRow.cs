namespace Hello100Admin.Modules.Seller.Infrastructure.Persistence.DbModels.Seller
{
    /// <summary>
    /// 판매자 송금 현황 집계 레코드
    /// </summary>
    internal sealed record GetSellerRemitCountRow
    {
        /// <summary>
        /// 대기 상태 송금 금액 합계
        /// </summary>
        public long PendingAmount { get; init; }

        /// <summary>
        /// 요청 상태 송금 금액 합계
        /// </summary>
        public long RequestAmount { get; init; }

        /// <summary>
        /// 성공 상태 송금 금액 합계
        /// </summary>
        public long SuccessAmount { get; init; }

        /// <summary>
        /// 실패 상태 송금 금액 합계
        /// </summary>
        public long FailAmount { get; init; }

        /// <summary>
        /// 삭제(취소) 상태 송금 금액 합계
        /// </summary>
        public long DeleteAmount { get; init; }

        /// <summary>
        /// 대기 상태 건수
        /// </summary>
        public int PendingCount { get; init; }

        /// <summary>
        /// 요청 상태 건수
        /// </summary>
        public int RequestCount { get; init; }

        /// <summary>
        /// 성공 상태 건수
        /// </summary>
        public int SuccessCount { get; init; }

        /// <summary>
        /// 실패 상태 건수
        /// </summary>
        public int FailCount { get; init; }

        /// <summary>
        /// 삭제(취소) 상태 건수
        /// </summary>
        public int DeleteCount { get; init; }
    }
}
