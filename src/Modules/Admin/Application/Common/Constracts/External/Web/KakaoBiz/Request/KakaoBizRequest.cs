namespace Hello100Admin.Modules.Admin.Application.Common.Constracts.External.Web.KakaoBiz.Request
{
    public class KakaoBizRequest
    {
        public string HospNo { get; set; } = default!;
        public string FromDate { get; set; } = default!;
        public string ToDate { get; set; } = default!;
        public string EncKey { get; set; } = default!;
    }
}
