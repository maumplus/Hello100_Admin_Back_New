namespace Hello100Admin.API.Constracts.Admin.HospitalUser
{
    public sealed record UpdateHospitalUserRoleRequest
    {
        public required int UserRole { get; init; }
    }
}
