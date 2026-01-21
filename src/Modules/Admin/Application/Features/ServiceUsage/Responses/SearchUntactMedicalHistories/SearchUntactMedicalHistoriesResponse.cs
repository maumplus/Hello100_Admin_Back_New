namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Responses.SearchUntactMedicalHistories
{
    public sealed record SearchUntactMedicalHistoriesResponse
    {
        public int TotalCount { get; init; }
        public int TotalRsrv { get; init; }
        public int TotalClinicEnd { get; init; }
        public int TotalClinicFail { get; init; }
        public int TotalClinicCancel { get; init; }
        public int TotalSuccessAmt { get; init; }
        public int TotalProgressAmt { get; init; }
        public int TotalFailAmt { get; init; }
        public int TotalSumAmt { get; init; }

        public List<SearchUntactMedicalHistoryItem> DetailList { get; init; } = default!;
    }

    public sealed record SearchUntactMedicalHistoryItem
    {
        public int RowNum { get; init; }
        public string UId { get; init; } = default!;
        public int MId { get; init; }
        public string? DoctNm { get; init; }
        public string? ReqDate { get; init; }
        public string Name { get; init; } = default!;
        public int PtntState { get; init; }
        public string? PtntStateNm { get; init; }
        public string ReceptType { get; init; } = default!;
        public string ReceptTypeNm { get; init; } = default!;
        public int Amount { get; init; }
        public string? ProcessStatusNm { get; init; }
        public int? ProcessStatus { get; init; }
        public int? PaymentId { get; init; }
    }
}
