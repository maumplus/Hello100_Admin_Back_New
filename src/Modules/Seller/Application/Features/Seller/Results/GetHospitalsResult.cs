namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Results
{
    public class GetHospitalsResult
    {
        /// <summary>
        /// 요양기관 번호
        /// </summary>
        public string HospNo { get; set; } = default!;

        /// <summary>
        /// 병원명
        /// </summary>
        public string HospName { get; set; } = default!;

        /// <summary>
        /// 사업자번호
        /// </summary>
        public string? BusinessNo { get; set; }

        /// <summary>
        /// 차트 구분
        /// </summary>
        public string ChartType { get; set; } = default!;

        /// <summary>
        /// 사업자 구분 (CT01: 법인, CT02: 개인)
        /// </summary>
        public string? BusinessLevel { get; set; }

        /// <summary>
        /// 병원 주소
        /// </summary>
        public string HospAddr { get; set; } = default!;

        /// <summary>
        /// 병원 우편번호
        /// </summary>
        public string HospPostCd { get; set; } = default!;

        /// <summary>
        /// 병원 전화번호
        /// </summary>
        public string HospTel { get; set; } = default!;
    }
}
