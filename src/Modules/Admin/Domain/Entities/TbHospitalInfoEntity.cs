namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    /// <summary>
    /// 병원/약국정보
    /// </summary>
    public class TbHospitalInfoEntity
    {
        /// <summary>
        /// 요양기관키
        /// </summary>
        public string HospKey { get; set; } = null!;

        /// <summary>
        /// 종별코드
        /// </summary>
        public string HospClsCd { get; set; } = null!;

        /// <summary>
        /// 이름
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 주소
        /// </summary>
        public string Addr { get; set; } = null!;

        /// <summary>
        /// 우편번호
        /// </summary>
        public string PostCd { get; set; } = null!;

        /// <summary>
        /// 전화번호
        /// </summary>
        public string Tel { get; set; } = null!;

        /// <summary>
        /// 사이트
        /// </summary>
        public string? Site { get; set; }

        /// <summary>
        /// 위경도x좌표
        /// </summary>
        public double Lat { get; set; }

        /// <summary>
        /// 위경도y좌표
        /// </summary>
        public double Lng { get; set; }

        /// <summary>
        /// 등록날짜
        /// </summary>
        public int RegDt { get; set; }

        /// <summary>
        /// 테스트병원여부(0:일반, 1:테스트)
        /// </summary>
        public sbyte IsTest { get; set; }
    }
}
