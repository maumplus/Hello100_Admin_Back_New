using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Domain.Entities;

namespace Hello100Admin.Modules.Admin.Domain.Repositories
{
    public interface IHospitalsRepository
    {
        /// <summary>
        /// 병원 생성
        /// </summary>
        /// <param name="db"></param>
        /// <param name="medicalEntities"></param>
        /// <param name="hospitalEntity"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<int> CreateHospitalAsync(
            DbSession db, string hospNo, List<TbHospitalMedicalInfoEntity>? medicalEntities, TbHospitalInfoEntity hospitalEntity, CancellationToken ct);

        /// <summary>
        /// 병원 정보 갱신
        /// </summary>
        /// <param name="db"></param>
        /// <param name="medicalEntities"></param>
        /// <param name="hospitalEntity"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<int> UpdateHospitalAsync(
            DbSession db, List<TbHospitalMedicalInfoEntity>? medicalEntities, TbHospitalInfoEntity hospitalEntity, CancellationToken ct);

        /// <summary>
        /// 병원 삭제
        /// </summary>
        /// <param name="db"></param>
        /// <param name="hospNo"></param>
        /// <param name="hospKey"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<int> DeleteHospitalAsync(DbSession db, string? hospNo, string hospKey, CancellationToken ct);
    }
}
