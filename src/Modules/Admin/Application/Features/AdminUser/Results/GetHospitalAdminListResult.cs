namespace Hello100Admin.Modules.Admin.Application.Features.AdminUser.Results
{
    public class GetHospitalAdminListResult
    {
        /// <summary>
        /// 행 번호
        /// </summary>
        public int RowNum { get; set; }
        /// <summary>
        /// 관리자아이디
        /// </summary>
        public string AId { get; set; } = default!;
        /// <summary>
        /// 계정아이디
        /// </summary>
        public string AccId { get; set; } = default!;
        /// <summary>
        /// 요양기관번호
        /// </summary>
        public string HospNo { get; set; } = default!;
        /// <summary>
        /// 등급(tb_common:07)
        /// </summary>
        public string Grade { get; set; } = default!;
        /// <summary>
        /// 전화전호
        /// </summary>
        public string? Tel { get; set; }
        /// <summary>
        /// 최종접속일
        /// </summary>
        public string LastLoginDt { get; set; } = default!;
        /// <summary>
        /// 매핑 최종 동의 시간
        /// </summary>
        public string AgreeDt { get; set; } = default!;
        /// <summary>
        /// QR 아이디
        /// </summary>
        public string QId { get; set; } = default!;
        /// <summary>
        /// 요양기관 키
        /// </summary>
        public string HospKey { get; set; } = default!;
        /// <summary>
        /// 병원명
        /// </summary>
        public string HospitalName { get; set; } = default!;
        /// <summary>
        /// 대리점 코드
        /// </summary>
        public string? Agency { get; set; }
        /// <summary>
        /// 대리점명
        /// </summary>
        public string? AgencyNm { get; set; }
        /// <summary>
        /// QR코드 생성일
        /// </summary>
        public string? QrCreateDt { get; set; }
    }
}
