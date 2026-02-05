namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Results
{
    public sealed class GetQuestionnairesResult
    {
        /// <summary>
        /// 문진표 코드
        /// </summary>
        public int IntCd { get; set; }
        /// <summary>
        /// 문진표 이름
        /// </summary>
        public string IntNm { get; set; } = default!;
        /// <summary>
        /// 카테고리
        /// </summary>
        public string Category { get; set; } = default!;
    }
}
