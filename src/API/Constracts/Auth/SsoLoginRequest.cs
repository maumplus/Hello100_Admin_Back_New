using static Hello100Admin.BuildingBlocks.Common.Definition.Enums.GlobalConstant;

namespace Hello100Admin.API.Constracts.Auth
{
    public class SsoLoginRequest
    {
        /// <summary>
        /// 차트구분\
        /// E: 이지스차트\
        /// N: 닉스차트
        /// </summary>
        public required string ChartType { get; set; }
        /// <summary>
        /// 요양기관번호
        /// </summary>
        public required string Id { get; set; }
        /// <summary>
        /// 차트에서 전달받은 key
        /// </summary>
        public required string Key { get; set; }
    }
}
