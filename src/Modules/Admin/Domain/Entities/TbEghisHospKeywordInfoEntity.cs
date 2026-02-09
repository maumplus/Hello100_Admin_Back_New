namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    /// <summary>
    /// 병원홍보태그정보
    /// </summary>
    public class TbEghisHospKeywordInfoEntity
    {
        /// <summary>
        /// 대표키워드
        /// </summary>
        public int MasterSeq { get; set; }

        /// <summary>
        /// 상세키워드
        /// </summary>
        public int DetailSeq { get; set; }

        /// <summary>
        /// 요양기관키
        /// </summary>
        public string HospKey { get; set; } = null!;

        /// <summary>
        /// 요양기관키
        /// </summary>
        public string HospNo { get; set; } = null!;

        /// <summary>
        /// 태그명
        /// </summary>
        public string TagNm { get; set; } = null!;

        /// <summary>
        /// 삭제유무
        /// </summary>
        public string DelYn { get; set; } = null!;

        /// <summary>
        /// 등록날짜
        /// </summary>
        public int RegDt { get; set; }
    }
}
