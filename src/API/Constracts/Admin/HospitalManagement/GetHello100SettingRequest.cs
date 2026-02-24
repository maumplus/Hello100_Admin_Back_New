namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public sealed record GetHello100SettingRequest
    {
        /// <summary>
        /// 요양기관번호
        /// </summary>
        public required string HospNo { get; init; }
        /// <summary>
        /// 요양기관 키
        /// </summary>
        public required string HospKey { get; init; }
    }
}
