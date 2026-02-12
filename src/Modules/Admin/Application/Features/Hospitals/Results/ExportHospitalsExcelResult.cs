namespace Hello100Admin.Modules.Admin.Application.Features.Hospitals.Results
{
    public class ExportHospitalsExcelResult
    {
        /// <summary>
        /// 요양기관 키
        /// </summary>
        public string HospKey { get; set; } = default!;
        /// <summary>
        /// 병원명
        /// </summary>
        public string Name { get; set; } = default!;
        /// <summary>
        /// 종별코드
        /// </summary>
        public string HospClsCd { get; set; } = default!;
        /// <summary>
        /// 종별코드명
        /// </summary>
        public string CmName { get; set; } = default!;
        /// <summary>
        /// 우편번호
        /// </summary>
        public string PostCd { get; set; } = default!;
        /// <summary>
        /// 주소
        /// </summary>
        public string Addr { get; set; } = default!;
        /// <summary>
        /// 대표번호
        /// </summary>
        public string Tel { get; set; } = default!;
        /// <summary>
        /// 병원URL
        /// </summary>
        public string Site { get; set; } = default!;
        /// <summary>
        /// 개설일자(yyyyMMdd)
        /// </summary>
        public string RegDt { get; set; } = default!;
        /// <summary>
        /// X좌표
        /// </summary>
        public double Lat { get; set; }
        /// <summary>
        /// Y좌표
        /// </summary>
        public double Lng { get; set; }
    }
}
