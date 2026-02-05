namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    /// <summary>
    /// 병원별진료과
    /// </summary>
    public class TbHospitalMedicalInfoEntity
    {
        /// <summary>
        /// 진료과코드(tb_common:03)
        /// </summary>
        public string MdCd { get; set; } = null!;

        /// <summary>
        /// 요양기관키
        /// </summary>
        public string HospKey { get; set; } = null!;

        /// <summary>
        /// 등록날짜
        /// </summary>
        public int RegDt { get; set; }

        public string MainYn { get; set; } = null!;
    }
}
