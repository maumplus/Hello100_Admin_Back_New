namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Responses.GetCertificates
{
    public sealed record GetCertificatesResponse
    {
        public int TotalCount { get; init; }
        public List<GetCertificatesItemResponse>? List { get; init; }
    }

    public record GetCertificatesItemResponse
    {
        public string Name { get; init; } = default!;
        public string ReDocCd { get; init; } = default!;
        public string ShowYn { get; init; } = default!;
        public Int16 SortNo { get; init; }
    }
}
