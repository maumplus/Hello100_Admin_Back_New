namespace Hello100Admin.API.Constracts.Admin.VisitPurpose
{
    public record BulkUpdateCertificatesRequest
    {
        public required string HospKey { get; init; }

        public required List<BulkUpdateMyCertificatesRequest> Certificates { get; init; }
    }

    public record BulkUpdateMyCertificatesRequest
    {
        public string ReDocCd { get; set; } = default!;
        public string ShowYn { get; set; } = default!;
        public int SortNo { get; set; }
    }
}
