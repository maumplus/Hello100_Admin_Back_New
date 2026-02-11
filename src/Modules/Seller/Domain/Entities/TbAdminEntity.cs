namespace Hello100Admin.Modules.Seller.Domain.Entities
{
    /// <summary>
    /// 관리자정보
    /// </summary>
    public class TbAdminEntity
    {
        /// <summary>
        /// 관리자아이디
        /// </summary>
        public string Aid { get; set; } = null!;

        /// <summary>
        /// 계정아이디
        /// </summary>
        public string AccId { get; set; } = null!;

        /// <summary>
        /// 계정비밀번호
        /// </summary>
        public string AccPwd { get; set; } = null!;

        /// <summary>
        /// 요양기관번호
        /// </summary>
        public string? HospNo { get; set; }

        /// <summary>
        /// 등급(tb_common:07)
        /// </summary>
        public string Grade { get; set; } = null!;

        /// <summary>
        /// 이름
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 전화번호
        /// </summary>
        public string? Tel { get; set; }

        /// <summary>
        /// 삭제유무
        /// </summary>
        public string DelYn { get; set; } = null!;

        /// <summary>
        /// 등록날짜
        /// </summary>
        public int RegDt { get; set; }

        /// <summary>
        /// 마지막 로그인 일시
        /// </summary>
        public int? LastLoginDt { get; set; }

        /// <summary>
        /// hosp 매핑 최종 동의 시간
        /// </summary>
        public int? AgreeDt { get; set; }

        /// <summary>
        /// 투팩터 인증 사용 여부 (0: 미사용, 1: 사용)
        /// </summary>
        public string Use2fa { get; set; } = null!;

        /// <summary>
        /// 계정 잠금 여부 (0: 미사용, 1: 사용)
        /// </summary>
        public string AccountLocked { get; set; } = null!;

        /// <summary>
        /// 로그인 실패 횟수
        /// </summary>
        public int LoginFailCount { get; set; }

        /// <summary>
        /// 마지막 비밀번호 변경 일시
        /// </summary>
        public int? LastPwdChangeDt { get; set; }

        /// <summary>
        /// 리프레시 토큰
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// 엑세스 토큰 &apos;0&apos;
        /// </summary>
        public string? AccessToken { get; set; }

        /// <summary>
        /// 2fa 시크릿 키
        /// </summary>
        public string? _2faKey { get; set; }

        ///// <summary>
        ///// 승인여부  (0: 미승인, 1: 승인)
        ///// </summary>
        //public string Approved { get; set; } = null!;

        ///// <summary>
        ///// 활성여부  (0: 비활성, 1:활성)
        ///// </summary>
        //public string Enabled { get; set; } = null!;

        ///// <summary>
        ///// 수정일자
        ///// </summary>
        //public int? ModDt { get; set; }
    }
}
