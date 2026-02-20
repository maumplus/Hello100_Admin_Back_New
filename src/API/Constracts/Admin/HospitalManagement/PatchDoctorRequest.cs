namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public record PatchDoctorRequest
    {
        /// <summary>
        /// 의사사번
        /// </summary>
        public required string EmplNo { get; set; }
        /// <summary>
        /// 의사명
        /// </summary>
        public required string DoctNm { get; set; }
        /// <summary>
        /// 진료과코드
        /// </summary>
        public string? DeptCd { get; set; }
        /// <summary>
        /// 진료과명
        /// </summary>
        public string? DeptNm { get; set; }
        /// <summary>
        /// 인원표시(최소) 사용여부
        /// </summary>
        public required string ViewMinCntYn { get; set; }
        /// <summary>
        /// 대기 인원표시에 따른 최소인원
        /// </summary>
        public string? ViewMinCnt { get; set; }
        /// <summary>
        /// 시간표시(최소) 사용여부
        /// </summary>
        public required string ViewMinTimeYn { get; set; }
        /// <summary>
        /// 대기 시간표시에 따른 최소시간
        /// </summary>
        public string? ViewMinTime { get; set; }
        /// <summary>
        /// 의사사진
        /// </summary>
        public IFormFile? Image { get; set; }
    }
}
