using Hello100Admin.BuildingBlocks.Common.Application;

namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.UpdateVisitPurposeForNonNhisHealthScreening
{
    public record UpdateVisitPurposeForNonNhisHealthScreeningCommand : IQuery<Result>
    {
        /// <summary>
        /// 내원 키
        /// </summary>
        public string VpCd { get; init; } = default!;

        /// <summary>
        /// 제목 (Title)
        /// </summary>
        public string Name { get; init; } = default!;

        /// <summary>
        /// 요양기관 키
        /// </summary>
        public string HospKey { get; init; } = default!;

        /// <summary>
        /// 문진표 사용 여부
        /// </summary>
        public string PaperYn { get; init; } = default!;

        /// <summary>
        /// 상세항목 사용 여부
        /// </summary>
        public string DetailYn { get; init; } = default!;

        /// <summary>
        /// 노출 설정
        /// </summary>
        public string ShowYn { get; init; } = default!;

        /// <summary>
        /// 문진 필수 여부
        /// </summary>
        public string? InpuirySkipYn { get; init; }

        /// <summary>
        /// 문진 항목 인덱스 (PaperYn = 'Y'일 경우 필수)
        /// </summary>
        public int? InpuiryIdx { get; init; }

        /// <summary>
        /// 문진 경로
        /// </summary>
        public string InpuiryUrl { get; init; } = default!;

        /// <summary>
        /// 접수 구분 설정 (1: QR/당일접수, 4: 예약) 비트 단위 [OR 연산] 결과
        /// </summary>
        public int Role { get; init; }

        /// <summary>
        /// 삭제 여부
        /// </summary>
        public string DelYn { get; init; } = default!;

        /// <summary>
        /// 등록 날짜
        /// </summary>
        public string? RegDt { get; init; }

        /// <summary>
        /// 상세 항목 목록 (DetailYn = 'Y'일 경우 필수)
        /// </summary>
        public List<string>? Details { get; init; }
    }

    public record UpdateMyVisitPurposeForNonNhisHealthScreeningCommand : IQuery<Result>
    {
        /// <summary>
        /// 내원 키
        /// </summary>
        public string VpCd { get; init; } = default!;

        /// <summary>
        /// 제목 (Title)
        /// </summary>
        public string Name { get; init; } = default!;

        /// <summary>
        /// 요양기관번호
        /// </summary>
        public string HospNo { get; init; } = default!;

        /// <summary>
        /// 관리자 아이디
        /// </summary>
        public string AId { get; init; } = default!;

        /// <summary>
        /// 요양기관 키
        /// </summary>
        public string HospKey { get; init; } = default!;

        /// <summary>
        /// 문진표 사용 여부
        /// </summary>
        public string PaperYn { get; init; } = default!;

        /// <summary>
        /// 상세항목 사용 여부
        /// </summary>
        public string DetailYn { get; init; } = default!;

        /// <summary>
        /// 노출 설정
        /// </summary>
        public string ShowYn { get; init; } = default!;

        /// <summary>
        /// 문진 필수 여부
        /// </summary>
        public string? InpuirySkipYn { get; init; }

        /// <summary>
        /// 문진 항목 인덱스 (PaperYn = 'Y'일 경우 필수)
        /// </summary>
        public int? InpuiryIdx { get; init; }

        /// <summary>
        /// 문진 경로
        /// </summary>
        public string InpuiryUrl { get; init; } = default!;

        /// <summary>
        /// 접수 구분 설정 (1: QR/당일접수, 4: 예약) 비트 단위 [OR 연산] 결과
        /// </summary>
        public int Role { get; init; }

        /// <summary>
        /// 삭제 여부
        /// </summary>
        public string DelYn { get; init; } = default!;

        /// <summary>
        /// 등록 날짜
        /// </summary>
        public string? RegDt { get; init; }

        /// <summary>
        /// 상세 항목 목록 (DetailYn = 'Y'일 경우 필수)
        /// </summary>
        public List<string>? Details { get; init; }
    }
}
