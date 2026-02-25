namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public sealed record CreateSymptomExamKeywordRequest
    {
        /// <summary>
        /// 대표 키워드명
        /// </summary>
        public required string MasterName { get; init; }
        /// <summary>
        /// 노출 여부
        /// </summary>
        public required string ShowYn { get; init; }
        /// <summary>
        /// 상세 키워드 사용 여부
        /// </summary>
        public required string DetailUseYn { get; init; }
        /// <summary>
        /// 상세 키워드명 리스트
        /// </summary>
        public required List<string> DetailNames { get; init; }
    }
}
