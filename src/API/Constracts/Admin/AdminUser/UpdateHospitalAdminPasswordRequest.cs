namespace Hello100Admin.API.Constracts.Admin.AdminUser
{
    public sealed record UpdateHospitalAdminPasswordRequest
    {
        /// <summary>
        /// 관리자아이디
        /// </summary>
        public required string AId { get; init; }
        /// <summary>
        /// 새 비밀번호
        /// </summary>
        public required string NewPassword { get; init; }
    }
}
