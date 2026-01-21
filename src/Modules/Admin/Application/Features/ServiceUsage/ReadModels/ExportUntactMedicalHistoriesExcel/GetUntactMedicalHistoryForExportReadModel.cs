namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.ReadModels.ExportUntactMedicalHistoriesExcel
{
    public sealed class GetUntactMedicalHistoryForExportReadModel
    {
        /// <summary>
        /// 신청자명
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// 진료예약일
        /// </summary>
        public string ReqDate { get; set; } = default!;

        /// <summary>
        /// 진료 유형
        /// </summary>
        public string ReceiptType { get; set; } = default!;

        /// <summary>
        /// 의사명
        /// </summary>
        public string DoctNm { get; set; } = default!;

        /// <summary>
        /// 결제 상태
        /// </summary>
        public string? ProcessStatus { get; set; }

        /// <summary>
        /// 결제 금액
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 진료 상태
        /// </summary>
        public string PtntState { get; set; } = default!;
    }
}
