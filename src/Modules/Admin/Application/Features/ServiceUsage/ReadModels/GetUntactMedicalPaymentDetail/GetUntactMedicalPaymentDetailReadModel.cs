namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.ReadModels.GetUntactMedicalPaymentDetail
{
    public sealed class GetUntactMedicalPaymentDetailReadModel
    {
        /// <summary>
        /// NHN KCP 결제 ID
        /// </summary>
        public string PaymentId { get; set; } = default!;

        /// <summary>
        /// 요양기관 번호
        /// </summary>
        public string HospNo { get; set; } = default!;

        /// <summary>
        /// 접수 번호
        /// </summary>
        public string ReceptNo { get; set; } = default!;

        /// <summary>
        /// 상점 관리 주문번호
        /// </summary>
        public string OrderIdxx { get; set; } = default!;

        /// <summary>
        /// 처리 상태
        /// </summary>
        public string ProcessStatus { get; set; } = default!;

        /// <summary>
        /// 처리 상태 명
        /// </summary>
        public string? ProcessStatusNm { get; set; }

        /// <summary>
        /// 실패메세지
        /// </summary>
        public string? FailedMsg { get; set; }

        /// <summary>
        /// 결과 코드
        /// </summary>
        public string? ResCd { get; set; }

        /// <summary>
        /// 결과 메시지
        /// </summary>
        public string? ResMsg { get; set; }

        /// <summary>
        /// 결제 건의 카드사 코드
        /// </summary>
        public string? CardCd { get; set; }

        /// <summary>
        /// 결제 건의 카드사명
        /// </summary>
        public string? CardName { get; set; }

        /// <summary>
        /// 결제 건의 카드번호(마스킹)
        /// </summary>
        public string? CardNo { get; set; }

        /// <summary>
        /// 결제 시각
        /// </summary>
        public string? AppTime { get; set; }

        /// <summary>
        /// 결제 건의 매입사명
        /// </summary>
        public string? AcQuname { get; set; }
    }
}
