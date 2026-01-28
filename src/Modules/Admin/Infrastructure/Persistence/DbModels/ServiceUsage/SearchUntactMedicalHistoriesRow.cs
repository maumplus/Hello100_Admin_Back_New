
namespace Hello100Admin.Modules.Admin.Infrastructure.Persistence.DbModels.ServiceUsage
{
    internal sealed class SearchUntactMedicalHistoriesRow
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
