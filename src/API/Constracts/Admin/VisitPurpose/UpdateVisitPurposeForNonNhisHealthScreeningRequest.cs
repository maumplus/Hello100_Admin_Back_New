namespace Hello100Admin.API.Constracts.Admin.VisitPurpose
{
    public sealed record UpdateVisitPurposeForNonNhisHealthScreeningRequest
    {
        /// <summary>
        /// 내원 키
        /// </summary>
        public required string VpCd { get; init; } = default!;

        /// <summary>
        /// 제목 (Title)
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// 문진표 사용 여부
        /// </summary>
        public required string PaperYn { get; init; }

        /// <summary>
        /// 상세항목 사용 여부
        /// </summary>
        public required string DetailYn { get; init; }

        /// <summary>
        /// 노출 설정
        /// </summary>
        public required string ShowYn { get; init; }

        /// <summary>
        /// 문진 항목 인덱스 (PaperYn = 'N'일 경우 -1)
        /// </summary>
        public int InquiryIdx { get; init; }

        /// <summary>
        /// 접수 구분 설정 (1: QR/당일접수, 4: 예약)
        /// </summary>
        public List<int>? Roles { get; init; }

        /// <summary>
        /// 상세 항목 목록 (DetailYn = 'Y'일 경우 필수)
        /// </summary>
        public List<string>? Details { get; init; }
    }
}
