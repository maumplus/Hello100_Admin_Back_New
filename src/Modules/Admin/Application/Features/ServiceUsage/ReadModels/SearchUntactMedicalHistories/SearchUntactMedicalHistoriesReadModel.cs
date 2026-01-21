namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.ReadModels.SearchUntactMedicalHistories
{
    public sealed class SearchUntactMedicalHistoriesReadModel
    {
        public int TotalCount { get; set; }
        public int TotalRsrv { get; set; }
        public int TotalClinicEnd { get; set; }
        public int TotalClinicFail { get; set; }
        public int TotalClinicCancel { get; set; }
        public int TotalSuccessAmt { get; set; }
        public int TotalProgressAmt { get; set; }
        public int TotalFailAmt { get; set; }
        public int TotalSumAmt { get; set; }

        public List<SearchUntactMedicalHistoryItemReadModel> DetailList { get; set; } = default!;
    }

    public sealed class SearchUntactMedicalHistoryItemReadModel
    {
        public int RowNum { get; set; }
        public string UId { get; set; } = default!;
        public int MId { get; set; }
        public string? DoctNm { get; set; }
        public string? ReqDate { get; set; }
        public string Name { get; set; } = default!;
        public int PtntState { get; set; }
        public string? PtntStateNm { get; set; }
        public string ReceptType { get; set; } = default!;
        public string ReceptTypeNm { get; set; } = default!;
        public int Amount { get; set; }
        public string? ProcessStatusNm { get; set; }
        public int? ProcessStatus { get; set; }
        public int? PaymentId { get; set; }
    }
}
