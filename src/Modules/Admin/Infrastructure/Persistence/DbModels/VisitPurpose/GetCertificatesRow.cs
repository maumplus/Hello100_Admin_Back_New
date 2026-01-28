namespace Hello100Admin.Modules.Admin.Infrastructure.Persistence.DbModels.VisitPurpose
{
    internal sealed record GetCertificatesRow
    {
        public string Name { get; set; } = default!;
        public string HospKey { get; set; } = default!;
        public string ReDocCd { get; set; } = default!;
        public string ShowYn { get; init; } = default!;
        public Int16 SortNo { get; set; }
    }
}
