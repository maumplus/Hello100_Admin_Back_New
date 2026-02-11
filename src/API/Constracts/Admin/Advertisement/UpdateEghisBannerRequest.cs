namespace Hello100Admin.API.Constracts.Admin.Advertisement
{
    public sealed record UpdateEghisBannerRequest
    {
        /// <summary>
        /// 이미지 ID
        /// </summary>
        public int ImgId { get; init; }
        /// <summary>
        /// 노출여부
        /// </summary>
        public required string ShowYn { get; init; } = default!;
        /// <summary>
        /// 송신 타입 [A: 안드로이드, I: 아이폰, 0: 전체]
        /// </summary>
        public required string SendType { get; init; } = default!;
        /// <summary>
        /// 링크구분 [O: 외부, I: 내부, M: 메뉴이동, N: 링크없음]
        /// </summary>
        public required string LinkType { get; init; } = default!;
        /// <summary>
        /// 링크경로
        /// </summary>
        public string? Url { get; init; }
        /// <summary>
        /// 링크경로
        /// </summary>
        public string? Url2 { get; init; }
        /// <summary>
        /// 기간설정시작일자 (yyyy-mm-dd)
        /// </summary>
        public string? StartDt { get; init; }
        /// <summary>
        /// 기간설정만료일자 (yyyy-mm-dd)
        /// </summary>
        public string? EndDt { get; init; }
        /// <summary>
        /// 정렬 순서
        /// </summary>
        public required int SortNo { get; init; }
        /// <summary>
        /// 이미지 파일 (이지스 배너 이미지)
        /// </summary>
        public IFormFile? Image { get; init; }
    }
}
