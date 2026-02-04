namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    public class TbAdInfoEntity
    {
        /// <summary>
        /// 광고아이디
        /// </summary>
        public int AdId { get; set; }

        /// <summary>
        /// 광고유형 tb_common(cls_cd:11)
        /// </summary>
        public string AdType { get; set; } = default!;

        /// <summary>
        /// 요양기관키(병원광고시만)
        /// </summary>
        public string? HospKey { get; set; }

        /// <summary>
        /// 진료과(병원광고시만)
        /// </summary>
        public string? MdCd { get; set; }

        /// <summary>
        /// 노출여부
        /// </summary>
        public string ShowYn { get; set; } = default!;

        /// <summary>
        /// A:안드로이드,I:아이폰,0:전체
        /// </summary>
        public string SendType { get; set; } = default!;

        /// <summary>
        /// 링크구분(O:외부, I:내부, M:메뉴이동)
        /// </summary>
        public string? LinkType { get; set; }

        /// <summary>
        /// 외부링크경로
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// 기간설정시작날짜(yyyyMMdd)
        /// </summary>
        public string? StartDt { get; set; }

        /// <summary>
        /// 기간설정만료날짜(yyyyMMdd)
        /// </summary>
        public string? EndDt { get; set; }

        /// <summary>
        /// 정렬순서(필요시만사용)
        /// </summary>
        public int SortNo { get; set; }

        /// <summary>
        /// 삭제유무
        /// </summary>
        public string DelYn { get; set; } = default!;

        /// <summary>
        /// 등록날짜
        /// </summary>
        public int RegDt { get; set; }

        /// <summary>
        /// 서브url 쇼핑몰 사용
        /// </summary>
        public string? Url2 { get; set; }

        ///// <summary>
        ///// 이미지 경로
        ///// 굳이 이미지 경로를 tb_image_info에 저장해야하는 이유는??
        ///// </summary>
        //public string? ImgPath { get; set; }
    }
}
