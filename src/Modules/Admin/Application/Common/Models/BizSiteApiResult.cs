namespace Hello100Admin.Modules.Admin.Application.Common.Models
{
    public class BizSiteApiResult
    {
        public int ResultCd { get; set; }
        public string Message { get; set; }
        public int ErrorCode { get; set; }
    }

    public class BizSiteApiResult<T>
    {
        public int ResultCd { get; set; }
        public string Message { get; set; }
        public T ResultData { get; set; }
        public int ErrorCode { get; set; }
    }
}
