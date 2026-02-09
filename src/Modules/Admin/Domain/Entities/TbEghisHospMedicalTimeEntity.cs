namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    /// <summary>
    /// 이지스병원진료시간정보
    /// </summary>
    public class TbEghisHospMedicalTimeEntity
    {
        /// <summary>
        /// 진료시간아이디
        /// </summary>
        public int MtId { get; set; }

        /// <summary>
        /// 요양기관키
        /// </summary>
        public string HospKey { get; set; } = null!;

        /// <summary>
        /// 요일
        /// </summary>
        public string MtWk { get; set; } = null!;

        /// <summary>
        /// 진료시간
        /// </summary>
        public string MtNm { get; set; } = null!;

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
