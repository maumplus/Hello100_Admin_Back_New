namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Responses.SearchExaminationResultAlimtalkHistories
{
    public sealed record SearchExaminationResultAlimtalkHistoriesResponse
    {
        /// <summary>
        /// 검사인원
        /// </summary>
        public int TotalPtntCount { get; init; }
        /// <summary>
        /// 전체 발송건수
        /// </summary>
        public int TotalSendCount { get; init; }
        /// <summary>
        /// 발송성공건수
        /// </summary>
        public int SendSuccessCount { get; init; }
        /// <summary>
        /// 발송실패건수
        /// </summary>
        public int SendFailCount { get; init; }
        /// <summary>
        /// App push 발송건수
        /// </summary>
        public int PushCount { get; init; }
        /// <summary>
        /// 알림톡 발송건수
        /// </summary>
        public int KakaoCount { get; init; }
        public List<SearchDiagnosticTestResultAlimtalkSendHistoryItem> List { get; init; } = default!;
    }

    public sealed record SearchDiagnosticTestResultAlimtalkSendHistoryItem
    {
        /// <summary>
        /// 순번
        /// </summary>
        public int RowNum { get; init; }
        /// <summary>
        /// 환자명
        /// </summary>
        public string PtntName { get; init; }
        /// <summary>
        /// 환자성별
        /// </summary>
        public string PtntSex { get; init; }
        /// <summary>
        /// 접수번호
        /// </summary>
        public string ReceptNo { get; init; }
        /// <summary>
        /// 진단검사일자(진료일자)
        /// </summary>
        public string ReqDate { get; init; }
        /// <summary>
        /// 결과발송일자
        /// </summary>
        public string SendDate { get; init; }
        /// <summary>
        /// 발송상태
        /// </summary>
        public string SendStatus { get; init; }
        /// <summary>
        /// 실패메세지
        /// </summary>
        public string Message { get; init; }
        /// <summary>
        /// 발송방식
        /// </summary>
        public string SendType { get; init; }
        /// <summary>
        /// 발송키
        /// </summary>
        public string NotificationId { get; init; }
    }
}
