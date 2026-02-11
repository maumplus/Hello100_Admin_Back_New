using Dapper;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper;
using System.Data;
using System.Text;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Microsoft.Extensions.Logging;
using Hello100Admin.Modules.Admin.Application.Features.Notice.Results;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.Notice
{
    public class NoticeStore : INoticeStore
    {
        #region FIELD AREA ****************************************
        private readonly ILogger<NoticeStore> _logger;
        #endregion

        #region CONSTRUCTOR AREA *******************************************
        public NoticeStore(ILogger<NoticeStore> logger)
        {
            _logger = logger;
        }
        #endregion

        #region INOTICESTORE IMPLEMENTS AREA **********************************
        public async Task<ListResult<GetNoticesResult>> GetNoticesAsync(DbSession db, int pageNo, int pageSize, string? searchKeyword, CancellationToken ct)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Keyword", searchKeyword ?? "", DbType.String);
            parameters.Add("Limit", pageSize, DbType.Int32);
            parameters.Add("OffSet", (pageNo - 1) * pageSize, DbType.Int32);
            parameters.Add("DelYn", "N", DbType.String);
            parameters.Add("Grade", "00", DbType.String);

            #region == Query ==
            StringBuilder sb = new StringBuilder();
            StringBuilder sbCondi = new StringBuilder();

            sbCondi.AppendLine($" del_yn = @DelYn ");
            sbCondi.AppendLine($" AND grade = @Grade ");
            sbCondi.AppendLine($" AND title LIKE CONCAT('%', @Keyword, '%') ");

            sb.AppendLine($"SET @total_cnt:= (SELECT COUNT(*) AS TotalCount FROM tb_notice WHERE {sbCondi.ToString()});");
            sb.AppendLine(" SET @rownum:= (@total_cnt+1) - @OffSet;         ");
            sb.AppendLine("  SELECT @rownum:= @rownum -1  AS RowNum         ");
            sb.AppendLine("     ,   noti_id				AS NotiId                       ");
            sb.AppendLine("     ,	title				AS Title                        ");
            sb.AppendLine("     ,	content				AS Content                      ");
            sb.AppendLine("     ,	send_type			AS SendType                     ");
            sb.AppendLine("     ,	show_yn				AS ShowYn                       ");
            sb.AppendLine("     ,	del_yn				AS DelYn                        ");
            sb.AppendLine("     ,	FROM_UNIXTIME(mod_dt, '%Y-%m-%d %H:%i') AS ModDt ");
            sb.AppendLine("     ,	FROM_UNIXTIME(reg_dt, '%Y-%m-%d %H:%i') AS RegDt ");
            sb.AppendLine("    FROM tb_notice                                ");
            sb.AppendLine($"  WHERE {sbCondi.ToString()}                    ");
            sb.AppendLine("   ORDER BY reg_dt DESC                          ");
            sb.AppendLine("   LIMIT @OffSet, @Limit;                        ");

            sb.AppendLine("  SELECT @total_cnt;                             ");
            #endregion

            var multi = await db.QueryMultipleAsync(sb.ToString(), parameters, ct, _logger);

            var result = new ListResult<GetNoticesResult>();

            result.Items = (await multi.ReadAsync<GetNoticesResult>()).ToList();
            result.TotalCount = await multi.ReadSingleAsync<int>();

            return result;
        }

        public async Task<GetNoticeResult> GetNoticeAsync(DbSession db, int notiId, CancellationToken ct)
        {
            var parameters = new DynamicParameters();
            parameters.Add("NotiId", notiId, DbType.Int32);

            #region == Query ==
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("  SELECT noti_id				AS NotiId                       ");
            sb.AppendLine("     ,	title				AS Title                        ");
            sb.AppendLine("     ,	content				AS Content                      ");
            sb.AppendLine("     ,	send_type			AS SendType                     ");
            sb.AppendLine("     ,	show_yn				AS ShowYn                       ");
            sb.AppendLine("     ,	del_yn				AS DelYn                        ");
            sb.AppendLine("     ,	FROM_UNIXTIME(mod_dt, '%Y-%m-%d %H:%i') AS ModDt ");
            sb.AppendLine("     ,	FROM_UNIXTIME(reg_dt, '%Y-%m-%d %H:%i') AS regDt ");
            sb.AppendLine("    FROM tb_notice                                           ");
            sb.AppendLine("   WHERE noti_id = @NotiId                                   ");
            sb.AppendLine("     AND del_yn = 'N'                                        ");
            #endregion

            var result = await db.QueryFirstOrDefaultAsync<GetNoticeResult>(sb.ToString(), parameters, ct, _logger);

            if (result == null)
                throw new BizException(AdminErrorCode.NoticeNotFound.ToError());

            return result;
        }
        #endregion
    }
}
