namespace Hello100Admin.API.Constracts.Admin.RequestsManagement
{
    public sealed record GetRequestBugsRequest
    {
        /// <summary>
        /// 페이지 번호
        /// </summary>
        public required int PageNo { get; set; }

        /// <summary>
        /// 페이지 사이즈
        /// </summary>
        public required int PageSize { get; set; }

        /// <summary>
        /// 조회 시작일
        /// </summary>
        public bool ApprYn { get; set; }
    }
}
