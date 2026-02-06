namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    /// <summary>
    /// 이지스병원설정정보
    /// </summary>
    public class TbEghisHospSettingsInfoEntity
    {
        /// <summary>
        /// 설정아이디
        /// </summary>
        public int StId { get; set; }
        /// <summary>
        /// 요양기관키
        /// </summary>
        public string HospKey { get; set; }
        /// <summary>
        /// 대기시간
        /// </summary>
        public string WaitTm { get; set; }
        /// <summary>
        /// Hello100 App 부가기능 [1: QR접수, 2: 오늘접수, 4: 진료예약, 32: 비대면진료, 64: 실손보험청구]
        /// </summary>
        public int Role { get; set; }
        /// <summary>
        /// 대기인원표시 (`0:대기인원,시간 표시 / 1:대기인원 표시 / 2: 대기시간표시)
        /// </summary>
        public int AwaitRole { get; set; }
        /// <summary>
        /// 모바일 접수 마감시간
        /// </summary>
        public string ReceptEndTime { get; set; }
        /// <summary>
        /// 등록날짜
        /// </summary>
        public int RegDt { get; set; }
        /// <summary>
        /// 검사결과 알림 서비스 설정(1:자동전송, 2:수동전송, 5:알림만(기본), 9:사용안함)
        /// </summary>
        public int ExamPushSet { get; set; }
    }
}
