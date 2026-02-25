namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results
{
    public class GetSymptomExamKeywordDetailResult
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
        /// 상세 키워드 사용 여부
        /// </summary>
        public string DetailUseYn { get; set; } = default!;
        /// <summary>
        /// 상세 키워드 리스트
        /// </summary>
        public List<GetSymptomExamKeywordDetailResultItem> DetailKeywordItems { get; set; } = default!;
    }

    public class GetSymptomExamKeywordDetailResultItem
    {
        /// <summary>
        /// 상세 키워드 고유번호
        /// </summary>
        public int DetailSeq { get; set; }
        /// <summary>
        /// 상세 키워드명
        /// </summary>
        public string DetailName { get; set; } = default!;
        /// <summary>
        /// 클릭 수
        /// </summary>
        public int SearchCnt { get; set; } = default!;
    }
}
