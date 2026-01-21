namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.ReadModels.ExportExaminationResultAlimtalkHistoriesExcel
{
    public class GetExaminationResultAlimtalkHistoryForExportReadModel
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
