namespace Hello100Admin.Modules.Admin.Application.Features.Keywords.Results
{
    public class GetKeywordsResult
    {
        /// <summary>
        /// 키워드 ID
        /// </summary>
        public string KId { get; set; } = default!;
        /// <summary>
        /// 키워드
        /// </summary>
        public string Keyword { get; set; } = default!;
        /// <summary>
        /// 삭제여부
        /// </summary>
        public string DelYn { get; set; } = default!;
        /// <summary>
        /// 마스터시퀀스
        /// </summary>
        public int MasterSeq { get; set; }
        /// <summary>
        /// 상세시퀀스
        /// </summary>
        public int DetailSeq { get; set; }
    }
}
