namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    public class TbUserEntity
    {
        /// <summary>
        /// 고객아이디
        /// </summary>
        public string UId { get; set; } = null!;

        /// <summary>
        /// SNS아이디(카카오,네이버,페이스북,이메일)
        /// </summary>
        public string? SnsId { get; set; }

        /// <summary>
        /// 이메일
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// 비밀번호
        /// </summary>
        public string? Pwd { get; set; }

        /// <summary>
        /// 로그인타입(tb_common테이블 cls_cd 01참고)
        /// </summary>
        public string LoginType { get; set; } = null!;

        /// <summary>
        /// 본인인증아이디
        /// </summary>
        public int? Said { get; set; }

        /// <summary>
        /// 삭제여부
        /// </summary>
        public string DelYn { get; set; } = null!;

        /// <summary>
        /// 등록날짜
        /// </summary>
        public int RegDt { get; set; }

        public string? Ci { get; set; }

        /// <summary>
        /// 0: 선택안함
        /// 1: 푸시알림
        /// 2: SMS
        /// 4: 메일
        /// 3: 푸시/SMS
        /// 5: 푸시/메일
        /// 6: SMS/메일
        /// 7: 푸시/SMS/메일
        /// </summary>
        public int MkCd { get; set; }

        /// <summary>
        /// 사용자권한(0:일반, 1:테스트계정)
        /// </summary>
        public int UserRole { get; set; }
    }
}
