namespace Hello100Admin.Modules.Admin.Application.Features.Advertisement.Results
{
    public sealed class GetPopupsResult
    {
        /// <summary>
        /// 행번호
        /// </summary>
        public int RowNum { get; set; }
        /// <summary>
        /// 광고ID
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
        /// 송신 타입 [A: 안드로이드, I: 아이폰, 0: 전체]
        /// </summary>
        public string SendType { get; set; } = default!;
        /// <summary>
        /// 링크구분 [O: 외부, I: 내부, M: 메뉴이동]
        /// </summary>
        public string? LinkType { get; set; }
        /// <summary>
        /// 외부링크경로
        /// </summary>
        public string? Url { get; set; }
        /// <summary>
        /// 정렬순서(필요시만사용)
        /// </summary>
        public int SortNo { get; set; }
        /// <summary>
        /// 삭제여부
        /// </summary>
        public string DelYn { get; set; } = default!;
        /// <summary>
        /// 이미지 URL
        /// </summary>
        public string? ImgUrl { get; set; }
        /// <summary>
        /// 기간설정시작일자 (yyyy-mm-dd)
        /// </summary>
        public string? StartDt { get; set; }
        /// <summary>
        /// 기간설정만료날짜 (yyyy-mm-dd)
        /// </summary>
        public string? EndDt { get; set; }
        /// <summary>
        /// 등록일시 (yyyy-mm-dd hh24:mi)
        /// </summary>
        public string RegDt { get; set; } = default!;
        /// <summary>
        /// 기간설정만료날짜 기준 경과일
        /// </summary>
        public int? CntDt { get; set; }
    }
}
