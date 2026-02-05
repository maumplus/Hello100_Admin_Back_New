namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    /// <summary>
    /// 승인요청정보테이블
    /// </summary>
    public class TbEghisHospApprovalInfoEntity
    {
        /// <summary>
        /// 승인요청아이디
        /// </summary>
        public int ApprId { get; set; }

        /// <summary>
        /// 승인유형[KW: 홍보키워드]
        /// </summary>
        public string ApprType { get; set; } = null!;

        /// <summary>
        /// 요양기관 키
        /// </summary>
        public string HospKey { get; set; } = null!;

        public string? Data { get; set; }

        /// <summary>
        /// 승인담당자
        /// </summary>
        public string? Aid { get; set; }

        /// <summary>
        /// 요청담당자
        /// </summary>
        public string? ReqAid { get; set; }

        /// <summary>
        /// 승인유무
        /// </summary>
        public string ApprYn { get; set; } = null!;

        /// <summary>
        /// 승인날짜
        /// </summary>
        public int? ApprDt { get; set; }

        /// <summary>
        /// 등록날짜
        /// </summary>
        public int RegDt { get; set; }
    }
}
