namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public sealed record UpdateSymptomExamKeywordRequest
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
        /// 상세 키워드 리스트
        /// </summary>
        public required List<UpdateSymptomExamKeywordRequestItem> DetailKeywordItems { get; init; }
    }

    public sealed record UpdateSymptomExamKeywordRequestItem
    {
        /// <summary>
        /// 상세 키워드 고유번호
        /// </summary>
        public required int DetailSeq { get; init; }
        /// <summary>
        /// 상세 키워드명
        /// </summary>
        public required string DetailName { get; init; }
    }
}
