using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.Advertisement.Results;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence
{
    public interface IAdvertisementStore
    {
        /// <summary>
        /// 팝업광고 조회
        /// </summary>
        /// <param name="db"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<ListResult<GetPopupsResult>> GetPopupsAsync(DbSession db, int pageNo, int pageSize, CancellationToken ct);

        /// <summary>
        /// 팝업광고 단건 조회
        /// </summary>
        /// <param name="db"></param>
        /// <param name="popupId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<GetPopupResult> GetPopupAsync(DbSession db, int popupId, CancellationToken ct);

        /// <summary>
        /// 이지스 배너 조회
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<ListResult<GetEghisBannersResult>> GetEghisBannersAsync(DbSession db, CancellationToken ct);
    }
}
