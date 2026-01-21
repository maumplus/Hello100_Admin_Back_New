namespace Hello100Admin.Modules.Admin.Infrastructure.Persistence.DbModels.ServiceUsage
{
    internal sealed record GetUntactMedicalHistoryForExportRow
    {
        /// <summary>
        /// 신청자명
        /// </summary>
        public string Name { get; init; } = default!;

        /// <summary>
        /// 진료예약일
        /// </summary>
        public string ReqDate { get; init; } = default!;

        /// <summary>
        /// 진료 유형
        /// </summary>
        public string ReceiptType { get; init; } = default!;

        /// <summary>
        /// 의사명
        /// </summary>
        public string DoctNm { get; init; } = default!;

        /// <summary>
        /// 결제 상태
        /// </summary>
        public string ProcessStatus { get; init; } = default!;

        /// <summary>
        /// 결제 금액
        /// </summary>
        public int Amount { get; init; }

        /// <summary>
        /// 진료 상태
        /// </summary>
        public string PtntState { get; init; } = default!;
    }
}
