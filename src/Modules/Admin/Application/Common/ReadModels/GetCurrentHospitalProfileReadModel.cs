namespace Hello100Admin.Modules.Admin.Application.Common.ReadModels
{
    public class GetCurrentHospitalProfileReadModel
    {
        /// <summary>
        /// 요양기관 키
        /// </summary>
        public string HospKey { get; set; } = default!;

        /// <summary>
        /// 요양기관 번호
        /// </summary>
        public string HospNo { get; set; } = default!;

        /// <summary>
        /// 병원명
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// 병원분류코드
        /// </summary>
        public string HospClsCd { get; set; } = default!;

        /// <summary>
        /// 주소
        /// </summary>
        public string Addr { get; set; } = default!;

        /// <summary>
        /// 우편번호
        /// </summary>
        public string PostCd { get; set; } = default!;

        /// <summary>
        /// 전화번호
        /// </summary>
        public string Tel { get; set; } = default!;

        /// <summary>
        /// 마감여부
        /// </summary>
        public char ClosingYn { get; set; }

        /// <summary>
        /// 삭제여부
        /// </summary>
        public char DelYn { get; set; }

        /// <summary>
        /// 설명
        /// </summary>
        public string Descrption { get; set; } = default!;

        /// <summary>
        /// 의사코드
        /// </summary>
        public string MdCd { get; set; } = default!;

        /// <summary>
        /// 키오스크 수
        /// </summary>
        public int KioskCnt { get; set; }

        /// <summary>
        /// 태블릿 수
        /// </summary>
        public int TabletCnt { get; set; }

        /// <summary>
        /// 차트유형
        /// </summary>
        public string ChartType { get; set; } = default!;
    }
}
