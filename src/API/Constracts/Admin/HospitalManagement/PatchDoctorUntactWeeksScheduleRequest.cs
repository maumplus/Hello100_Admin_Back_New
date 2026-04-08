namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public sealed record PatchDoctorUntactWeeksScheduleInfo
    {
        /// <summary>
        /// 요일순번
        /// </summary>
        public int WeekNum { get; init; }
        /// <summary>
        /// 예약번호
        /// </summary>
        public required int Ridx { get; init; }
        /// <summary>
        /// 비대면진료시작(시)
        /// </summary>
        public required int UntactStartHour { get; init; }
        /// <summary>
        /// 비대면진료시작(분)
        /// </summary>
        public required int UntactStartMinute { get; init; }
        /// <summary>
        /// 비대면진료종료(시)
        /// </summary>
        public required int UntactEndHour { get; init; }
        /// <summary>
        /// 비대면진료종료(분)
        /// </summary>
        public required int UntactEndMinute { get; init; }
        /// <summary>
        /// 비대면점심시작시간(시)
        /// </summary>
        public required int UntactBreakStartHour { get; init; }
        /// <summary>
        /// 비대면점심시작시간(분)
        /// </summary>
        public required int UntactBreakStartMinute { get; init; }
        /// <summary>
        /// 비대면점심종료시간(시)
        /// </summary>
        public required int UntactBreakEndHour { get; init; }
        /// <summary>
        /// 비대면점심종료시간(분)
        /// </summary>
        public required int UntactBreakEndMinute { get; init; }
        /// <summary>
        /// 비대면진료 사용여부
        /// </summary>
        public required string UntactUseYn { get; init; }
    }

    public sealed record PatchDoctorUntactWeeksScheduleRequest
    {
        /// <summary>
        /// 요양기관번호
        /// </summary>
        public required string HospNo { get; init; }
        /// <summary>
        /// 요양기관키
        /// </summary>
        public required string HospKey { get; init; }
        /// <summary>
        /// 의사사번
        /// </summary>
        public required string EmplNo { get; init; }
        /// <summary>
        /// 의사 면허번호
        /// </summary>
        public required string DoctNo { get; init; }
        /// <summary>
        /// 의사명
        /// </summary>
        public required string DoctNm { get; init; }
        /// <summary>
        /// 진료과코드
        /// </summary>
        public string? DeptCd { get; init; }
        /// <summary>
        /// 진료과명
        /// </summary>
        public string? DeptNm { get; init; }
        /// <summary>
        /// 화면 대기인원 표시[0:사용안함, 1:인원수, 2:시간, 3: 인원수, 시간 모두표시]
        /// </summary>
        public required int ViewRole { get; init; }
        /// <summary>
        /// 대기 시간표시에 따른 최소시간
        /// </summary>
        public required string ViewMinTime { get; init; }
        /// <summary>
        /// 대기 인원표시에 따른 최소인원
        /// </summary>
        public required string ViewMinCnt { get; init; }
        /// <summary>
        /// 비대면 진료 스케줄 목록
        /// </summary>
        public required List<PatchDoctorUntactWeeksScheduleInfo> DoctorScheduleList { get; init; }
    }

    public sealed record PatchMyDoctorUntactWeeksScheduleRequest
    {
        /// <summary>
        /// 의사사번
        /// </summary>
        public required string EmplNo { get; init; }
        /// <summary>
        /// 의사 면허번호
        /// </summary>
        public required string DoctNo { get; init; }
        /// <summary>
        /// 의사명
        /// </summary>
        public required string DoctNm { get; init; }
        /// <summary>
        /// 진료과코드
        /// </summary>
        public string? DeptCd { get; init; }
        /// <summary>
        /// 진료과명
        /// </summary>
        public string? DeptNm { get; init; }
        /// <summary>
        /// 화면 대기인원 표시[0:사용안함, 1:인원수, 2:시간, 3: 인원수, 시간 모두표시]
        /// </summary>
        public required int ViewRole { get; init; }
        /// <summary>
        /// 대기 시간표시에 따른 최소시간
        /// </summary>
        public required string ViewMinTime { get; init; }
        /// <summary>
        /// 대기 인원표시에 따른 최소인원
        /// </summary>
        public required string ViewMinCnt { get; init; }
        /// <summary>
        /// 비대면 진료 스케줄 목록
        /// </summary>
        public required List<PatchDoctorUntactWeeksScheduleInfo> DoctorScheduleList { get; init; }
    }
}
