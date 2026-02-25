namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results
{
    public sealed class GetSymptomExamKeywordsResult
    {
        /// <summary>
        /// 대표 키워드 고유번호
        /// </summary>
        public int MasterSeq { get; set; }
        /// <summary>
        /// 대표 키워드명
        /// </summary>
        public string MasterName { get; set; } = default!;
        /// <summary>
        /// 노출 여부
        /// </summary>
        public string ShowYn { get; set; } = default!;
        /// <summary>
        /// 정렬 순서
        /// </summary>
        public int SortNo { get; set; }
        /// <summary>
        /// 클릭 수
        /// </summary>
        public int SearchCnt { get; set; }
        /// <summary>
        /// 하위 키워드 수
        /// </summary>
        public int DetailCnt { get; set; }
        /// <summary>
        /// 등록 일시 (yyyy-MM-dd HH:mm)
        /// </summary>
        public string RegDt { get; set; } = default!;
    }
}
