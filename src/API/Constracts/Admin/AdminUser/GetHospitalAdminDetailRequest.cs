namespace Hello100Admin.API.Constracts.Admin.AdminUser
{
    public sealed record GetHospitalAdminDetailRequest
    {
        /// <summary>
        /// 관리자아이디
        /// </summary>
        public required string AId { get; init; }
    }
}
