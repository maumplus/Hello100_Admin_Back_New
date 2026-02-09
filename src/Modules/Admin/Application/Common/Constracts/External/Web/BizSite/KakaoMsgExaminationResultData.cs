namespace Hello100Admin.Modules.Admin.Application.Common.Constracts.External.Web.BizSite
{
    public class KakaoMsgExaminationResultData
    {
        //public string resultData { get; set; } // 원래 camelCase였으나, 다른 DTO들과 맞추기 위해 PascalCase로 변경. 파싱 문제 시 변경
        public string ResultData { get; set; }
    }
}
