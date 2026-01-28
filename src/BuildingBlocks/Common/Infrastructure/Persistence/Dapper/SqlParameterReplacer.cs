using Microsoft.Extensions.Logging;

namespace Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper
{
    /// <summary>
    /// 
    /// </summary>
    public static class SqlParameterReplacer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramDict"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static string ReplaceSqlParameters(string sql, IDictionary<string, object?> paramDict, ILogger? logger = null)
        {
            string query = sql;

            try
            {
                foreach (var (key, value) in paramDict)
                {
                    string placeholder = "@" + key;

                    string formatted = value switch
                    {
                        null => "[NULL]",
                        string s => $"'{s.Replace("'", "''")}'",
                        _ => $"'{value}'"
                    };


                    query = query.Replace(placeholder, formatted);
                }
            }
            catch (Exception e)
            {
                logger?.LogWarning(e, "Exception occurred while replace sql query.");
            }

            return query;
        }
    }
}
