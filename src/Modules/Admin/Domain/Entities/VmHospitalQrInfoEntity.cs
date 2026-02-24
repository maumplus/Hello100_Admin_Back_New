namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    public class VmHospitalQrInfoEntity
    {
        /// <summary>
        /// 관리자아이디
        /// </summary>
        public string Aid { get; set; } = null!;

        /// <summary>
        /// 계정아이디
        /// </summary>
        public string AccId { get; set; } = null!;

        /// <summary>
        /// 요양기관번호
        /// </summary>
        public string? HospNo { get; set; }

        /// <summary>
        /// 등급(tb_common:07)
        /// </summary>
        public string Grade { get; set; } = null!;

        /// <summary>
        /// 이름
        /// </summary>
        public string AdminName { get; set; } = null!;

        /// <summary>
        /// 전화번호
        /// </summary>
        public string? Tel { get; set; }

        /// <summary>
        /// 삭제유무
        /// </summary>
        public string DelYn { get; set; } = null!;

        public string? RegDt { get; set; }

        public string? LastLoginDt { get; set; }

        public string? AgreeDt { get; set; }

        /// <summary>
        /// QR아이디
        /// </summary>
        public string Qid { get; set; } = null!;

        /// <summary>
        /// 요양기관키
        /// </summary>
        public string? HospKey { get; set; }

        /// <summary>
        /// 이름
        /// </summary>
        public string? HospitalName { get; set; }

        /// <summary>
        /// 폐업유무
        /// </summary>
        public string? ClosingYn { get; set; }

        /// <summary>
        /// 대리점
        /// </summary>
        public string? Agency { get; set; }

        /// <summary>
        /// 공통이름
        /// </summary>
        public string? AgencyNm { get; set; }

        public string? QrCreateDt { get; set; }
    }
}
