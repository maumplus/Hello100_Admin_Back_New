namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    /// <summary>
    /// 이지스병원정보
    /// </summary>
    public class TbEghisHospInfoEntity
    {
        /// <summary>
        /// 요양기관키
        /// </summary>
        public string HospKey { get; set; } = null!;

        /// <summary>
        /// 요양기관번호
        /// </summary>
        public string HospNo { get; set; } = null!;

        /// <summary>
        /// 챠트회사코드
        /// </summary>
        public string ChartCd { get; set; } = null!;

        /// <summary>
        /// 상세문구
        /// </summary>
        public string? Desc { get; set; }

        /// <summary>
        /// 사업자등록번호
        /// </summary>
        public string? BusinessNo { get; set; }

        /// <summary>
        /// 폐업유무
        /// </summary>
        public string ClosingYn { get; set; } = null!;

        /// <summary>
        /// 삭제유무
        /// </summary>
        public string DelYn { get; set; } = null!;

        /// <summary>
        /// 등록날짜
        /// </summary>
        public int RegDt { get; set; }

        /// <summary>
        /// 차트타입 E:이지스전자차트 , N;닉스펜
        /// </summary>
        public string ChartType { get; set; } = null!;

        /// <summary>
        /// 사업자구분(CT01:법인사업자, CT02:개인사업자)
        /// </summary>
        public string? BusinessLevel { get; set; }
    }
}
