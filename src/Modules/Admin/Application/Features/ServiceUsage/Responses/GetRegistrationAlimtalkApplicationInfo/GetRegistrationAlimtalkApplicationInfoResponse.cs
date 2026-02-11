namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Responses.GetRegistrationAlimtalkApplicationInfo
{
    public sealed record GetRegistrationAlimtalkApplicationInfoResponse
    {
        /// <summary>
        /// 병원명
        /// </summary>
        public string Name { get; init; } = default!;

        /// <summary>
        /// 주소
        /// </summary>
        public string Addr { get; init; } = default!;

        /// <summary>
        /// 우편번호
        /// </summary>
        public string PostCd { get; init; } = default!;

        /// <summary>
        /// 전화번호
        /// </summary>
        public string Tel { get; init; } = default!;

        /// <summary>
        /// 신청 의사명
        /// </summary>
        public string? DoctNm { get; init; }
        /// <summary>
        /// 신청 의사 연락처
        /// </summary>
        public string? DoctTel { get; init; }
        /// <summary>
        /// 알림톡 발송 서비스 신청 여부
        /// </summary>
        public bool IsAlimtalkServiceApplied = false;

        /// <summary>
        /// 알림톡 발송 서비스 신청(진료접수) 카카오 샘플 이미지 URL
        /// </summary>
        public string KakaoSampleImageUrl { get; init; } = default!;
    }
}
