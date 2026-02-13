namespace Hello100Admin.API.Constracts.Admin.HospitalUser
{
    public sealed record SearchHospitalUsersRequest
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
        /// 조회 시작일
        /// </summary>
        public string? FromDate { get; init; }

        /// <summary>
        /// 조회 종료일
        /// </summary>
        public string? ToDate { get; init; }

        /// <summary>
        /// 검색 키워드 조회 타입 [Name: 1, Email: 2, Phone: 3]
        /// </summary>
        public required int KeywordSearchType { get; init; }

        /// <summary>
        /// 검색 키워드
        /// </summary>
        public string? SearchKeyword { get; init; }
    }
}
