namespace Hello100Admin.API.Constracts.Admin.AdminUser
{
    public sealed record GetHospitalAdminListRequest
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
        /// QR코드 생성 상태 [전체: 0, 매칭완료: 1, QR발행: 2, QR미발행: 3, 대리점 배정: 4, 대리점 미배정: 5]
        /// </summary>
        public required int QrState { get; init; }
        /// <summary>
        /// 검색 타입 [병원명: 1, 아이디: 2, 대리점: 3]
        /// </summary>
        public required int SearchType { get; init; }
        /// <summary>
        /// 검색 키워드
        /// </summary>
        public string? SearchKeyword { get; init; }
    }
}
