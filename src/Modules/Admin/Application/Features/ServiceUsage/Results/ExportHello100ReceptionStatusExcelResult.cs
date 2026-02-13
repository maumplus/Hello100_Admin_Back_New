namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Results
{
    public class ExportHello100ReceptionStatusExcelResult
    {
        /// <summary>
        /// 조회일 기준 하루 전 헬로100상태 정보 목록
        /// </summary>
        public List<ExportHello100ReceptionStatusExcelResultItem> YesterdayItems { get; set; } = default!;
        /// <summary>
        /// 조회일 기준 헬로100상태 정보 목록
        /// </summary>
        public List<ExportHello100ReceptionStatusExcelResultItem> PeriodItems { get; set; } = default!;
    }

    public class ExportHello100ReceptionStatusExcelResultItem
    {
        // 병원 정보
        public string HospNo { get; set; } = string.Empty; // 요양기관번호
        public string HospName { get; set; } = string.Empty; // 병원명

        // 전체(공통) 상태별 집계
        public int TotalCount { get; set; } // 총계
        public int ReservationWaitingCount { get; set; } // 예약대기 (ptnt_state = 3)
        public int WaitingCount { get; set; } // 대기     (ptnt_state = 2)
        public int ReceptionCount { get; set; } // 접수     (ptnt_state = 1)
        public int CanceledCount { get; set; } // 취소     (ptnt_state = 8)
        public int CompletedCount { get; set; } // 완료     (ptnt_state >= 9)
        public int FailedCount { get; set; } // 실패     (ptnt_state = 7)

        // QR접수 (recept_type = 'RC')
        public int QrReservationWaitingCount { get; set; } // QR접수_예약대기
        public int QrWaitingCount { get; set; } // QR접수_대기
        public int QrReceptionCount { get; set; } // QR접수_접수
        public int QrCanceledCount { get; set; } // QR접수_취소
        public int QrCompletedCount { get; set; } // QR접수_완료

        // 오늘접수 (recept_type = 'TR')
        public int TodayReservationWaitingCount { get; set; } // 오늘접수_예약대기
        public int TodayWaitingCount { get; set; } // 오늘접수_대기
        public int TodayReceptionCount { get; set; } // 오늘접수_접수
        public int TodayCanceledCount { get; set; } // 오늘접수_취소
        public int TodayCompletedCount { get; set; } // 오늘접수_완료

        // 진료예약 (recept_type = 'RS')
        public int ReservationReservationWaitingCount { get; set; } // 진료예약_예약대기
        public int ReservationWaitingCountByType { get; set; } // 진료예약_대기
        public int ReservationReceptionCount { get; set; } // 진료예약_접수
        public int ReservationCanceledCount { get; set; } // 진료예약_취소
        public int ReservationCompletedCount { get; set; } // 진료예약_완료

        // 비대면접수 (recept_type = 'NR')
        public int NonFaceToFaceReservationWaitingCount { get; set; } // 비대면접수_예약대기
        public int NonFaceToFaceWaitingCount { get; set; } // 비대면접수_대기
        public int NonFaceToFaceReceptionCount { get; set; } // 비대면접수_접수
        public int NonFaceToFaceCanceledCount { get; set; } // 비대면접수_취소
        public int NonFaceToFaceCompletedCount { get; set; } // 비대면접수_완료
    }
}
