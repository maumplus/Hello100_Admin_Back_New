namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public sealed record GetHospitalsUsingHello100ServiceRequest
    {
        /// <summary>
        /// 페이지 번호
        /// </summary>
        public required int PageNo { get; init; }
        /// <summary>
        /// 페이지 사이즈
        /// </summary>
        public required int PageSize { get; init; }
        /// <summary>
        /// 검색차트타입 ["": 전체, E: 이지스전자차트, N: 닉스펜차트]
        /// </summary>
        public string? SearchChartType { get; init; }
        /// <summary>
        /// 검색 타입 [병원명: 1, 요양기관번호: 2]
        /// </summary>
        public required int SearchType { get; init; }
        /// <summary>
        /// 검색 키워드
        /// </summary>
        public string? SearchKeyword { get; init; }
    }
}
