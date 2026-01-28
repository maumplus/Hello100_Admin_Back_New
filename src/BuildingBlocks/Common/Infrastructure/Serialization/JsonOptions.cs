using System.Text.Encodings.Web;
using System.Text.Json;

namespace Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization
{
    public static class JsonOptions
    {
        /// <summary>
        /// 애플리케이션 기본 JSON 정책 (API, 내부 처리 등)
        /// </summary>
        public static readonly JsonSerializerOptions Default =
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                //// 값이 null인 속성은 JSON에서 제외
                //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                //// Enum 값을 숫자가 아니라 문자열 이름으로 직렬화
                //Converters =
                //{
                //    new JsonStringEnumConverter()
                //}
            };

        /// <summary>
        /// DB 저장/로그 보관용 JSON 정책
        /// 한글 이스케이프 방지 + 가독성 유지
        /// </summary>
        public static readonly JsonSerializerOptions Storage =
            new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
    }
}
