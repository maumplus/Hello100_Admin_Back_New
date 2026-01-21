namespace Hello100Admin.Modules.Admin.Infrastructure.Persistence.DbModels.Common
{
    internal sealed record CurrentHospitalInfo
    {
        /// <summary>
        /// 요양기관 키
        /// </summary>
        public string HospKey { get; init; } = default!;

        /// <summary>
        /// 요양기관 번호
        /// </summary>
        public string HospNo { get; init; } = default!;

        /// <summary>
        /// 병원명
        /// </summary>
        public string Name { get; init; } = default!;

        /// <summary>
        /// 병원분류코드
        /// </summary>
        public string HospClsCd { get; init; } = default!;

        /// <summary>
        /// 주소
        /// </summary>
        public string Addr { get; init; } = default!;

        /// <summary>
        /// 우편번호
        /// </summary>
        public string PostCd { get; init; } = default!;

        /// <summary>
        /// 전화번호
        /// </summary>
        public string Tel { get; init; } = default!;

        /// <summary>
        /// 마감여부
        /// </summary>
        public char ClosingYn { get; init; }

        /// <summary>
        /// 삭제여부
        /// </summary>
        public char DelYn { get; init; }

        /// <summary>
        /// 설명
        /// </summary>
        public string Descrption { get; init; } = default!;

        /// <summary>
        /// 의사코드
        /// </summary>
        public string MdCd { get; init; } = default!;

        /// <summary>
        /// 키오스크 수
        /// </summary>
        public int KioskCnt { get; init; }

        /// <summary>
        /// 태블릿 수
        /// </summary>
        public int TabletCnt { get; init; }

        /// <summary>
        /// 차트유형
        /// </summary>
        public string ChartType { get; init; } = default!;
    }
}
