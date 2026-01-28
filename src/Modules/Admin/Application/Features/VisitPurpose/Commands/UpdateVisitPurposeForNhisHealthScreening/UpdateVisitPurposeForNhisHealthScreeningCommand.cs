using Hello100Admin.BuildingBlocks.Common.Application;

namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.UpdateVisitPurposeForNhisHealthScreening
{
    public record UpdateVisitPurposeForNhisHealthScreeningCommand : IQuery<Result>
    {
        /// <summary>
        /// 요양기관 키
        /// </summary>
        public string HospKey { get; init; } = default!;

        /// <summary>
        /// 노출 설정
        /// </summary>
        public string ShowYn { get; init; } = default!;

        /// <summary>
        /// 접수 구분 설정 (1: QR/당일접수, 4: 예약)
        /// </summary>
        public int Role { get; init; }

        /// <summary>
        /// 상세 항목 리스트 중 노출 체크 상태인 검진 코드(VpCd) 목록
        /// DetailShowYn -> DetailVpCodesToShow
        /// </summary>
        public List<string>? DetailShowYn { get; init; }
    }
}
