namespace Hello100Admin.Modules.Seller.Infrastructure.External.Http.Models
{
    public class WebRequestResult
    {
        /// <summary>
        /// http 상태 코드
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// 결과
        /// </summary>
        public string? ResponseData { get; set; }

        /// <summary>
        /// header 응답시간
        /// </summary>
        public string? HeaderDate { get; set; }
    }
}
