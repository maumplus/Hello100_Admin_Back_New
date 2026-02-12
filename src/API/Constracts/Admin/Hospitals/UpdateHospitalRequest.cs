namespace Hello100Admin.API.Constracts.Admin.Hospitals
{
    public sealed record UpdateHospitalRequest
    {
        /// <summary>
        /// 요양기관 키
        /// </summary>
        public required string HospKey { get; init; }
        /// <summary>
        /// 병원명
        /// </summary>
        public required string Name { get; init; }
        /// <summary>
        /// 주소
        /// </summary>
        public required string Addr { get; init; }
        /// <summary>
        /// 우편번호
        /// </summary>
        public required string PostCd { get; init; }
        /// <summary>
        /// 대표번호
        /// </summary>
        public required string Tel { get; init; }
        /// <summary>
        /// 홈페이지
        /// </summary>
        public string? Site { get; init; }
        /// <summary>
        /// 위경도 x좌표
        /// </summary>
        public double Lat { get; init; }
        /// <summary>
        /// 위경도 y좌표
        /// </summary>
        public double Lng { get; init; }
        /// <summary>
        /// 테스트병원여부
        /// 0:일반병원
        /// 1:테스트병원
        /// </summary>
        public int IsTest { get; init; }
        /// <summary>
        /// 진료과 코드 목록 (선택한 진료과 코드)
        /// </summary>
        public List<string>? MdCds { get; init; }
    }
}
