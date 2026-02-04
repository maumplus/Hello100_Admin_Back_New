using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Domain.Entities;

namespace Hello100Admin.Modules.Admin.Domain.Repositories
{
    public interface IAdvertisementRepository
    {
        /// <summary>
        /// 팝업광고 등록
        /// </summary>
        /// <param name="db"></param>
        /// <param name="adInfo"></param>
        /// <param name="imageInfo"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<int> CreatePopupAsync(DbSession db, TbAdInfoEntity adInfo, TbImageInfoEntity imageInfo, CancellationToken ct);

        /// <summary>
        /// 팝업광고 수정
        /// </summary>
        /// <param name="db"></param>
        /// <param name="adInfo"></param>
        /// <param name="imageInfo"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<int> UpdatePopupAsync(DbSession db, TbAdInfoEntity adInfo, TbImageInfoEntity imageInfo, CancellationToken ct);

        /// <summary>
        /// 공통 광고 삭제 [팝업, 이지스배너]
        /// </summary>
        /// <param name="db"></param>
        /// <param name="adInfo"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<int> DeleteAdvertisementAsync(DbSession db, TbAdInfoEntity adInfo, CancellationToken ct);
    }
}
