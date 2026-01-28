namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.ReadModels.GetCertificates
{
    public sealed class GetCertificatesReadModel
    {
        public int TotalCount { get; set; }
        public List<GetCertificatesItemReadModel>? List { get; set; }
    }

    public sealed class GetCertificatesItemReadModel
    {
        public string Name { get; set; } = default!;
        public string HospKey { get; set; } = default!;
        public string ReDocCd { get; set; } = default!;
        public string ShowYn { get; init; } = default!;
        public Int16 SortNo { get; set; }
    }
}
