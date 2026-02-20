namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    public class TbCommonEntity
    {
        /// <summary>
        /// 공통시퀀스
        /// </summary>
        public int CmSeq { get; set; }

        /// <summary>
        /// 클래스코드
        /// </summary>
        public string ClsCd { get; set; } = null!;

        /// <summary>
        /// 공통코드
        /// </summary>
        public string CmCd { get; set; } = null!;

        /// <summary>
        /// 클래스이름
        /// </summary>
        public string ClsName { get; set; } = null!;

        /// <summary>
        /// 공통이름
        /// </summary>
        public string CmName { get; set; } = null!;

        /// <summary>
        /// 언어코드
        /// </summary>
        public string Locale { get; set; } = null!;

        /// <summary>
        /// 삭제여부
        /// </summary>
        public string DelYn { get; set; } = null!;

        /// <summary>
        /// 정렬순서
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 등록날짜
        /// </summary>
        public string RegDt { get; set; } = null!; // 실제 DB Entity는 int type임. 매핑 주의
    }
}
