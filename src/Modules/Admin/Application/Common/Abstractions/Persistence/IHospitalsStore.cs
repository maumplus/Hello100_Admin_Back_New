using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.Hospitals.Results;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence
{
    public interface IHospitalsStore
    {
        /// <summary>
        /// 병원목록 조회
        /// </summary>
        /// <param name="db"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchType"></param>
        /// <param name="searchKeyword"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<ListResult<SearchHospitalsResult>> SearchHospitalsAsync(DbSession db, int pageNo, int pageSize, int searchType, string? searchKeyword, CancellationToken ct = default);

        /// <summary>
        /// 특정 병원 조회
        /// </summary>
        /// <param name="db"></param>
        /// <param name="hospKey"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<GetHospitalDetailResult> GetHospitalDetailAsync(DbSession db, string hospKey, CancellationToken ct = default);

        /// <summary>
        /// 병원목록 엑셀 출력
        /// </summary>
        /// <param name="db"></param>
        /// <param name="searchType"></param>
        /// <param name="searchKeyword"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<List<ExportHospitalsExcelResult>> ExportHospitalsExcelAsync(DbSession db, int searchType, string? searchKeyword, CancellationToken ct = default);
    }
}
