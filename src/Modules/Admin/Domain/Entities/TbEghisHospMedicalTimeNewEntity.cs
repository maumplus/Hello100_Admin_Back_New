namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    public class TbEghisHospMedicalTimeNewEntity
    {
        /// <summary>
        /// 요양기관키
        /// </summary>
        public string HospKey { get; set; } = null!;

        /// <summary>
        /// 요양기관번호
        /// </summary>
        public string HospNo { get; set; } = null!;

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
        /// 사용유무
        /// </summary>
        public string UseYn { get; set; } = null!;

        /// <summary>
        /// 등록날짜
        /// </summary>
        public int RegDt { get; set; }
    }
}
