namespace Hello100Admin.API.Constracts.Admin.AdminUser
{
    public record DeleteHospitalAdminMappingRequest
    {
        /// <summary>
        /// 매핑 삭제하려는 병원 관리자 아이디
        /// </summary>
        public required string HospitalAId { get; init; }
        /// <summary>
        /// 매핑 삭제를 위한 로그인 관리자 비밀번호
        /// </summary>
        public required string AccPwd { get; init; }
    }
}
