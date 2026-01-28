using Dapper;

namespace Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper
{
    /// <summary>
    /// 
    /// </summary>
    public static class DapperParameterExtractor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Dictionary<string, object?> ToDictionary(DynamicParameters? parameters)
        {
            var dict = new Dictionary<string, object?>();

            if (parameters == null)
            {
                return dict;
            }

            var names = parameters.ParameterNames?.Distinct().ToList();

            if (names == null || names.Count == 0)
            {
                return dict;
            }

            foreach (var name in names)
            {
                var value = parameters.Get<dynamic>(name);
                dict[name] = value;
            }

            return dict;
        }
    }
}
