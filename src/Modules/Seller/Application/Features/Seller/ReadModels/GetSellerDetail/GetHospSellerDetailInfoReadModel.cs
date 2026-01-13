namespace Hello100Admin.Modules.Seller.Application.Features.Seller.ReadModels.GetSellerDetail
{
    public class GetHospSellerDetailInfoReadModel
    {
        /// <summary>
        /// 일련번호 (고유 ID)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 병원명
        /// </summary>
        public string HospName { get; set; } = "";

        /// <summary>
        /// 사업자 번호
        /// </summary>
        public string BusinessNo { get; set; } = "";

        /// <summary>
        /// 사업자 구분
        /// </summary>
        public string BusinessLevel { get; set; } = "";

        /// <summary>
        /// 요양기관 번호
        /// </summary>
        public string HospNo { get; set; } = "";

        /// <summary>
        /// 차트 구분
        /// </summary>
        public string ChartType { get; set; } = "";

        /// <summary>
        /// 은행 코드
        /// </summary>
        public string BankCd { get; set; } = "";

        /// <summary>
        /// 은행명
        /// </summary>
        public string BankName { get; set; } = "";

        /// <summary>
        /// 은행 로고 이미지 경로
        /// </summary>
        public string? BankImgPath { get; set; }

        /// <summary>
        /// 계좌 번호 (하이픈 없이)
        /// </summary>
        public string DepositNo { get; set; } = "";

        /// <summary>
        /// 예금주명
        /// </summary>
        public string Depositor { get; set; } = "";

        /// <summary>
        /// 활성여부(0:비활성, 1:활성)
        /// </summary>
        public string Enabled { get; set; } = "";

        /// <summary>
        /// 비고
        /// </summary>
        public string? Etc { get; set; }

        /// <summary>
        /// 연동 여부
        /// </summary>
        public string IsSync { get; set; } = "";

        /// <summary>
        /// 등록일 (UNIX TIMESTAMP)
        /// </summary>
        public int RegDt { get; set; }

        /// <summary>
        /// 수정일일 (UNIX TIMESTAMP)
        /// </summary>
        public int? ModDt { get; set; }
    }
}
