namespace Hello100Admin.Modules.Admin.Application.Features.Departments.Results
{
    public class GetDepartmentsResult
    {
        /// <summary>
        /// 공통시퀀스
        /// </summary>
        public int CmSeq { get; set; }
        /// <summary>
        /// 클래스코드
        /// </summary>
        public string ClsCode { get; set; } = default!;
        /// <summary>
        /// 공통코드
        /// </summary>
        public string CmCode { get; set; } = default!;
        /// <summary>
        /// 클래스이름
        /// </summary>
        public string ClsName { get; set; } = default!;
        /// <summary>
        /// 공통이름
        /// </summary>
        public string CmName { get; set; } = default!;
        /// <summary>
        /// 언어코드
        /// </summary>
        public string Locale { get; set; } = default!;
        /// <summary>
        /// 삭제여부
        /// </summary>
        public string DelYn { get; set; } = default!;
        /// <summary>
        /// 정렬순서
        /// </summary>
        public int Sort { get; set; }
    }
}
