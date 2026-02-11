namespace Hello100Admin.API.Constracts.Admin.Notice
{
    public class CreateNoticeRequest
    {
        /// <summary>
        /// 제목
        /// </summary>
        public required string Title { get; set; }
        /// <summary>
        /// 내용
        /// </summary>
        public required string Content { get; set; }
        /// <summary>
        /// 송신 타입 [A: 안드로이드, I: 아이폰, 0: 전체]
        /// </summary>
        public required string SendType { get; set; }
        /// <summary>
        /// 노출여부
        /// </summary>
        public required string ShowYn { get; set; }
    }
}
