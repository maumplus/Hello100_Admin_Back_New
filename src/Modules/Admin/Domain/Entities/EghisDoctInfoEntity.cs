namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    public class EghisDoctInfoEntity
    {
        /// <summary>
        /// 요양기관번호
        /// </summary>
        public string HospNo { get; set; } = null!;

        /// <summary>
        /// 요양기관키
        /// </summary>
        public string HospKey { get; set; } = null!;

        /// <summary>
        /// 의사사번
        /// </summary>
        public string EmplNo { get; set; } = null!;

        /// <summary>
        /// 진료일[&apos;&apos;: 기본템플릿 사용 ]
        /// </summary>
        public string ClinicYmd { get; set; } = null!;

        /// <summary>
        /// 의사 면허번호
        /// </summary>
        public string DoctNo { get; set; } = null!;

        /// <summary>
        /// 의사명
        /// </summary>
        public string DoctNm { get; set; } = null!;

        /// <summary>
        /// 진료과코드
        /// </summary>
        public string? DeptCd { get; set; }

        /// <summary>
        /// 진료과명
        /// </summary>
        public string? DeptNm { get; set; }

        /// <summary>
        /// 요일순번
        /// </summary>
        public int WeekNum { get; set; }

        /// <summary>
        /// 진료시작시간
        /// </summary>
        public int StartHour { get; set; }

        /// <summary>
        /// 진료시작분
        /// </summary>
        public int StartMinute { get; set; }

        /// <summary>
        /// 진료종료시간
        /// </summary>
        public int EndHour { get; set; }

        /// <summary>
        /// 진료종료분
        /// </summary>
        public int EndMinute { get; set; }

        /// <summary>
        /// 점심시작시간
        /// </summary>
        public int BreakStartHour { get; set; }

        /// <summary>
        /// 점심시작분
        /// </summary>
        public int BreakStartMinute { get; set; }

        /// <summary>
        /// 점심종료시간
        /// </summary>
        public int BreakEndHour { get; set; }

        /// <summary>
        /// 점심종료분
        /// </summary>
        public int BreakEndMinute { get; set; }

        /// <summary>
        /// 환자 진료 시간
        /// </summary>
        public int IntervalTime { get; set; }

        /// <summary>
        /// 요일별 의사 메세지
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// 부가서비스[1: qr접수 , 2:당일접수, 4:예약, 8:qr 접수마감, 16:당일접수마감]
        /// </summary>
        public int Hello100Role { get; set; }

        /// <summary>
        /// 예약번호
        /// </summary>
        public int Ridx { get; set; }

        /// <summary>
        /// 화면 대기인원 표시[0:사용안함, 1:인원수, 2:시간, 3: 인원수, 시간 모두표시]
        /// </summary>
        public int ViewRole { get; set; }

        /// <summary>
        /// 대기 시간표시에 따른 최소시간
        /// </summary>
        public string ViewMinTime { get; set; } = null!;

        /// <summary>
        /// 대기 인원표시에 따른 최소인원
        /// </summary>
        public string ViewMinCnt { get; set; } = null!;

        /// <summary>
        /// 사용유무
        /// </summary>
        public string UseYn { get; set; } = null!;

        /// <summary>
        /// 등록날짜
        /// </summary>
        public DateTime RegDt { get; set; }

        /// <summary>
        /// 의사대표진료과
        /// </summary>
        public string? MainMdCd { get; set; }

        /// <summary>
        /// 비대면진료시작(시)
        /// </summary>
        public int UntactStartHour { get; set; }

        /// <summary>
        /// 비대면진료시작(분)
        /// </summary>
        public int UntactStartMinute { get; set; }

        /// <summary>
        /// 비대면진료종료(시)
        /// </summary>
        public int UntactEndHour { get; set; }

        /// <summary>
        /// 비대면진료죵료(분)
        /// </summary>
        public int UntactEndMinute { get; set; }

        /// <summary>
        /// 비대면진료소요시간
        /// </summary>
        public int UntactIntervalTime { get; set; }

        /// <summary>
        /// 비대면사용유무
        /// </summary>
        public string UntactUseYn { get; set; } = null!;

        /// <summary>
        /// 비대면점심시작시간(시)
        /// </summary>
        public int UntactBreakStartHour { get; set; }

        /// <summary>
        /// 비대면점심시작시간(분)
        /// </summary>
        public int UntactBreakStartMinute { get; set; }

        /// <summary>
        /// 비대면점심죵료시간(분)
        /// </summary>
        public int UntactBreakEndHour { get; set; }

        /// <summary>
        /// 비대면점심죵료시간(분)
        /// </summary>
        public int UntactBreakEndMinute { get; set; }

        /// <summary>
        /// 프론트 노출여부[0:비노출, 1:헬로100, 2:헬로데스크, 4:키오스크]
        /// </summary>
        public int FrontViewRole { get; set; }
    }
}
