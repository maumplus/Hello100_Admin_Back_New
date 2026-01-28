using System.Text.Json;

namespace Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization
{
    public static class JsonSerializationExtensions
    {
        /// <summary>
        /// 객체를 JSON 문자열로 직렬화 (CamelCase & 한글 이스케이프)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string ToJson(this object obj)
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            return JsonSerializer.Serialize(obj, JsonOptions.Default);
        }

        /// <summary>
        /// 객체를 JSON 문자열로 직렬화 (DB 저장/로그 보관용)
        /// 한글 이스케이프 방지 + 가독성 유지
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string ToJsonForStorage(this object obj)
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            return JsonSerializer.Serialize(obj, JsonOptions.Storage);
        }

        public static T? FromJson<T>(this string json)
        {
            if (json is null)
                throw new ArgumentNullException(nameof(json));

            return JsonSerializer.Deserialize<T>(json, JsonOptions.Default)
                ?? throw new JsonException("Deserialization returned null.");
        }

        public static string? ToJsonOrNull(this object? obj)
        {
            return obj is null ? null : JsonSerializer.Serialize(obj, JsonOptions.Default);
        }
    }
}
