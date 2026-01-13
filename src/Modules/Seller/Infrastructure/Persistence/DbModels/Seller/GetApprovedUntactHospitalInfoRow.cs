namespace Hello100Admin.Modules.Seller.Infrastructure.Persistence.DbModels.Seller
{
    /// <summary>
    /// 비대면 진료 승인 병원
    /// </summary>
    internal sealed record GetApprovedUntactHospitalInfoRow
    {
        /// <summary>
        /// 요양 기관 키
        /// </summary>
        public string HospKey { get; init; } = default!;

        /// <summary>
        /// 요양 기관 번호
        /// </summary>
        public string HospNo { get; init; } = default!;

        /// <summary>
        /// 병원 명
        /// </summary>
        public string HospName { get; init; } = default!;

        /// <summary>
        /// 사업자 등록 번호
        /// </summary>
        public string? BusinessNo { get; init; }

        /// <summary>
        /// 차트 타입 E: 이지스 차트, N: 닉스 차트
        /// </summary>
        public string ChartType { get; init; } = default!;

        /// <summary>
        /// 사업자 구분 CT01: 법인 사업자, CT02: 개인 사업자
        /// </summary>
        public string? BusinessLevel { get; init; }

        /// <summary>
        /// 병원 주소
        /// </summary>
        public string HospAddr { get; init; } = default!;

        /// <summary>
        /// 병원 우편 번호
        /// </summary>
        public string HospPostCd { get; init; } = default!;

        /// <summary>
        /// 병원 전화 번호
        /// </summary>
        public string HospTel { get; init; } = default!;
    }
}
