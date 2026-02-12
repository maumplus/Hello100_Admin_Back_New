namespace Hello100Admin.API.Constracts.Admin.Hospitals
{
    public sealed record DeleteHospitalRequest
    {
        public required string HospKey { get; init; } = default!;
    }
}
