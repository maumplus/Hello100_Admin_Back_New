namespace Hello100Admin.Modules.Seller.Infrastructure.Persistence.DbModels.Common
{
    /// <summary>
    /// 관리자 테이블 레코드
    /// </summary>
    internal sealed record TbAdminRow
    {
        /// <summary>
        /// 관리자아이디
        /// </summary>
        public string AId { get; init; } = default!;

        /// <summary>
        /// 계정아이디
        /// </summary>
        public string AccId { get; init; } = default!;

        /// <summary>
        /// 계정비밀번호
        /// </summary>
        public string AccPwd { get; init; } = default!;

        /// <summary>
        /// 요양기관번호
        /// </summary>
        public string? HospNo { get; init; }

        /// <summary>
        /// 등급(tb_common:07)
        /// </summary>
        public string Grade { get; init; } = default!;

        /// <summary>
        /// 이름
        /// </summary>
        public string Name { get; init; } = default!;

        /// <summary>
        /// 전화번호
        /// </summary>
        public string? Tel { get; init; }

        /// <summary>
        /// 삭제유무
        /// </summary>
        public string DelYn { get; init; } = default!;

        /// <summary>
        /// 등록날짜
        /// </summary>
        public int RegDt { get; init; }

        /// <summary>
        /// 마지막 로그인 일시
        /// </summary>
        public int? LastLoginDt { get; init; }

        /// <summary>
        /// hosp 매핑 최종 동의 시간
        /// </summary>
        public int? AgreeDt { get; init; }

        /// <summary>
        /// 투팩터 인증 사용 여부 (0: 미사용, 1: 사용)
        /// </summary>
        public string Use2fa { get; init; } = default!;

        /// <summary>
        /// 계정 잠금 여부 (0: 미사용, 1: 사용)
        /// </summary>
        public string AccountLocked { get; init; } = default!;

        /// <summary>
        /// 로그인 실패 횟수
        /// </summary>
        public int LoginFailCount { get; init; }

        /// <summary>
        /// 마지막 비밀번호 변경 일시
        /// </summary>
        public int? LastPwdChangeDt { get; init; }

        /// <summary>
        /// 리프레시 토큰
        /// </summary>
        public string? RefreshToken { get; init; }

        /// <summary>
        /// 엑세스 토큰 '0'
        /// </summary>
        public string? AccessToken { get; init; }

        /// <summary>
        /// 2fa 시크릿 키
        /// </summary>
        public string? _2faKey { get; init; }

        /// <summary>
        /// 승인여부 (0: 미승인, 1: 승인)
        /// </summary>
        public string Approved { get; init; } = default!;

        /// <summary>
        /// 활성여부 (0: 비활성, 1:활성)
        /// </summary>
        public string Enabled { get; init; } = default!;

        /// <summary>
        /// 수정일자
        /// </summary>
        public int? ModDt { get; init; }
    }
}
