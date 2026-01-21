namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Responses.GetExaminationResultAlimtalkApplicationInfo
{
    public record GetExaminationResultAlimtalkApplicationInfoResponse
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
        /// 알림톡 발송 서비스 신청(검사결과) 카카오 샘플 이미지 URL
        /// </summary>
        public string KakaoSampleImageUrl { get; init; } = default!;
    }
}
