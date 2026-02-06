namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    /// <summary>
    /// 공지사항정보
    /// </summary>
    public class TbNoticeEntity
    {
        /// <summary>
        /// 공지아이디
        /// </summary>
        public int NotiId { get; set; }
        /// <summary>
        /// 제목
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 본문
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// tb_common:cls_cd 15(00:이지스공지,01:병원공지)
        /// </summary>
        public string Grade { get; set; }
        /// <summary>
        /// 요양기관키
        /// </summary>
        public string HospKey { get; set; }
        /// <summary>
        /// A:안드로이드,I:아이폰,0:전체
        /// </summary>
        public string SendType { get; set; }
        /// <summary>
        /// 노출유무
        /// </summary>
        public string ShowYn { get; set; }
        /// <summary>
        /// 삭제유무
        /// </summary>
        public string DelYn { get; set; }
        /// <summary>
        /// 수정날짜
        /// </summary>
        public int? ModDt { get; set; }
        /// <summary>
        /// 등록날짜
        /// </summary>
        public int RegDt { get; set; }
    }
}
