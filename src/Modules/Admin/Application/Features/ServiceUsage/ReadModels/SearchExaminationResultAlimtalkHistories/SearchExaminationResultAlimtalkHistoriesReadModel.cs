namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.ReadModels.SearchExaminationResultAlimtalkHistories
{
    public sealed class SearchExaminationResultAlimtalkHistoriesReadModel
    {
        /// <summary>
        /// 검사인원
        /// </summary>
        public int TotalPtntCount { get; set; }
        /// <summary>
        /// 전체 발송건수
        /// </summary>
        public int TotalSendCount { get; set; }
        /// <summary>
        /// 발송성공건수
        /// </summary>
        public int SendSuccessCount { get; set; }
        /// <summary>
        /// 발송실패건수
        /// </summary>
        public int SendFailCount { get; set; }
        /// <summary>
        /// App push 발송건수
        /// </summary>
        public int PushCount { get; set; }
        /// <summary>
        /// 알림톡 발송건수
        /// </summary>
        public int KakaoCount { get; set; }
        public List<SearchExaminationResultAlimtalkHistoryReadModel> List { get; set; } = default!;
    }

    public sealed class SearchExaminationResultAlimtalkHistoryReadModel
    {
        /// <summary>
        /// 순번
        /// </summary>
        public int RowNum { get; set; }
        /// <summary>
        /// 환자명
        /// </summary>
        public string PtntName { get; set; }
        /// <summary>
        /// 환자성별
        /// </summary>
        public string PtntSex { get; set; }
        /// <summary>
        /// 접수번호
        /// </summary>
        public string ReceptNo { get; set; }
        /// <summary>
        /// 진단검사일자(진료일자)
        /// </summary>
        public string ReqDate { get; set; }
        /// <summary>
        /// 결과발송일자
        /// </summary>
        public string SendDate { get; set; }
        /// <summary>
        /// 발송상태
        /// </summary>
        public string SendStatus { get; set; }
        /// <summary>
        /// 실패메세지
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 발송방식
        /// </summary>
        public string SendType { get; set; }
        /// <summary>
        /// 발송키
        /// </summary>
        public string NotificationId { get; set; }
    }
}
