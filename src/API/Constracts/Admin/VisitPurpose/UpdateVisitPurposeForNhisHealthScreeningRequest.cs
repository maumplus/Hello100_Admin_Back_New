namespace Hello100Admin.API.Constracts.Admin.VisitPurpose
{
    public sealed record UpdateVisitPurposeForNhisHealthScreeningRequest
    {
        /// <summary>
        /// 요양기관키
        /// </summary>
        public required string HospKey { get; init; }
        /// <summary>
        /// 노출 설정
        /// </summary>
        public required string ShowYn { get; init; }
        /// <summary>
        /// 접수 구분 설정 (1: QR/당일접수, 4: 예약)
        /// </summary>
        public List<int>? Roles { get; init; }
        /// <summary>
        /// 상세 항목 리스트 중 노출 체크 상태인 검진 코드(VpCd) 목록
        /// DetailShowYn -> DetailVpCodesToShow
        /// </summary>
        public List<string>? DetailShowYn { get; init; }
    }

    public sealed record UpdateMyVisitPurposeForNhisHealthScreeningRequest
    {
        /// <summary>
        /// 노출 설정
        /// </summary>
        public required string ShowYn { get; init; }

        /// <summary>
        /// 접수 구분 설정 (1: QR/당일접수, 4: 예약)
        /// </summary>
        public List<int>? Roles { get; init; }

        /// <summary>
        /// 상세 항목 리스트 중 노출 체크 상태인 검진 코드(VpCd) 목록
        /// DetailShowYn -> DetailVpCodesToShow
        /// </summary>
        public List<string>? DetailShowYn { get; init; }
    }
}
