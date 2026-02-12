namespace Hello100Admin.API.Constracts.Admin.Hospitals
{
    public sealed record GetHospitalsRequest
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
        /// 검색 타입 [0: 병원명, 1: 대표번호]
        /// </summary>
        public required int SearchType { get; init; }

        /// <summary>
        /// 검색 키워드
        /// </summary>
        public string? SearchKeyword { get; init; }
    }
}
