namespace Hello100Admin.Modules.Admin.Application.Common.Constracts.External.Web.KakaoBiz.Response
{
    public sealed class KakaoBizResult<T>
    {
        public int ResultCd { get; set; }
        public string Message { get; set; }
        public T ResultData { get; set; }
        public int ErrorCode { get; set; }
    }

    public sealed class KakaoMsgSendHistoryDataSet
    {
        public int ListCount { get; set; }
        public List<KakaoMsgSendHistoryData> List { get; set; }
    }

    public sealed class KakaoMsgSendHistoryData
    {
        public int KtNum { get; set; }
        public string LicenseCd { get; set; }
        public string HospCd { get; set; }
        public string HospNm { get; set; }
        public string SendDate { get; set; }
        public string ResultCd { get; set; }
        public string ResultMsg { get; set; }
        public string Phone { get; set; }
        public string TemplateCd { get; set; }
        public string TemplateNm { get; set; }
        public string TemplateTypeNm { get; set; }
        public string HcResult { get; set; }
    }
}
