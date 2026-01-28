namespace Hello100Admin.Modules.Admin.Application.Features.HospitalStatistics.Results
{
    public sealed class GetRegistrationStatsByVisitPurposeResult
    {
        public string VisitPurpose { get; set; } = default!;
        public int Recept { get; set; }
        public int Cancel { get; set; }
    }
}
