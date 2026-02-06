namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results
{
    public sealed class GetHello100SettingResult
    {
        /// <summary>
        /// 요양기관번호
        /// </summary>
        public string HospNo { get; set; } = default!;
        /// <summary>
        /// 병원명
        /// </summary>
        public string Name { get; set; } = default!;
        /// <summary>
        /// 공지ID
        /// </summary>
        public int NoticeId { get; set; }
        /// <summary>
        /// 설정ID
        /// </summary>
        public int StId { get; set; }
        /// <summary>
        /// 대기시간
        /// </summary>
        public string WaitTm { get; set; } = default!;
        /// <summary>
        /// 모바일 접수 마감시간 (당일 접수 마감)
        /// </summary>
        public string? ReceptEndTime { get; set; }
        public string? CallReceptEndTime { get; set; }
        /// <summary>
        /// 차트타입 [E: 이지스전자차트, N: 닉스펜차트]
        /// </summary>
        public string ChartType { get; set; } = default!;
        public int Role { get; set; }
        public int AwaitRole { get; set; }
        /// <summary>
        /// 공지 내용
        /// </summary>
        public string? Content { get; set; }
        public string? SendYn { get; set; }
        public string? SendStartYmd { get; set; }
        public string? SendEndYmd { get; set; }
        /// <summary>
        /// 검사결과 알림 서비스 설정(1:자동전송, 2:수동전송, 5:알림만, 9:사용안함(기본))
        /// </summary>
        public int ExamPushSet { get; set; } = 9;
        /// <summary>
        /// 검사결과 알림 서비스 승인여부
        /// </summary>
        public string ExamApproveYn { get; set; } = "N";
    }
}
