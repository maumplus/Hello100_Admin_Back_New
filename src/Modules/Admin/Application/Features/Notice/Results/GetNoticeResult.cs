namespace Hello100Admin.Modules.Admin.Application.Features.Notice.Results
{
    public sealed class GetNoticeResult
    {
        /// <summary>
        /// 공지 ID
        /// </summary>
        public int NotiId { get; set; }
        /// <summary>
        /// 제목
        /// </summary>
        public string Title { get; set; } = default!;
        /// <summary>
        /// 내용
        /// </summary>
        public string Content { get; set; } = default!;
        /// <summary>
        /// 송신 타입 [A: 안드로이드, I: 아이폰, 0: 전체]
        /// </summary>
        public string SendType { get; set; } = default!;
        /// <summary>
        /// 노출여부
        /// </summary>
        public string ShowYn { get; set; } = default!;
    }
}
