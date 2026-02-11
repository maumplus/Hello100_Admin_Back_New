using Dapper;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Domain.Entities;
using System.Data;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.Notice
{
    public class NoticeRepository : INoticeRepository
    {
        #region FIELD AREA ****************************************
        private readonly ILogger<NoticeRepository> _logger;
        #endregion

        #region CONSTRUCTOR AREA *******************************************
        public NoticeRepository(ILogger<NoticeRepository> logger, IConfiguration config)
        {
            _logger = logger;
        }
        #endregion

        #region INOTICEREPOSITORY IMPLEMENTS AREA **********************************
        public async Task<int> CreateNoticeAsync(DbSession db, TbNoticeEntity noticeInfo, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Title", noticeInfo.Title, DbType.String);
            parameters.Add("Content", noticeInfo.Content, DbType.String);
            parameters.Add("SendType", noticeInfo.SendType, DbType.String);
            parameters.Add("ShowYn", noticeInfo.ShowYn, DbType.String);
            parameters.Add("DelYn", "N", DbType.String);

            #region == Query ==
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("  INSERT INTO tb_notice          ");
            sb.AppendLine("     (   title                   ");
            sb.AppendLine("     ,   content                 ");
            sb.AppendLine("     ,   send_type               ");
            sb.AppendLine("     ,   show_yn                 ");
            sb.AppendLine("     ,   del_yn                  ");
            sb.AppendLine("     ,   reg_dt                  ");
            sb.AppendLine("     ,   mod_dt                  ");
            sb.AppendLine("     )                           ");
            sb.AppendLine(" VALUES                          ");
            sb.AppendLine("     (   @Title                  ");
            sb.AppendLine("     ,   @Content                ");
            sb.AppendLine("     ,   @SendType               ");
            sb.AppendLine("     ,   @ShowYn                 ");
            sb.AppendLine("     ,   @DelYn                  ");
            sb.AppendLine("     ,   UNIX_TIMESTAMP(now())   ");
            sb.AppendLine("     ,   UNIX_TIMESTAMP(now())   ");
            sb.AppendLine("     ) ;                         ");
            #endregion

            var result = await db.ExecuteAsync(sb.ToString(), parameters, ct, _logger);

            if (result <= 0)
                throw new BizException(AdminErrorCode.CreateNoticeFailed.ToError());

            return result;
        }

        public async Task<int> UpdateNoticeAsync(DbSession db, TbNoticeEntity noticeInfo, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("NotiId", noticeInfo.NotiId, DbType.Int32);
            parameters.Add("Title", noticeInfo.Title, DbType.String);
            parameters.Add("Content", noticeInfo.Content, DbType.String);
            parameters.Add("SendType", noticeInfo.SendType, DbType.String);
            parameters.Add("ShowYn", noticeInfo.ShowYn, DbType.String);

            #region == Query ==
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("  UPDATE tb_notice                           ");
            sb.AppendLine("     SET title       = @Title                ");
            sb.AppendLine("     ,   content     = @Content              ");
            sb.AppendLine("     ,   send_type   = @SendType             ");
            sb.AppendLine("     ,   show_yn     = @ShowYn               ");
            sb.AppendLine("     ,   mod_dt      = UNIX_TIMESTAMP(NOW()) ");
            sb.AppendLine("   WHERE noti_id     = @NotiId               ");
            #endregion

            var result = await db.ExecuteAsync(sb.ToString(), parameters, ct, _logger);

            if (result <= 0)
                throw new BizException(AdminErrorCode.UpdateNoticeFailed.ToError());

            return result;
        }

        public async Task<int> DeleteNoticeAsync(DbSession db, int notiId, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("NotiId", notiId, DbType.Int32);

            #region == Query ==
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("  UPDATE tb_notice           ");
            sb.AppendLine("     SET del_yn = 'Y'        ");
            sb.AppendLine("   WHERE noti_id = @NotiId ; ");
            #endregion

            var result = await db.ExecuteAsync(sb.ToString(), parameters, ct, _logger);

            if (result <= 0)
                throw new BizException(AdminErrorCode.DeleteNoticeFailed.ToError());

            return result;
        }
        #endregion
    }
}
