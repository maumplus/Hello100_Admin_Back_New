namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results
{
    public sealed class GetDoctorUntactApplicationResult
    {
        /// <summary>
        /// 요양기관번호
        /// </summary>
        public string HospNo { get; set; } = default!;
        /// <summary>
        /// 요양기관키
        /// </summary>
        public string HospKey { get; set; } = default!;
        /// <summary>
        /// 병원명
        /// </summary>
        public string HospName { get; set; } = default!;
        /// <summary>
        /// 우편번호
        /// </summary>
        public string PostCd { get; set; } = default!;
        /// <summary>
        /// 주소
        /// </summary>
        public string Addr { get; set; } = default!;
        /// <summary>
        /// 전화번호
        /// </summary>
        public string Tel { get; set; } = default!;
        /// <summary>
        /// 의사 정보
        /// </summary>
        public GetDoctorUntactApplicationResultDoctorInfoItem DoctorInfo { get; set; } = default!;
        /// <summary>
        /// 의사 면허종류 목록
        /// </summary>
        public List<GetDoctorUntactApplicationResultLicenseTypeItem> LicenseTypes { get; set; } = default!;
    }

    public sealed class GetDoctorUntactApplicationResultLicenseTypeItem
    {
        /// <summary>
        /// 공통시퀀스
        /// </summary>
        public int CmSeq { get; set; }
        /// <summary>
        /// 클래스코드
        /// </summary>
        public string ClsCd { get; set; } = default!;
        /// <summary>
        /// 공통코드
        /// </summary>
        public string CmCd { get; set; } = default!;
        /// <summary>
        /// 클래스이름
        /// </summary>
        public string ClsName { get; set; } = default!;
        /// <summary>
        /// 공통이름
        /// </summary>
        public string CmName { get; set; } = default!;
        /// <summary>
        /// 정렬순서
        /// </summary>
        public int Sort { get; set; }
    }

    public sealed class GetDoctorUntactApplicationResultDoctorInfoItem
    {
        /// <summary>
        /// 의사 사번
        /// </summary>
        public string EmplNo { get; set; } = default!;
        /// <summary>
        /// 의사 면허번호
        /// </summary>
        public string DoctNo { get; set; } = default!;
        /// <summary>
        /// 의사명
        /// </summary>
        public string DoctNm { get; set; } = default!;
    }
}
