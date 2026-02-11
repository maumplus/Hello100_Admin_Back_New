using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Domain.Entities;

namespace Hello100Admin.Modules.Admin.Domain.Repositories
{
    public interface INoticeRepository
    {
        /// <summary>
        /// 공지사항 신규 생성
        /// </summary>
        /// <param name="db"></param>
        /// <param name="noticeInfo"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<int> CreateNoticeAsync(DbSession db, TbNoticeEntity noticeInfo, CancellationToken ct);

        /// <summary>
        /// 공지사항 정보 갱신
        /// </summary>
        /// <param name="db"></param>
        /// <param name="noticeInfo"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<int> UpdateNoticeAsync(DbSession db, TbNoticeEntity noticeInfo, CancellationToken ct);

        /// <summary>
        /// 공지사항 삭제
        /// </summary>
        /// <param name="db"></param>
        /// <param name="notiId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<int> DeleteNoticeAsync(DbSession db, int notiId, CancellationToken ct);
    }
}
