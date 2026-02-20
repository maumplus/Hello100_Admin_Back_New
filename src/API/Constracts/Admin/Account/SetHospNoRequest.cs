namespace Hello100Admin.API.Constracts.Admin.Account
{
    public sealed record SetHospNoRequest
    {
        /// <summary>
        /// 요양기관번호(병원 고유번호)
        /// </summary>
        public string HospNo { get; set; } = default!;

        /// <summary>
        /// 요양기관키(병원 고유키)
        /// </summary>
        public string HospKey { get; set; } = default!;
        /// <summary>
        /// 차트타입 [E: 이지스전자차트, N: 닉스펜차트]
        /// </summary>
        public string ChartType { get; set; } = default!;
    }
}
