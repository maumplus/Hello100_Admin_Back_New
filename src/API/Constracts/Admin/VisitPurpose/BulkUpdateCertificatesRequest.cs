namespace Hello100Admin.API.Constracts.Admin.VisitPurpose
{
    public record BulkUpdateCertificatesRequest
    {
        public string ReDocCd { get; set; } = default!;
        public string ShowYn { get; set; } = default!;
        public int SortNo { get; set; }
    }
}
