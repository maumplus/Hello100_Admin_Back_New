namespace Hello100Admin.API.Constracts.Admin.HospitalUser
{
    public sealed record SearchHospitalUsersRequest
    {
        /// <summary>
        /// 페이지 번호
        /// </summary>
        public int PageNo { get; set; }

        /// <summary>
        /// 페이지 사이즈
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 조회 시작일
        /// </summary>
        public string? FromDate { get; set; }

        /// <summary>
        /// 조회 종료일
        /// </summary>
        public string? ToDate { get; set; }

        /// <summary>
        /// 검색 키워드 조회 타입 [Name: 1, Email: 2, Phone: 3]
        /// </summary>
        public required int KeywordSearchType { get; set; }

        /// <summary>
        /// 검색 키워드
        /// </summary>
        public string? SearchKeyword { get; set; }
    }
}
