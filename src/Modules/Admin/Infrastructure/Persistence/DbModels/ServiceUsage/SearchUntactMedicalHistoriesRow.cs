
namespace Hello100Admin.Modules.Admin.Infrastructure.Persistence.DbModels.ServiceUsage
{
    internal sealed class SearchUntactMedicalHistoriesRow
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

        public List<SearchUntactMedicalHistoryDetailRow> DetailList { get; set; } = default!;
    }

    internal sealed class SearchUntactMedicalHistoryDetailRow
    {
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
