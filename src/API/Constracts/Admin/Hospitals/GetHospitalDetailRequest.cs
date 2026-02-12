namespace Hello100Admin.API.Constracts.Admin.Hospitals
{
    public sealed record GetHospitalDetailRequest
    {
        public required string HospKey { get; init; } = default!;
    }
}
