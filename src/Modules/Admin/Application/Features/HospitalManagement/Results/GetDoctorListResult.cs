namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results
{
    public class GetDoctorListResult
    {
        /// <summary>
        /// 요양기관번호
        /// </summary>
        public string HospNo { get; set; } = string.Empty;
        /// <summary>
        /// 요양기관키
        /// </summary>
        public string HospKey { get; set; } = string.Empty;
        /// <summary>
        /// 의사사번
        /// </summary>
        public string EmplNo { get; set; } = string.Empty;
        /// <summary>
        /// 의사 면허번호
        /// </summary>
        public string DoctNo { get; set; } = string.Empty;
        /// <summary>
        /// 의사명
        /// </summary>
        public string DoctNm { get; set; } = string.Empty;
        /// <summary>
        /// 진료과코드
        /// </summary>
        public string DeptCd { get; set; } = string.Empty;
        /// <summary>
        /// 진료과명
        /// </summary>
        public string DeptNm { get; set; } = string.Empty;
        /// <summary>
        /// 진료날짜
        /// </summary>
        public string WeeksNm { get; set; } = string.Empty;
        /// <summary>
        /// 노출여부
        /// </summary>
        public string FrontViewRole { get; set; } = string.Empty;
    }
}
