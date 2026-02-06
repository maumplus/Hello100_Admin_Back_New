using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Domain.Entities;

namespace Hello100Admin.Modules.Admin.Domain.Repositories
{
    public interface IHospitalManagementRepository
    {
        /// <summary>
        /// 병원 정보 등록/수정
        /// </summary>
        /// <param name="db"></param>
        /// <param name="aId"></param>
        /// <param name="hospKey"></param>
        /// <param name="apprType"></param>
        /// <param name="reqJson"></param>
        /// <param name="imageUrls"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<int> UpsertHospitalAsync(
            DbSession db, string aId, string hospKey, string apprType, string reqJson, List<string> imageUrls, CancellationToken ct);


        /// <summary>
        /// 병원 정보(승인) 등록/수정
        /// </summary>
        /// <param name="db"></param>
        /// <param name="aId"></param>
        /// <param name="apprId"></param>
        /// <param name="hospNo"></param>
        /// <param name="hospKey"></param>
        /// <param name="description"></param>
        /// <param name="businessNo"></param>
        /// <param name="businessLevel"></param>
        /// <param name="mainMdCd"></param>
        /// <param name="clinicTimesEntity"></param>
        /// <param name="clinicTimesNewEntity"></param>
        /// <param name="deptCodesEntity"></param>
        /// <param name="keywordsEntity"></param>
        /// <param name="imagesEntity"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<int> UpsertAdmHospitalAsync
            (DbSession db, string aId, int apprId, string hospNo, string hospKey, string? description, string? businessNo, string? businessLevel, string? mainMdCd,
            List<TbEghisHospMedicalTimeEntity> clinicTimesEntity, List<TbEghisHospMedicalTimeNewEntity> clinicTimesNewEntity,
            List<TbHospitalMedicalInfoEntity> deptCodesEntity, List<TbEghisHospKeywordInfoEntity> keywordsEntity, List<TbImageInfoEntity> imagesEntity, CancellationToken ct);

        /// <summary>
        /// Hello100 설정 등록/수정
        /// </summary>
        /// <param name="db"></param>
        /// <param name="hospNo"></param>
        /// <param name="settingEntity"></param>
        /// <param name="noticeEntity"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<int> UpsertHello100SettingAsync(DbSession db, string hospNo, TbEghisHospSettingsInfoEntity settingEntity, TbNoticeEntity noticeEntity, CancellationToken ct);
    }
}
