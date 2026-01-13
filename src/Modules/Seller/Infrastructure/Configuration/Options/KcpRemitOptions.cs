namespace Hello100Admin.Modules.Seller.Infrastructure.Configuration.Options
{
    public class KcpRemitOptions
    {
        /// <summary>
        /// KCP 상점 코드
        /// </summary>
        public string SiteCd { get; set; }

        /// <summary>
        /// KCP 연동 메인 URL 
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// PEM 인증서 경로 (상대 또는 절대 경로)
        /// </summary>
        public string CertPath { get; set; }
    }
}
