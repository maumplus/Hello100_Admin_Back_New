namespace Hello100Admin.API.Constracts.Admin.ServiceUsage
{
    public record SubmitAlimtalkApplicationRequest
    {
        /// <summary>
        /// 신청인
        /// </summary>
        public required string DoctNm { get; init; }

        /// <summary>
        /// 휴대 전화번호(계약서 수신번호)
        /// </summary>
        public required string DoctTel { get; init; }

        /// <summary>
        /// 신청 유형 ["": 알림톡 발송 서비스 신청(진료접수), "KakaoJoinTestResult": 알림톡 발송 서비스 신청(검사결과)]
        /// </summary>
        public string? TmpType { get; init; } = "";
    }
}
