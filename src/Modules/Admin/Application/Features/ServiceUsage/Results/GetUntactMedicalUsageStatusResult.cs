namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Results
{
    public sealed class GetUntactMedicalUsageStatusResult
    {
        /// <summary>
        /// 진료 인원
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 진료접수(예약) 건수
        /// </summary>
        public int TotalRsrv { get; set; }
        /// <summary>
        /// 진료완료 건수
        /// </summary>
        public int TotalClinicEnd { get; set; }
        /// <summary>
        /// 진료취소 건수
        /// </summary>
        public int TotalClinicCancel { get; set; }
        /// <summary>
        /// 결제완료 건수
        /// </summary>
        public int TotalPaymentSuccess { get; set; }
        /// <summary>
        /// 결제실패 건수
        /// </summary>
        public int TotalPaymentFail { get; set; }
        /// <summary>
        /// 총 결제 금액
        /// </summary>
        public int TotalSumAmt { get; set; }
        /// <summary>
        /// 비대면 진료 현황 목록
        /// </summary>
        public List<GetUntactMedicalUsageStatusResultItem> UsageStatusItems { get; set; } = default!;
    }

    public class GetUntactMedicalUsageStatusResultItem
    {
        /// <summary>
        /// 행 번호
        /// </summary>
        public int RowNum { get; set; }
        /// <summary>
        /// 진료일자 (yyyyMMdd)
        /// </summary>
        public string ReqDate { get; set; } = default!;
        /// <summary>
        /// 진료시간 (HH:mm:ss)
        /// </summary>
        public string ReqTime { get; set; } = default!;
        /// <summary>
        /// 병원명
        /// </summary>
        public string HospName { get; set; } = default!;
        /// <summary>
        /// 의사명
        /// </summary>
        public string DoctorName { get; set; } = default!;
        /// <summary>
        /// 환자명
        /// </summary>
        public string PtntName { get; set; } = default!;
        /// <summary>
        /// 진료과명
        /// </summary>
        public string ReceptType { get; set; } = default!;
        /// <summary>
        /// 접수번호
        /// </summary>
        public string ReceptNo { get; set; } = default!;
        /// <summary>
        /// 결제 상태
        /// </summary>
        public string PaymentStatus { get; set; } = default!;
        /// <summary>
        /// 결제 금액
        /// </summary>
        public string? PaymentAmt { get; set; }
        /// <summary>
        /// 진료 상태
        /// </summary>
        public string PtntState { get; set; } = default!;
        /// <summary>
        /// 주문번호
        /// </summary>
        public string? OrdrIdxx { get; set; }
    }
}
