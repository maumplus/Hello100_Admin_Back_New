namespace Hello100Admin.Modules.Seller.Application.Features.Seller.ReadModels.GetSellerList
{
    public class GetHospSellerListReadModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int RowNum { get; set; }

        /// <summary>
        /// 고유 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 요양기관번호
        /// </summary>
        public string HospNo { get; set; } = null!;

        /// <summary>
        /// 병원명
        /// </summary>
        public string HospName { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public string BusinessLevel { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string BusinessNo { get; set; } = "";

        /// <summary>
        /// 판매자 ID (aid(관리자아이디) + emplno(의사번호))
        /// </summary>
        public string SellerId { get; set; } = null!;

        /// <summary>
        /// 은행 코드
        /// </summary>
        public string BankCd { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public string ChartType { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string BankName { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string? BankImgPath { get; set; } = "";

        /// <summary>
        /// 계좌 번호 (하이픈 없이)
        /// </summary>
        public string DepositNo { get; set; } = null!;

        /// <summary>
        /// 예금주명
        /// </summary>
        public string Depositor { get; set; } = null!;

        /// <summary>
        /// 활성여부(0:비활성, 1:활성)
        /// </summary>
        public string Enabled { get; set; } = null!;

        /// <summary>
        /// 비고
        /// </summary>
        public string? Etc { get; set; }

        /// <summary>
        /// 연동 여부
        /// </summary>
        public string IsSync { get; set; } = null!;

        /// <summary>
        /// 등록자 아이디
        /// </summary>
        public string RegAid { get; set; } = null!;

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
