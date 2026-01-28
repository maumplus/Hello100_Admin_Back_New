using Microsoft.Extensions.Logging;

namespace Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper
{
    /// <summary>
    /// 
    /// </summary>
    public static class DapperLogHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="caller"></param>
        /// <param name="lineNum"></param>
        public static void LogQueryStart(ILogger? logger, string sql, object? param, string? caller = null, int lineNum = 0)
        {
            logger?.LogInformation(
                "[QUERY][START] Caller Method [{Method} [{LineNum}]]| Params = {@Params}\n{Sql}",
                caller,
                lineNum,
                param,
                sql
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="elapsed"></param>
        /// <param name="caller"></param>
        /// <param name="lineNum"></param>
        public static void LogQuerySuccess(ILogger? logger, long elapsed, string? caller = null, int lineNum = 0)
        {
            logger?.LogInformation(
                "[QUERY][END] Caller Method [{Method} [{LineNum}]] | Completed in {Elapsed} ms",
                caller,
                lineNum,
                elapsed
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="ex"></param>
        /// <param name="elapsed"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="caller"></param>
        /// <param name="lineNum"></param>
        public static void LogQueryError(ILogger? logger, Exception ex, long? elapsed, string sql, object? param, string? caller = null, int lineNum = 0)
        {
            logger?.LogError(
                ex,
                "[QUERY][ERROR] Caller Method [{Method} [{LineNum}]] | Elapsed={Elapsed} ms | Params={@Params}\n{Sql}",
                caller,
                lineNum,
                elapsed,
                param,
                sql
            );
        }
    }
}
