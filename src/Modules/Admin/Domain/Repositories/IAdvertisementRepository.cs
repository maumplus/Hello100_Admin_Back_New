using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Domain.Entities;

namespace Hello100Admin.Modules.Admin.Domain.Repositories
{
    public interface IAdvertisementRepository
    {
        /// <summary>
        /// 공통 광고 등록 (팝업, 이지스 배너)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="adInfo"></param>
        /// <param name="imageInfo"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<int> CreateAdvertisementAsync(DbSession db, TbAdInfoEntity adInfo, TbImageInfoEntity imageInfo, CancellationToken ct);

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

        /// <summary>
        /// 이지스 배너 광고 순번 및 노출 여부 수정
        /// </summary>
        /// <param name="db"></param>
        /// <param name="adInfo"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<int> BulkUpdateEghisBannersAsync(DbSession db, List<TbAdInfoEntity> adInfo, CancellationToken ct);

        /// <summary>
        /// 이지스 배너 광고 수정
        /// </summary>
        /// <param name="db"></param>
        /// <param name="adInfo"></param>
        /// <param name="imageInfo"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<int> UpdateEghisBannerAsync(DbSession db, TbAdInfoEntity adInfo, TbImageInfoEntity imageInfo, CancellationToken ct);
    }
}
