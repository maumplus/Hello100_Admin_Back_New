using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Dapper;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper
{
    /// <summary>
    /// Dapper 기술 확장
    /// </summary>
    public static class DapperExtensions
    {
        #region FIELD AREA ***************************************
        private const int _defaultTimeout = 15;
        #endregion

        #region INTERNAL STATIC METHOD AREA ********************************************
        private static CommandDefinition DefineCommand(string sql, object? param, IDbTransaction? tran, int? timeout)
            => new CommandDefinition(sql, param, tran, timeout ?? _defaultTimeout);
        #endregion

        #region GENERAL STATIC METHOD AREA **********************************************
        /// <summary>
        /// 첫 번째 행만 조회 (없으면 null 반환)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="logger"></param>
        /// <param name="timeout"></param>
        /// <param name="callerName"></param>
        /// <param name="lineNum"></param>
        /// <returns>단일 객체 <typeparamref name="T?"></typeparamref></returns>
        public static async Task<T?> QueryFirstOrDefaultAsync<T>(this DbSession db, string sql, object? param = null, ILogger? logger = null, int? timeout = null, [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNum = 0)
        {
            Stopwatch? sw = null;

            var paramDic = DapperParameterExtractor.ToDictionary(param as DynamicParameters);

            string sqlPreview = SqlParameterReplacer.ReplaceSqlParameters(sql, paramDic, logger);

            try
            {
                DapperLogHelper.LogQueryStart(logger, sqlPreview, paramDic, callerName, lineNum);

                sw = Stopwatch.StartNew();

                var result = await db.Connection.QueryFirstOrDefaultAsync<T>(DefineCommand(sql, param, db.Transaction, timeout));

                sw.Stop();

                DapperLogHelper.LogQuerySuccess(logger, sw.ElapsedMilliseconds, callerName, lineNum);

                return result;
            }
            catch (MySqlException e)
            {
                if (sw is { IsRunning: true }) sw.Stop();

                DapperLogHelper.LogQueryError(logger, e, sw?.ElapsedMilliseconds, sqlPreview, paramDic, callerName, lineNum);

                throw;
            }
            catch (Exception e)
            {
                if (sw is { IsRunning: true }) sw.Stop();

                DapperLogHelper.LogQueryError(logger, e, sw?.ElapsedMilliseconds, sqlPreview, paramDic, callerName, lineNum);

                throw;
            }
        }

        /// <summary>
        /// 여러 행/여러 컬럼 조회
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="logger"></param>
        /// <param name="timeout"></param>
        /// <param name="callerName"></param>
        /// <param name="lineNum"></param>
        /// <returns>결과 목록 <see cref="IEnumerable{T}"></see></returns>
        public static async Task<IEnumerable<T>> QueryAsync<T>(this DbSession db, string sql, object? param = null, ILogger? logger = null, int? timeout = null, [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNum = 0)
        {
            Stopwatch? sw = null;

            var paramDic = DapperParameterExtractor.ToDictionary(param as DynamicParameters);

            string sqlPreview = SqlParameterReplacer.ReplaceSqlParameters(sql, paramDic, logger);

            try
            {
                DapperLogHelper.LogQueryStart(logger, sqlPreview, paramDic, callerName, lineNum);

                sw = Stopwatch.StartNew();

                var result = await db.Connection.QueryAsync<T>(DefineCommand(sql, param, db.Transaction, timeout));

                sw.Stop();

                DapperLogHelper.LogQuerySuccess(logger, sw.ElapsedMilliseconds, callerName, lineNum);

                return result;
            }
            catch (MySqlException e)
            {
                if (sw is { IsRunning: true }) sw.Stop();

                DapperLogHelper.LogQueryError(logger, e, sw?.ElapsedMilliseconds, sqlPreview, paramDic, callerName, lineNum);

                throw;
            }
            catch (Exception e)
            {
                if (sw is { IsRunning: true }) sw.Stop();

                DapperLogHelper.LogQueryError(logger, e, sw?.ElapsedMilliseconds, sqlPreview, paramDic, callerName, lineNum);

                throw;
            }
        }

        /// <summary>
        /// 단일 값(Scalar) 조회 (COUNT, MAX, ID 등)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="logger"></param>
        /// <param name="timeout"></param>
        /// <param name="callerName"></param>
        /// <param name="lineNum"></param>
        /// <returns>단일 값(object 또는 지정 타입) <see cref="object"></see></returns>
        public static async Task<T?> ExecuteScalarAsync<T>(this DbSession db, string sql, object? param = null, ILogger? logger = null, int? timeout = null, [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNum = 0)
        {
            Stopwatch? sw = null;

            var paramDic = DapperParameterExtractor.ToDictionary(param as DynamicParameters);

            string sqlPreview = SqlParameterReplacer.ReplaceSqlParameters(sql, paramDic, logger);

            try
            {
                DapperLogHelper.LogQueryStart(logger, sqlPreview, paramDic, callerName, lineNum);

                sw = Stopwatch.StartNew();

                var result = await db.Connection.ExecuteScalarAsync<T>(DefineCommand(sql, param, db.Transaction, timeout));

                sw.Stop();

                DapperLogHelper.LogQuerySuccess(logger, sw.ElapsedMilliseconds, callerName, lineNum);

                return result;
            }
            catch (MySqlException e)
            {
                if (sw is { IsRunning: true }) sw.Stop();

                DapperLogHelper.LogQueryError(logger, e, sw?.ElapsedMilliseconds, sqlPreview, paramDic, callerName, lineNum);

                throw;
            }
            catch (Exception e)
            {
                if (sw is { IsRunning: true }) sw.Stop();

                DapperLogHelper.LogQueryError(logger, e, sw?.ElapsedMilliseconds, sqlPreview, paramDic, callerName, lineNum);

                throw;
            }
        }

        /// <summary>
        /// Non-query 실행 (INSERT/UPDATE/DELETE)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="logger"></param>
        /// <param name="timeout"></param>
        /// <param name="callerName"></param>
        /// <param name="lineNum"></param>
        /// <returns>영향받은 행 수 <see cref="int"></see></returns>
        public static async Task<int> ExecuteAsync(this DbSession db, string sql, object? param = null, ILogger? logger = null, int? timeout = null, [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNum = 0)
        {
            Stopwatch? sw = null;

            var paramDic = DapperParameterExtractor.ToDictionary(param as DynamicParameters);

            string sqlPreview = SqlParameterReplacer.ReplaceSqlParameters(sql, paramDic, logger);

            try
            {
                DapperLogHelper.LogQueryStart(logger, sqlPreview, paramDic, callerName, lineNum);

                sw = Stopwatch.StartNew();

                var result = await db.Connection.ExecuteAsync(DefineCommand(sql, param, db.Transaction, timeout));

                sw.Stop();

                DapperLogHelper.LogQuerySuccess(logger, sw.ElapsedMilliseconds, callerName, lineNum);

                return result;
            }
            catch (MySqlException e)
            {
                if (sw is { IsRunning: true }) sw.Stop();

                DapperLogHelper.LogQueryError(logger, e, sw?.ElapsedMilliseconds, sqlPreview, paramDic, callerName, lineNum);

                throw;
            }
            catch (Exception e)
            {
                if (sw is { IsRunning: true }) sw.Stop();

                DapperLogHelper.LogQueryError(logger, e, sw?.ElapsedMilliseconds, sqlPreview, paramDic, callerName, lineNum);

                throw;
            }
        }
        #endregion
    }
}
