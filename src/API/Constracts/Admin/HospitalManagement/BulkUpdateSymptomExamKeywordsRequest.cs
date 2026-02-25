namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public sealed record BulkUpdateSymptomExamKeywordsRequest
    {
        /// <summary>
        /// 대표 키워드 고유번호
        /// </summary>
        public required int MasterSeq { get; init; }
        /// <summary>
        /// 정렬 순서
        /// </summary>
        public required int SortNo { get; init; }
        /// <summary>
        /// 노출 여부
        /// </summary>
        public required string ShowYn { get; init; }
    }
}
