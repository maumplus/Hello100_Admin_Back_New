using System.Text.Json.Serialization;

namespace Hello100Admin.Modules.Admin.Application.Features.AdminUser.Results
{
    public sealed class GetHospitalAdminDetailResult
    {
        /// <summary>
        /// 관리자아이디
        /// </summary>
        public string AId { get; set; } = default!;
        /// <summary>
        /// 요양기관번호
        /// </summary>
        public string? HospNo { get; set; }
        /// <summary>
        /// 병원명
        /// </summary>
        public string? HospName { get; set; }
        /// <summary>
        /// 관리자명
        /// </summary>
        public string AdminName { get; set; } = default!;
        /// <summary>
        /// 권한 [S: 전체관리자, A: 대리점관리자, C: 병원관리자]
        /// </summary>
        public string Grade { get; set; } = default!;
        /// <summary>
        /// 전화번호
        /// </summary>
        public string? Tel { get; set; }
        /// <summary>
        /// QR 코드 발행일
        /// </summary>
        public string? QrCreateDt { get; set; }
        /// <summary>
        /// 최종 접속일
        /// </summary>
        public string? LastLoginDt { get; set; }
        /// <summary>
        /// QR 코드 URL
        /// </summary>
        public string QrUrl { get; set; } = default!;
        /// <summary>
        /// 접속 IP가 내부 IP인지 여부
        /// </summary>
        public bool IsCompanyIp { get; set; }
    }

    public class QrReqJson
    {
        [JsonPropertyName("frame_name")]
        public string FrameName { get; set; } = "no-frame";
        [JsonPropertyName("qr_code_text")]
        public string QrCodeText { get; set; } = default!;
        [JsonPropertyName("image_format")]
        public string ImageFormat { get; set; } = "PNG";
        [JsonPropertyName("image_width")]
        public int ImageWidth { get; set; } = 780;
        [JsonPropertyName("foreground_color")]
        public string ForegroundColor { get; set; } = "#000000";
        [JsonPropertyName("background_color")]
        public string BackgroundColor { get; set; } = "#ffffff";
        [JsonPropertyName("frame_color")]
        public string FrameColor { get; set; } = "#000000";
        [JsonPropertyName("frame_text_color")]
        public string FrameTextColor { get; set; } = "#ffffff";
        [JsonPropertyName("frame_icon_name")]
        public string FrameIconName { get; set; } = "app";
        [JsonPropertyName("frame_text")]
        public string? FrameText { get; set; }
        [JsonPropertyName("marker_left_template")]
        public string MarkerLeftTemplate { get; set; } = "version1";
        [JsonPropertyName("marker_right_template")]
        public string MarkerrightTemplate { get; set; } = "version1";
        [JsonPropertyName("marker_bottom_template")]
        public string MarkerBottomTemplate { get; set; } = "version1";
        [JsonPropertyName("download")]
        public int Download { get; set; } = 1;
    }
}
