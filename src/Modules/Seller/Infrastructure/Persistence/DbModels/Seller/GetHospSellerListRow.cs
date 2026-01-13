namespace Hello100Admin.Modules.Seller.Infrastructure.Persistence.DbModels.Seller
{
    /// <summary>
    /// 병원 판매자 계좌 정보 테이블 레코드
    /// </summary>
    internal sealed record GetHospSellerListRow
    {
        /// <summary>
        /// 전체 행 수
        /// </summary>
        public int TotalCount { get; init; }

        /// <summary>
        /// 행 번호
        /// </summary>
        public int RowNum { get; init; }

        /// <summary>
        /// 고유 ID
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// 요양기관번호
        /// </summary>
        public string HospNo { get; init; } = default!;

        /// <summary>
        /// 병원명
        /// </summary>
        public string HospName { get; init; } = default!;

        /// <summary>
        /// 사업자 구분
        /// </summary>
        public string BusinessLevel { get; init; } = default!;

        /// <summary>
        /// 사업자 번호
        /// </summary>
        public string BusinessNo { get; init; } = default!;

        /// <summary>
        /// 판매자 ID (aid(관리자아이디) + emplno(의사번호))
        /// </summary>
        public string SellerId { get; init; } = default!;

        /// <summary>
        /// 은행 코드
        /// </summary>
        public string BankCd { get; init; } = default!;

        /// <summary>
        /// 차트 구분
        /// </summary>
        public string ChartType { get; init; } = default!;

        /// <summary>
        /// 은행명
        /// </summary>
        public string BankName { get; init; } = default!;

        /// <summary>
        /// 은행 로고 이미지 경로
        /// </summary>
        public string? BankImgPath { get; init; }

        /// <summary>
        /// 계좌 번호 (하이픈 없이)
        /// </summary>
        public string DepositNo { get; init; } = default!;

        /// <summary>
        /// 예금주명
        /// </summary>
        public string Depositor { get; init; } = default!;

        /// <summary>
        /// 활성여부(0:비활성, 1:활성)
        /// </summary>
        public string Enabled { get; init; } = default!;

        /// <summary>
        /// 비고
        /// </summary>
        public string? Etc { get; init; }

        /// <summary>
        /// 연동 여부
        /// </summary>
        public string IsSync { get; init; } = default!;

        /// <summary>
        /// 관리자 아이디
        /// </summary>
        public string RegAid { get; init; } = default!;

        /// <summary>
        /// 등록일 (UNIX TIMESTAMP)
        /// </summary>
        public int RegDt { get; init; }

        /// <summary>
        /// 수정일 (UNIX TIMESTAMP)
        /// </summary>
        public int? ModDt { get; init; }
    }
}
