namespace Hello100Admin.API.Constracts.Admin.Common
{
    /// <summary>
    /// 요양기관번호, 키의 경우 Request Query로 전달받지 않도록 별도의 Request record 생성
    /// Request가 요양기관번호, 키만 필요할 경우 사용하며, 프로퍼티 추가 금지
    /// </summary>
    public sealed record HospRequest
    {
        /// <summary>
        /// 요양기관번호
        /// </summary>
        public required string HospNo { get; init; }
        /// <summary>
        /// 요양기관키
        /// </summary>
        public required string HospKey { get; init; }
    }
}
