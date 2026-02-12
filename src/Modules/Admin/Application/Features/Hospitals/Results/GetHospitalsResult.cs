namespace Hello100Admin.Modules.Admin.Application.Features.Hospitals.Results
{
    public sealed class SearchHospitalsResult
    {
        /// <summary>
        /// 행번호
        /// </summary>
        public int RowNum { get; set; }
        /// <summary>
        /// 요양기관 키
        /// </summary>
        public string HospKey { get; set; } = default!;
        /// <summary>
        /// 병원명
        /// </summary>
        public string Name { get; set; } = default!;
        /// <summary>
        /// 주소
        /// </summary>
        public string Addr { get; set; } = default!;
        /// <summary>
        /// 대표번호
        /// </summary>
        public string Tel { get; set; } = default!;
    }
}
