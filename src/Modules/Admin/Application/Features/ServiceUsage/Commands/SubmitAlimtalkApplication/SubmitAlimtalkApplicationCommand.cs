using Hello100Admin.BuildingBlocks.Common.Application;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Commands.SubmitAlimtalkApplication
{
    public record SubmitAlimtalkApplicationCommand : ICommand<Result>
    {
        /// <summary>
        /// 요양기관번호
        /// </summary>
        public string HospNo { get; init; } = default!;

        /// <summary>
        /// 요양기관 키
        /// </summary>
        public string HospKey { get; init; } = default!;

        /// <summary>
        /// 신청인
        /// </summary>
        public string DoctNm { get; init; } = default!;

        /// <summary>
        /// 휴대 전화번호(계약서 수신번호)
        /// </summary>
        public string DoctTel { get; init; } = default!;

        /// <summary>
        /// 신청 유형 ["": 알림톡 발송 서비스 신청(진료접수), "KakaoJoinTestResult": 알림톡 발송 서비스 신청(검사결과)]
        /// </summary>
        public string TmpType { get; init; } = default!;
    }
}
