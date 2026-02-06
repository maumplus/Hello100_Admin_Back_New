namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public class UpsertHello100SettingRequest
    {
        /// <summary>
        /// 공지ID
        /// </summary>
        public int NoticeId { get; set; }
        /// <summary>
        /// 설정ID
        /// </summary>
        public int StId { get; set; }
        /// <summary>
        /// Hello100 App 부가기능(모바일접수) [1: QR접수, 2: 당일접수, 4: 진료예약, 16: 당일접수마감, 32: 비대면진료, 64: 실손보험청구]
        /// 당일접수마감 체크 + 시간 입력 시 16 Flag 추가. (이 프로세스 맞는지???)
        /// </summary>
        public List<int>? Roles { get; init; }
        /// <summary>
        /// 대기인원표시 [0: 대기인원,시간 표시 / 1: 대기인원 표시 / 2: 대기시간 표시]
        /// </summary>
        public int AwaitRole { get; set; }
        /// <summary>
        /// 모바일 접수 마감시간 (당일 접수 마감)
        /// </summary>
        public string? ReceptEndTime { get; set; }
        /// <summary>
        /// 공지 내용
        /// </summary>
        public string? Notice { get; set; }
        /// <summary>
        /// 검사결과 알림 서비스 설정 [1: 자동전송, 2: 수동전송, 5: 알림만(기본), 9: 사용안함]
        /// </summary>
        public int ExamPushSet { get; set; }
    }
}
