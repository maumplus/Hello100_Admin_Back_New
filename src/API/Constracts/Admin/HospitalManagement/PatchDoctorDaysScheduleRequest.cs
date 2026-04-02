namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public sealed record PatchDoctorDaysScheduleInfo
    {
        /// <summary>
        /// 진료일[&apos;&apos;: 기본템플릿 사용 ]
        /// </summary>
        public required string ClinicYmd { get; init; }
        /// <summary>
        /// 요일순번
        /// </summary>
        public required int WeekNum { get; init; }
        /// <summary>
        /// 진료시작시간
        /// </summary>
        public required int StartHour { get; init; }
        /// <summary>
        /// 진료시작분
        /// </summary>
        public required int StartMinute { get; init; }
        /// <summary>
        /// 진료종료시간
        /// </summary>
        public required int EndHour { get; init; }
        /// <summary>
        /// 진료종료분
        /// </summary>
        public required int EndMinute { get; init; }
        /// <summary>
        /// 점심시작시간
        /// </summary>
        public required int BreakStartHour { get; init; }
        /// <summary>
        /// 점심시작분
        /// </summary>
        public required int BreakStartMinute { get; init; }
        /// <summary>
        /// 점심종료시간
        /// </summary>
        public required int BreakEndHour { get; init; }
        /// <summary>
        /// 점심종료분
        /// </summary>
        public required int BreakEndMinute { get; init; }
        /// <summary>
        /// 환자 진료 시간
        /// </summary>
        public required int IntervalTime { get; init; }
        /// <summary>
        /// 부가서비스[1: qr접수 , 2:당일접수, 4:예약, 8:qr 접수마감, 16:당일접수마감]
        /// </summary>
        public required int Hello100Role { get; init; }
        /// <summary>
        /// 사용유무
        /// </summary>
        public required string UseYn { get; init; }
        /// <summary>
        /// 예약번호
        /// </summary>
        public required int Ridx { get; init; }
    }

    public sealed record PatchDoctorDaysScheduleRequest
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
        /// 지정 스케줄 목록
        /// </summary>
        public required List<PatchDoctorDaysScheduleInfo> DoctorScheduleList { get; init; }
    }

    public sealed record PatchMyDoctorDaysScheduleRequest
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
        /// 지정 스케줄 목록
        /// </summary>
        public required List<PatchDoctorDaysScheduleInfo> DoctorScheduleList { get; init; }
    }
}
