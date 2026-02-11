using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.Notice.Results;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence
{
    public interface INoticeStore
    {
        /// <summary>
        /// 공지사항 목록 조회
        /// </summary>
        /// <param name="db"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<ListResult<GetNoticesResult>> GetNoticesAsync(DbSession db, int pageNo, int pageSize, string? keyword, CancellationToken ct);

        /// <summary>
        /// 공지사항 상세 조회
        /// </summary>
        /// <param name="db"></param>
        /// <param name="notiId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<GetNoticeResult> GetNoticeAsync(DbSession db, int notiId, CancellationToken ct);
    }
}
