namespace Hello100Admin.Modules.Admin.Infrastructure.Persistence.DbModels.ServiceUsage
{
    internal sealed record GetExaminationResultAlimtalkHistoryForExportRow
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
