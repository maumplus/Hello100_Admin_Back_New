using Hello100Admin.Modules.Admin.Application.Common.Models;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Results
{
    public class GetHospitalServiceUsageStatusResult
    {
        /// <summary>
        /// 서비스 단위 접수현황
        /// </summary>
        public List<GetHospitalServiceUsageStatusResultItemByServiceUnit> StatusByServiceUnit { get; set; } = default!;
        /// <summary>
        /// 병원 단위 접수현황
        /// </summary>
        public ListResult<GetHospitalServiceUsageStatusResultItemByHospitalUnit> StatusByHospitalUnit { get; set; } = default!;
    }

    public class GetHospitalServiceUsageStatusResultItemByServiceUnit
    {
        /// <summary>
        /// 진료유형코드
        /// </summary>
        public string ReceptType { get; set; } = default!;
        /// <summary>
        /// 진료유형명
        /// </summary>
        public string ReceptTypeNm { get; set; } = default!;
        /// <summary>
        /// 접수대기
        /// </summary>
        public int WaitingCount { get; set; }
        /// <summary>
        /// 접수완료
        /// </summary>
        public int ReceptionCount { get; set; }
        /// <summary>
        /// 실패
        /// </summary>
        public int ReceptionFailedCount { get; set; }
        /// <summary>
        /// 취소
        /// </summary>
        public int ReceptionCanceledCount { get; set; }
        /// <summary>
        /// 진료완료
        /// </summary>
        public int TreatmentCompletedCount { get; set; }
        /// <summary>
        /// 총계
        /// </summary>
        public int TotalReceptionCount { get; set; }
    }

    public class GetHospitalServiceUsageStatusResultItemByHospitalUnit
    {
        /// <summary>
        /// 행번호
        /// </summary>
        public int RowNum { get; set; }
        /// <summary>
        /// 요양기관명
        /// </summary>
        public string HospName { get; set; } = default!;
        /// <summary>
        /// 요양기관번호
        /// </summary>
        public string HospNo { get; set; } = default!;
        /// <summary>
        /// 접수대기
        /// </summary>
        public int WaitingCount { get; set; }
        /// <summary>
        /// 접수완료
        /// </summary>
        public int ReceptionCount { get; set; }
        /// <summary>
        /// 실패
        /// </summary>
        public int ReceptionFailedCount { get; set; }
        /// <summary>
        /// 취소
        /// </summary>
        public int ReceptionCanceledCount { get; set; }
        /// <summary>
        /// 진료완료
        /// </summary>
        public int TreatmentCompletedCount { get; set; }
    }
}
