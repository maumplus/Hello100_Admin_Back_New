

namespace Hello100Admin.Modules.Admin.Application.Features.Asset.Results
{
    public class GetUsageListResult
    {
        public int RowNum { get; set; }
        public string SerialKey { get; set; } = default!;
        public string LoginType { get; set; } = default!;
        public string HospNo { get; set; } = default!;
        public string HospName { get; set; } = default!;
        public string LicenseCd { get; set; } = default!;
        public string PushToken { get; set; } = default!;
        public string EmplNo { get; set; } = default!;
        public string OsType { get; set; } = default!;
        public string OsVer { get; set; } = default!;
        public string AppVer { get; set; } = default!;
        public string AgentVer { get; set; } = default!;
        public string AgentIp { get; set; } = default!;
        public string DeviceInfo { get; set; } = default!;
        public string AccessDt { get; set; } = default!;
        public string BeforeAccessDt { get; set; } = default!;
        public string RegDt { get; set; } = default!;
        public int AccessDiffDay { get; set; }
        public string QrCheck { get; set; } = default!;
    }
}
