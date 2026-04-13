namespace Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions
{
    public static class UnixTimeExtensions
    {
        #region STATIC FIELD AREA ******************************************
        private static readonly TimeSpan KstOffset = TimeSpan.FromHours(9);
        #endregion

        #region STATIC METHOD AREA ******************************************
        /// <summary>
        /// Unix time (seconds)을 KST(UTC+9) 기준의 <see cref="DateTimeOffset"/>으로 변환
        /// </summary>
        /// <param name="unixTime">Unix timestamp (seconds)</param>
        /// <returns>KST 기준 DateTimeOffset, 값이 없으면 null</returns>
        public static DateTimeOffset? ToKstDateTimeOffset(this long? unixTime)
            => unixTime == null ? null : DateTimeOffset.FromUnixTimeSeconds(unixTime.Value).ToOffset(KstOffset);

        /// <summary>
        /// Unix time (seconds, int?)을 KST(UTC+9) 기준의 <see cref="DateTimeOffset"/>으로 변환
        /// 내부적으로 long?으로 변환하여 처리
        /// </summary>
        /// <param name="unixTime">Unix timestamp (seconds)</param>
        /// <returns>KST 기준 DateTimeOffset, 값이 없으면 null</returns>
        public static DateTimeOffset? ToKstDateTimeOffset(this int? unixTime)
            => ((long?)unixTime).ToKstDateTimeOffset();

        /// <summary>
        /// Unix time (seconds)을 KST(UTC+9) 기준의 문자열로 변환
        /// 기본 포맷: "yyyy-MM-dd HH:mm:ss"
        /// </summary>
        public static string? ToKstDateTimeString(this long? unixTime)
            => unixTime.ToKstDateTimeString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// Unix time (seconds)을 KST(UTC+9) 기준의 문자열로 변환 (포맷 지정)
        /// </summary>
        public static string? ToKstDateTimeString(this long? unixTime, string format)
            => unixTime.ToKstDateTimeOffset()?.ToString(format);

        /// <summary>
        /// Unix time (seconds, int?)을 KST(UTC+9) 기준의 문자열로 변환
        /// </summary>
        public static string? ToKstDateTimeString(this int? unixTime)
            => ((long?)unixTime).ToKstDateTimeString();

        /// <summary>
        /// Unix time (seconds, int?)을 KST(UTC+9) 기준의 문자열로 변환 (포맷 지정)
        /// </summary>
        public static string? ToKstDateTimeString(this int? unixTime, string format)
            => ((long?)unixTime).ToKstDateTimeString(format);
        #endregion
    }
}
