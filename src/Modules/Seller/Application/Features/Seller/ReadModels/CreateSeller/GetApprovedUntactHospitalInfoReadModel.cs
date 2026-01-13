namespace Hello100Admin.Modules.Seller.Application.Features.Seller.ReadModels.CreateSeller
{
    public class GetApprovedUntactHospitalInfoReadModel
    {
        /// <summary>
        /// 요양 기관 키
        /// </summary>
        public string HospKey { get; set; } = "";

        /// <summary>
        /// 요양 기관 번호
        /// </summary>
        public string HospNo { get; set; } = "";

        /// <summary>
        /// 병원 명
        /// </summary>
        public string HospName { get; set; } = "";

        /// <summary>
        /// 사업자 등록 번호
        /// </summary>
        public string? BusinessNo { get; set; }

        /// <summary>
        /// 차트 타입
        /// E: 이지스 차트, N: 닉스 차트
        /// </summary>
        public string ChartType { get; set; } = "";

        /// <summary>
        /// 사업자 구분
        /// CT01: 법인 사업자, CT02: 개인 사업자
        /// </summary>
        public string? BusinessLevel { get; set; }

        /// <summary>
        /// 병원 주소
        /// </summary>
        public string HospAddr { get; set; } = "";

        /// <summary>
        /// 병원 우편 번호
        /// </summary>
        public string HospPostCd { get; set; } = "";

        /// <summary>
        /// 병원 전화 번호
        /// </summary>
        public string HospTel { get; set; } = "";
    }
}
