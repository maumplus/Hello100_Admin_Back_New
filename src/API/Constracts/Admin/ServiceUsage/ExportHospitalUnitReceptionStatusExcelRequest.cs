namespace Hello100Admin.API.Constracts.Admin.ServiceUsage
{
    public sealed record ExportHospitalUnitReceptionStatusExcelRequest
    {
        /// <summary>
        /// 조회 시작일
        /// </summary>
        public string? FromDate { get; init; }
        /// <summary>
        /// 조회 종료일
        /// </summary>
        public string? ToDate { get; init; }
        /// <summary>
        /// 검색 차트타입 [전체: "", 이지스: "E", 닉스: "N"]
        /// </summary>
        public string? SearchChartType { get; init; }
        /// <summary>
        /// 검색 타입 [1: 병원명, 2: 요양기관번호]
        /// </summary>
        public required int SearchType { get; init; }
        /// <summary>
        /// 검색 키워드
        /// </summary>
        public string? SearchKeyword { get; init; }
        /// <summary>
        /// QR 접수 체크 여부
        /// </summary>
        public required string QrCheckInYn { get; init; }
        /// <summary>
        /// 오늘 접수 체크 여부
        /// </summary>
        public required string TodayRegistrationYn { get; init; }
        /// <summary>
        /// 진료 예약 체크 여부
        /// </summary>
        public required string AppointmentYn { get; init; }
        /// <summary>
        /// 비대면 진료 체크 여부
        /// </summary>
        public required string TelemedicineYn { get; init; }
        /// <summary>
        /// 테스트병원 제외 여부
        /// </summary>
        public required string ExcludeTestHospitalsYn { get; init; }
    }
}
