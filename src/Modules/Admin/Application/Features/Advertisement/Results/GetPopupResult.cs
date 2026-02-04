namespace Hello100Admin.Modules.Admin.Application.Features.Advertisement.Results
{
    public sealed class GetPopupResult
    {
        /// <summary>
        /// 이미지ID
        /// </summary>
        public int ImgId { get; set; }
        /// <summary>
        /// 이미지명
        /// </summary>
        public string? ImgName { get; set; }
        /// <summary>
        /// 광고ID
        /// </summary>
        public int AdId { get; set; }
        /// <summary>
        /// 노출여부
        /// </summary>
        public string ShowYn { get; set; } = default!;
        /// <summary>
        /// 송신 타입 [A: 안드로이드, I: 아이폰, 0: 전체]
        /// </summary>
        public string SendType { get; set; } = default!;
        /// <summary>
        /// 링크구분 [O: 외부, I: 내부, M: 메뉴이동, N: 링크없음]
        /// </summary>
        public string LinkType { get; set; } = default!;
        /// <summary>
        /// 외부링크경로
        /// </summary>
        public string? Url { get; set; }
        /// <summary>
        /// 이미지 URL
        /// </summary>
        public string? ImgUrl { get; set; }
        /// <summary>
        /// 시작일자
        /// </summary>
        public string? StartDt { get; set; }
        /// <summary>
        /// 종료일자
        /// </summary>
        public string? EndDt { get; set; }
        /// <summary>
        /// 정렬순서
        /// </summary>
        public int SortNo { get; set; }
    }
}
