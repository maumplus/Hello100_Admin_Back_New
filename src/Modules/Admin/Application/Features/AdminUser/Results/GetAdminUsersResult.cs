namespace Hello100Admin.Modules.Admin.Application.Features.AdminUser.Results
{
    public class GetAdminUsersResult
    {
        /// <summary>
        /// 행 번호
        /// </summary>
        public int RowNum { get; set; }
        /// <summary>
        /// 관리자아이디
        /// </summary>
        public string AId { get; set; } = default!;
        /// <summary>
        /// 계정아이디
        /// </summary>
        public string AccId { get; set; } = default!;
        /// <summary>
        /// 계정비밀번호
        /// </summary>
        public string AccPwd { get; set; } = default!;
        /// <summary>
        /// 등급(tb_common:07)
        /// </summary>
        public string Grade { get; set; } = default!;
        /// <summary>
        /// 등급명
        /// </summary>
        public string GradeName { get; set; } = default!;
        /// <summary>
        /// 이름
        /// </summary>
        public string Name { get; set; } = default!;
        /// <summary>
        /// 전화전호
        /// </summary>
        public string Tel { get; set; } = default!;
        /// <summary>
        /// 삭제유무
        /// </summary>
        public string DelYn { get; set; } = default!;
        /// <summary>
        /// 최종접속일 (yyyy-MM-dd HH:mm)
        /// </summary>
        public string LastLoginDt { get; set; } = default!;
    }
}
